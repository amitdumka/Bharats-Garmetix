using Garmetix.Commons.Dashboard.PageModels;
using Garmetix.CoreBase.Dashboard.Pages;
using Garmetix.CoreServices.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Garmetix.CoreBase.Dashboard
{
    public static class DashboardModule
    {
        public static MauiAppBuilder EnableDashboard(this  MauiAppBuilder builder)
        {
            builder.Services.AddSingleton<DashboardService>();//TODO: for time being it is here it should be move to shared module
            builder.Services.AddSingleton<DefaultDashboardPageModel>();
            builder.Services.AddSingleton<DashboardPage>();

            return builder;
        }
    }
}
