
using System.IO;
using Nancy;

namespace NancyMusicStore
{
    public class RootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            return Directory.GetCurrentDirectory();
        }
    }
}