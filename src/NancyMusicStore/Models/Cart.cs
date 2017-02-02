using System;

namespace NancyMusicStore.Models
{
    public class Cart
    {       
        public int RecordId { get; set; }
        public string CartId { get; set; }
        public int AlbumId { get; set; }
        public int Count { get; set; }
        public DateTime DateCreated { get; set; }        
    }
}