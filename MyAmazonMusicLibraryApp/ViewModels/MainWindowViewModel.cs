using Avalonia.Media.Imaging;
using MyAmazonMusicLibraryApp.Models;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyAmazonMusicLibraryApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _musicLibraryEmpty = true;
        private bool _isBusy;
        private string? searchUrl;
        private MusicLibViewModel model;
        private Bitmap? _cover;
        private Regex playlistRegex = new Regex("^https?:\\/\\/music.amazon.com\\/playlists\\/[A-Z0-9]{10}$");
        private Regex albumRegex = new Regex("^https?:\\/\\/music.amazon.com\\/albums\\/[A-Z0-9]{10}$");
        private MusicLibEntity musicLibrary = new MusicLibEntity();
        private List<SongEntity> songs = new List<SongEntity>();

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

        public ICommand FindMusicLibCommand { get; }

        public string? SearchUrl
        {
            get => searchUrl;
            set => this.RaiseAndSetIfChanged(ref searchUrl, value);
        }
        public Bitmap? Cover
        {
            get => _cover;
            private set => this.RaiseAndSetIfChanged(ref _cover, value);
        }
        public MusicLibViewModel Model
        {
            get => model;
            set => this.RaiseAndSetIfChanged(ref model, value);
        }
        public bool IsBusy
        {
            get => _isBusy;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value);
        }
        public bool MusicLibraryEmpty
        {
            get => _musicLibraryEmpty;
            set => this.RaiseAndSetIfChanged(ref _musicLibraryEmpty, value);
        }

        public MainWindowViewModel()
        {
            FindMusicLibCommand = ReactiveCommand.Create(async () =>
            {
                IsBusy = true;

                if (!String.IsNullOrEmpty(SearchUrl) &&
                    (playlistRegex.IsMatch(SearchUrl) ||
                    albumRegex.IsMatch(SearchUrl)))
                {
                    MusicLibraryEmpty = false;
                    Model = new MusicLibViewModel(SearchUrl);
                    await Model.InitializeModel();
                    MusicLibrary = Model.MusicLibrary;
                    Cover = await LoadCoverAsync(Model.MusicLibrary.AvatarUrl);
                    Songs = Model.Songs;
                }

                IsBusy = false;
            });
        }

        public async Task<Bitmap> LoadCoverAsync(string coverUrl)
        {
            await using (var imageStream = await LoadCoverBitmapAsync(coverUrl))
            {
                return await Task.Run(() => Bitmap.DecodeToWidth(imageStream, 400));
            }
        }

        public async Task<Stream> LoadCoverBitmapAsync(string coverUrl)
        {
            var data = await new HttpClient().GetByteArrayAsync(coverUrl);

            return new MemoryStream(data);
        }
    }
}
