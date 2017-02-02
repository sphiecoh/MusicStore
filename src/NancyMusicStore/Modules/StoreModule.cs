using Nancy;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using NancyMusicStore.ViewModels;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NancyMusicStore.Modules
{
    public class StoreModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        public StoreModule(IDbHelper dbHelper) : base("/store")
        {
            _dbHelper = dbHelper;

            Get("/", _ => View["Index", GetGenreList()]);

            Get("/genremenu", _ => Response.AsJson(GetGenreList()));

            Get("details/{id:int}",_ =>
            {
                int id = 0;
                if (int.TryParse(_.id, out id))
                {
                    string cmd = "public.get_album_details_by_aid";
                    var album = _dbHelper.QueryFirstOrDefault<AlbumDetailsViewModel>(cmd, new
                    {
                        aid = id
                    }, null, null, CommandType.StoredProcedure);
                    if (album != null)
                    {
                        return View["Details", album];
                    }
                }
                return View["Shared/Error"];
            });

            Get("browse/{genre}", _ =>
            {
                string genre = _.genre;
                ViewBag.Genre = genre;

                string cmd = "public.get_album_list_by_gname";
                var albumList = _dbHelper.Query<AlbumListViewModel>(cmd, new
                {
                    gname = genre
                }, null, true, null, CommandType.StoredProcedure).ToList();
                return View["Browse", albumList];
            });
        }

        private IList<Genre> GetGenreList()
        {
            string cmd = "public.get_all_genres";
            return _dbHelper.Query<Genre>(cmd, null, null, true, null, CommandType.StoredProcedure);
        }
    }
}