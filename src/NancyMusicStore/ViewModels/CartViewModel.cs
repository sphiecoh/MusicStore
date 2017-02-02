namespace NancyMusicStore.ViewModels
{
    public class CartViewModel
    {        
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public System.DateTime DateCreated { get; set; }

        public decimal Price { get; set; }
        public string Title { get; set; }
    }
}