using Nancy;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace NancyMusicStore.Modules
{
    public class HomeModule : NancyModule
    {
        private readonly IDbHelper _dbHelper;
        public HomeModule(IDbHelper dbHelper) : base("/")
        {
            _dbHelper = dbHelper;

            Get("/", _ =>
            {
                var albums = GetTopSellingAlbums(5);
                return View["Index", albums];
            });
        }

        /// <summary>
        /// get top count selling albums 
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private List<Album> GetTopSellingAlbums(int count)
        {
         const string sql = "public.get_top_selling_albums";
            return _dbHelper.Query<Album>(sql, new
            {
                num = count
            }, null, true, null, CommandType.StoredProcedure).ToList();
        }
    }
}