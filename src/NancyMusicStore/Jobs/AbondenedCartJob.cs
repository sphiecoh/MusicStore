using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Serilog;

namespace NancyMusicStore.Jobs {
    public class AbondenedCartJob : IJob {
        private readonly AppSettings appSettings;
        private readonly ILogger logger;

        public AbondenedCartJob (AppSettings appSettings, ILogger logger) {
            this.logger = logger;
            this.appSettings = appSettings;
        }
        public string Name { get; set; } = nameof (AbondenedCartJob);
        public string Cron { get; set; } = Hangfire.Cron.Daily ();
        public JobType JobType { get; set; } = JobType.Reccuring;

        public async Task Run () {
            using (var conn = new Npgsql.NpgsqlConnection (appSettings.DatabaseConnection)) {
                var carts = await conn.QueryAsync<dynamic> ("select * from carts where datecreated < @date ", new { date = DateTime.Today.AddDays (-1) });
                logger.Information("Found @count cart(s) to remove",carts.Count());
                foreach (var item in carts) {
                    await conn.ExecuteAsync ("update albums set quantity = quantity + @q where albumid = @aid", new { q = item.count, aid = item.albumid });
                    await conn.ExecuteAsync ("delete from carts where recordid = @rid", new { rid = item.recordid });
                }
            }
        }
    }
}