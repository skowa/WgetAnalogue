using System.IO;
using System.Threading.Tasks;
using WgetAnalogue.Implementations;

namespace WgetAnalogue.Console
{
    public class Program
    {
        public static async Task Main()
        {
            var dataSaver = new DataSaverToDirectory(new DirectoryInfo($@"{Directory.GetCurrentDirectory()}\src"));
            var constraints = new Constraint(LinkTransitionConstraintType.CurrentDomain, new []{"img", "jpg", "js", "css"});
            var wget = new WebSiteDownloader(dataSaver);

            await wget.DownloadWebsiteAsync(@"https://stackoverflow.com/questions", constraints, 1);
        }
    }
}
