using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;

namespace NbApp.Web.Models
{
    public class TheAppSetting
    {
        public string AppName { get; set; } = "NbApp";
        public string PathBase { get; set; } = "";
        public bool DebugMode { get; set; } = true;
        public string AutoRazorPageNavs { get; set; } = "Template";

        public List<string> ParseAutoRazorPageNavs()
        {
            var items = new List<string>();
            if (!string.IsNullOrWhiteSpace(AutoRazorPageNavs))
            {
                items = AutoRazorPageNavs.Split(',').Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).ToList();
                return items;
            }

            return items;
        }
    }

    public static class TheAppSettingSetup
    {
        public static void Setup(IServiceCollection services)
        {
            services.AddOptions<TheAppSetting>().BindConfiguration("TheAppSetting");
        }
    }
}