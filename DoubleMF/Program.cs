using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DoubleMF.Data;
using Microsoft.EntityFrameworkCore;

namespace DoubleMF
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            await LoadAppSettingsAsync(builder);

            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            var db = builder.Configuration.GetConnectionString("DoubleMFSqliteDB");
            var serverlessBaseURI = builder.Configuration["ServerlessBaseURI"];
            System.Diagnostics.Debug.WriteLine($"DB Name: {db}");
            System.Diagnostics.Debug.WriteLine($"Other Configure: {serverlessBaseURI}");

            try
            {
                /*Using Repository Pattern to Fetch data from DB*/

                //var services = new ServiceCollection();
                //System.Diagnostics.Debug.WriteLine("1");
                //builder.Services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(db, options => options.MaxBatchSize(512)));
                //builder.Services.AddDbContext<DoubleMFDBContext>(options => options.EnableSensitiveDataLogging());
                //System.Diagnostics.Debug.WriteLine("2");
                ///*Adding AMC implementation to Dependency Injection*/
                //builder.Services.AddTransient<DoubleMF.Data.Services.IAMCData, DoubleMF.Data.Services.AMCData>();
                //builder.Services.AddTransient<DoubleMF.Data.Services.IMFData, DoubleMF.Data.Services.MFData>();
                //builder.Services.AddTransient<DoubleMF.Data.Services.INAVData, DoubleMF.Data.Services.NAVData>();

                //System.Diagnostics.Debug.WriteLine("3");
                ///*Fetching AMC implementation from DI*/
                //var provider = builder.Services.BuildServiceProvider();
                //System.Diagnostics.Debug.WriteLine("4");
                //var amcQuary = provider.GetService<Data.Services.IAMCData>();
                //System.Diagnostics.Debug.WriteLine("5");
                //var vAMC = await amcQuary.GetAMCsAsync();

                //foreach (var amc in vAMC)
                //{
                //    System.Diagnostics.Debug.WriteLine($"{amc.AMCId} >> {amc.AMCName}");
                //}

                await builder.Build().RunAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }

        }
        /// <summary>
        /// Load appsettings.json from the  wwwroot folder
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static async Task LoadAppSettingsAsync(WebAssemblyHostBuilder builder)
        {
            // read JSON file as a stream for configuration
            var client = new HttpClient() { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
            // the appsettings file must be in 'wwwroot'
            using var response = await client.GetAsync("appsettings.json");
            using var stream = await response.Content.ReadAsStreamAsync();
            builder.Configuration.AddJsonStream(stream);
            builder.Services.AddTransient<Data.Services.ISayHello, Data.Services.SayHello>();

            var db = builder.Configuration.GetConnectionString("DoubleMFSqliteDB");
            System.Diagnostics.Debug.WriteLine($"JAG-DIV: {db}");
            db = @"Data Source=DoubleMF.db";
            //"Data Source=DB\\ETracker.db"
            builder.Services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(db));
            //builder.Services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(db, options => options.MaxBatchSize(512)));
            //builder.Services.AddDbContext<DoubleMFDBContext>(options => options.EnableSensitiveDataLogging());
            builder.Services.AddTransient<DoubleMF.Data.Services.IAMCData, DoubleMF.Data.Services.AMCData>();
        }
    }
}
