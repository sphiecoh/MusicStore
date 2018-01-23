using Nancy;
using Nancy.ModelBinding;
using Nancy.Security;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using NancyMusicStore.ViewModels;
using System;
using System.Data;

namespace NancyMusicStore.Modules
{
    public class StoreManagerModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        public StoreManagerModule(IDbHelper dbHelper) : base("/storemanager")
        {
            _dbHelper = dbHelper;
            this.RequiresAuthentication();

            Get("/", _ =>
            {
                
                var list = _dbHelper.Query<AlbumListViewModel>("get_all_albums", null, null, true, null, CommandType.StoredProcedure);
                return View["Index", list];
            });

            Get("/create", _ => View["Create"]);

            Post("/create", _ =>
            {
                var album = this.Bind<Album>();

                string cmd = "public.add_album";

                var res = _dbHelper.ExecuteScalar(cmd, new
                {
                    gid = album.GenreId,
                    aid = album.ArtistId,
                    t = album.Title,
                    p = album.Price,
                    aurl = album.AlbumArtUrl
                }, null, null, CommandType.StoredProcedure);

                if (Convert.ToInt32(res) > 0)
                {
                    return Response.AsRedirect("/storemanager");
                }
                ViewBag.Err = "Some Error Occurred";
                return View["Create"];
            });

            Get("/details/{id:int}", _ =>
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

            Get("/edit/{id:int}", _ =>
            {
                int id = 0;
                if (int.TryParse(_.id, out id))
                {
                    string cmd = "public.get_album_by_aid";
                    var album = _dbHelper.QueryFirstOrDefault<Album>(cmd, new
                    {
                        aid = id
                    }, null, null, CommandType.StoredProcedure);
                    if (album != null)
                    {
                        return View["Edit", album];
                    }
                }
                return View["Shared/Error"];
            });

            Post("/edit", _ =>
            {
                var album = this.Bind<Album>();
                string cmd = "public.update_album_by_aid";
                _dbHelper.Execute(cmd, new
                {
                    aid = album.AlbumId,
                    gid = album.GenreId,
                    arid = album.ArtistId,
                    t = album.Title,
                    p = album.Price,
                    aurl = album.AlbumArtUrl,
                    q = album.Quantity
                }, null, null, CommandType.StoredProcedure);
                return Response.AsRedirect("/storemanager");
            });

            Get("/delete/{id:int}", _ =>
            {
                int id = 0;
                if (int.TryParse(_.id, out id))
                {
                    string cmd = "public.get_album_by_aid";
                    var album = _dbHelper.QueryFirstOrDefault<Album>(cmd, new
                    {
                        aid = id
                    }, null, null, CommandType.StoredProcedure);
                    if (album != null)
                    {
                        return View["Delete", album];
                    }
                }
                return View["Shared/Error"];
            });

            Post("/delete/{id:int}", _ =>
            {
                int id = 0;
                if (int.TryParse(_.id, out id))
                {
                    string cmd = "public.delete_album_by_aid";
                    var res = _dbHelper.ExecuteScalar(cmd, new
                    {
                        aid = id
                    }, null, null, CommandType.StoredProcedure);
                    if (Convert.ToInt32(res) == 0)
                    {
                        return Response.AsRedirect("~/storemanager");
                    }
                }
                return View["Shared/Error"];
            });


            Get("/getallgenres", _ =>
            {
                string cmd = "public.get_all_genres";
                var list = _dbHelper.Query<Genre>(cmd, null, null, true, null, CommandType.StoredProcedure);
                return Response.AsJson(list);
            });

            Get("/getallartists", _ =>
            {
                string cmd = "public.get_all_artists";
                var list = _dbHelper.Query<Artist>(cmd, null, null, true, null, CommandType.StoredProcedure);
                return Response.AsJson(list);
            });
        }

    }
}