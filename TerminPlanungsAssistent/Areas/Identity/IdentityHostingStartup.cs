using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TerminPlanungsAssistent.Areas.Identity.Data;
using TerminPlanungsAssistent.Data;

[assembly: HostingStartup(typeof(TerminPlanungsAssistent.Areas.Identity.IdentityHostingStartup))]
namespace TerminPlanungsAssistent.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
                services.AddDbContext<TerminPlanungsAssistentContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("TerminPlanungsAssistentContextConnection")));

                services.AddDefaultIdentity<TerminPlanungsAssistentUser>(options => options.SignIn.RequireConfirmedAccount = true)
                    .AddEntityFrameworkStores<TerminPlanungsAssistentContext>();
            });
        }
    }
}