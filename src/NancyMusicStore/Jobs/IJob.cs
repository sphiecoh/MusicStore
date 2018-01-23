using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NancyMusicStore.Jobs
{
    interface IJob
    {
        void Run();
        string Name { get; set; }
        string Cron { get; set; }
        JobType JobType { get; set; }

    }
   public  enum JobType
    {
        Reccuring,
        OnceOff
    }
}
