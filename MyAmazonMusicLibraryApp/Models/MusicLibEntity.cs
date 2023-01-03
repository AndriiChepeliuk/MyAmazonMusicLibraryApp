namespace MyAmazonMusicLibraryApp.Models
{
    public class MusicLibEntity
    {
        //playlist name, avatar, description
        public string? MusicLibType { get; set; }
        public string? MusicLibName { get; set; }
        public string? AlbumArtist { get; set; }
        public string? AvatarUrl { get; set; }
        public string? Description { get; set; }
        public int SongsCount { get; set; }
    }
}
