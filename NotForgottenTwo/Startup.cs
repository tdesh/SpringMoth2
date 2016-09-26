using Microsoft.Owin;
using Owin;
using Hangfire;
using Hangfire.SqlServer;
using System;
using NotForgottenTwo.Services;

[assembly: OwinStartupAttribute(typeof(NotForgottenTwo.Startup))]
namespace NotForgottenTwo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);

            // GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            GlobalConfiguration.Configuration
             .UseSqlServerStorage(
                 "DefaultConnection",
                 new SqlServerStorageOptions { QueuePollInterval = TimeSpan.FromSeconds(1) });


            app.UseHangfireDashboard();
            app.UseHangfireServer();

            //The call to AddOrUpdate method will create a new recurring job or update existing job with the same identifier.
            RecurringJob.AddOrUpdate("NotificationServer", () => NotificationService.SendNextNotification(), Cron.MinuteInterval(1));

        }
    }
   
}
