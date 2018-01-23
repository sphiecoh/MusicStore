namespace NancyMusicStore.Database
{
    using System.Collections.Generic;
    using NancyMusicStore.Models;
    using GenFu;
    using System.Linq;
    using Dapper;

    public class SeedData 
    {
        public  static void Populate(string connectionString)
        {
            GenFu.Configure<Genre>().Fill(x => x.Name).AsMusicGenreName();
            GenFu.Configure<Artist>().Fill(x => x.Name).AsMusicArtistName();
            GenFu.Configure<Album>().Fill(x => x.AlbumArtUrl,"/Content/Images/placeholder.gif").Fill(x => x.Price).WithinRange(100,500);
            var rand = new System.Random(100); 
            using (var conn = new Npgsql.NpgsqlConnection(connectionString))
            {
                if(conn.ExecuteScalar<int>("select count(1) from albums ") == 0)
                {
                     for (int i = 0; i < 10; i++)
                     {
                        var genre = A.New<Genre>();
                        var artist = A.New<Artist>();
                        var genreid = conn.ExecuteScalar<int>("insert into genres (name,description) values (@name,@description) returning genreid;",new{genre.Name,genre.Description});
                        var artistid = conn.ExecuteScalar<int>("insert into artists (name) values (@name) returning artistid;",new{artist.Name});
                        var albums = A.ListOf<Album>(10);
                        albums.ForEach(a => {
                            conn.Execute("insert into albums(genreid,artistid,title,price,albumarturl) values(@genreid,@artistid,@title,@price,@albumarturl) returning albumid;", new { genreid,artistid , a.Title,a.Price ,a.AlbumArtUrl });
                        });
                     }
                }
            } 
        }
    }
}