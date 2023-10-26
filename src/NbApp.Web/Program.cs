using NbApp.Web.Bootstrap;
using System.Threading.Tasks;

namespace NbApp.Web
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            await WebAppEntry.Run(args);
        }
    }
}