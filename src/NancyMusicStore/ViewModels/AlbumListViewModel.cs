namespace NancyMusicStore.ViewModels
{
    public class AlbumListViewModel
    {
        public int AlbumId { get; set; }

        public string GenreName { get; set; }

        public string Title { get; set; }

        public string AlbumArtUrl { get; set; }

        public string ArtistName { get; set; }

        public decimal Price { get; set; }
        public int Quantity{get;set;}
         public string BgColor => Quantity < 20 ? "red":string.Empty;
    }
}