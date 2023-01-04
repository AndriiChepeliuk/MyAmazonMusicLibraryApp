using CefSharp.OffScreen;
using CefSharp;
using MyAmazonMusicLibraryApp.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using HtmlAgilityPack;
using ReactiveUI;

namespace MyAmazonMusicLibraryApp.ViewModels
{
    public class MusicLibViewModel : ViewModelBase
    {
        private HtmlDocument htmlDocument;
        private MusicLibEntity musicLibrary = new MusicLibEntity();
        private List<SongEntity> songs = new List<SongEntity>();
        private string url;
        private readonly string musicLibHtmlXPath = "//*//music-app/div[3]/div/div/div/music-detail-header";

        Dictionary<string, string> songsContentHtmlXPath = new Dictionary<string, string>()
        {
            ["PLAYLIST"] = "//*//music-container/div/div[2]/div/div/music-image-row",
            ["ALBUM"] = "//*//music-container/div/div[2]/div/div/music-text-row"
        };

        public MusicLibEntity MusicLibrary
        {
            get => musicLibrary;
            set => this.RaiseAndSetIfChanged(ref musicLibrary, value);
        }
        public List<SongEntity> Songs
        {
            get => songs;
            set => this.RaiseAndSetIfChanged(ref songs, value);
        }
        public string Url { get { return url; } set { this.url = value; } }

        public MusicLibViewModel(string searchUrl)
        {
            this.url = searchUrl;
        }

        public async Task InitializeModel()
        {
            using (ChromiumWebBrowser browser = new ChromiumWebBrowser(Url))
            {
                browser.Size = new Size(1920, 5000);

                var initialLoadResponse = await browser.WaitForInitialLoadAsync();

                if (!initialLoadResponse.Success)
                {
                    throw new Exception(string.Format("Page load failed with ErrorCode:{0}, HttpStatusCode:{1}", initialLoadResponse.ErrorCode, initialLoadResponse.HttpStatusCode));
                }

                await Task.Delay(5000);

                var response = await browser.EvaluateScriptAsync("document.documentElement.outerHTML");
                string html = (String)response.Result;
                htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
            }

            MusicLibrary = GetMusicLibEntity(musicLibHtmlXPath);



        }

        public MusicLibEntity GetMusicLibEntity(string musicLibHtmlXPath)
        {
            var musicLib = new MusicLibEntity();
            var musicLibData = htmlDocument.DocumentNode.SelectSingleNode(musicLibHtmlXPath);

            musicLib.SongsCount = int.Parse(musicLibData.GetAttributeValue("tertiary-text", "").Split(' ')[0]);
            musicLib.MusicLibType = musicLibData.GetAttributeValue("label", "").ToUpper();
            musicLib.MusicLibName = musicLibData.GetAttributeValue("headline", "");
            musicLib.AvatarUrl = musicLibData.GetAttributeValue("image-src", "");

            switch (musicLib.MusicLibType)
            {
                case "PLAYLIST":
                    musicLib.Description = musicLibData.GetAttributeValue("secondary-text", "");
                    break;

                case "ALBUM":
                    musicLib.Description = musicLib.AlbumArtist = musicLibData.GetAttributeValue("primary-text", "");
                    break;

                default:
                    musicLib.Description = "";
                    break;
            }

            return musicLib;
        }

        //public List<SongEntity> GetPlaylistSongs(
        //    HtmlDocument htmlDocument,
        //    string songsContentHtmlXPath,
        //    int musicLibSongsCount)
        //{
        //    var songs = new List<SongEntity>();
        //    var

        //    return songs;
        //}
    }
}
