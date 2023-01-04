using Avalonia.Media.Imaging;
using ReactiveUI;
using System;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyAmazonMusicLibraryApp.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool _isBusy;
        private string? searchUrl;
        private MusicLibViewModel model;
        private Bitmap? _cover;
        private Regex playlistRegex = new Regex("^https?:\\/\\/music.amazon.com\\/playlists\\/[A-Z0-9]{10}$");
        private Regex albumRegex = new Regex("^https?:\\/\\/music.amazon.com\\/albums\\/[A-Z0-9]{10}$");

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

        public MainWindowViewModel()
        {
            FindMusicLibCommand = ReactiveCommand.Create(async () =>
            {
                IsBusy = true;

                if (!String.IsNullOrEmpty(SearchUrl) &&
                    (playlistRegex.IsMatch(SearchUrl) ||
                    albumRegex.IsMatch(SearchUrl)))
                {
                    Model = new MusicLibViewModel(SearchUrl);
                    await Model.InitializeModel();
                    Cover = await LoadCoverAsync(Model.MusicLibrary.AvatarUrl);
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
