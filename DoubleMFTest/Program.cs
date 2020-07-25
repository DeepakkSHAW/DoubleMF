using DoubleMF.Data;
using DoubleMF.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DoubleMF.Helper;
using System.Runtime.CompilerServices;

namespace DoubleMFTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Double-MF Testing project");

            try
            {
                await UpdateDB2020(DateTime.Today.AddDays(-1));

                /*Calling Data access layer*/
                var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);


                var configuration = builder.Build();
                Debug.WriteLine(configuration.GetConnectionString("DoubleMFSqliteDB"));

                /*Using Repository Pattern to Fetch data from DB*/
                var services = new ServiceCollection();

                services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(configuration.GetConnectionString("DoubleMFSqliteDB"), options => options.MaxBatchSize(512)));
                services.AddDbContext<DoubleMFDBContext>(options => options.EnableSensitiveDataLogging());

                /*Adding AMC implementation to Dependency Injection*/
                services.AddTransient<DoubleMF.Data.Services.IAMCData, DoubleMF.Data.Services.AMCData>();
                services.AddTransient<DoubleMF.Data.Services.IMFData, DoubleMF.Data.Services.MFData>();
                services.AddTransient<DoubleMF.Data.Services.INAVData, DoubleMF.Data.Services.NAVData>();

                /*Fetching AMC implementation from DI*/
                var provider = services.BuildServiceProvider();
                var amcQuary = provider.GetService<DoubleMF.Data.Services.IAMCData>();

                //var vAMC = await amcQuary.GetAMCsAsync();
                //foreach (var amc in vAMC)
                //{
                //    Console.WriteLine($"{amc.AMCId} >> {amc.AMCName}");
                //}

                /* Download MF NAV Data from Internet */
                Console.WriteLine($"---MF Latest NAV download in progress.--");
                var navData = new List<DoubleMF.Download.AMFDataModel>();
                var latestNAV = new DoubleMF.Download.AMFiindia();
                DateTime dt = new DateTime(2020, 1, 1);

                if (await latestNAV.NAVDataDownloadAsync(dt))
                navData = await latestNAV.GetDownloadedDataAsync();
                Console.WriteLine($"---MF Latest NAV downloaded item {navData.Count()} completed.--");
                //foreach (var item in navData)
                //{
                //    Console.WriteLine($"{item.SchemeCode} ### {item.SchemeName} ### {item.Date}");
                //}

                Console.WriteLine($"---AMC update to db in progress--");
                var amcDone = await amcQuary.SaveDownloadedAMCData(navData);
                Console.WriteLine($"---AMC updated completed successfully : {amcDone.ToString()}.--");

                var mfQuary = provider.GetService<DoubleMF.Data.Services.IMFData>();
                Console.WriteLine($"---MF Details update to db in progress--");
                var mfDone = await mfQuary.SaveDownloadedData(navData);
                Console.WriteLine($"---MF details updated completed successfully : {mfDone.ToString()}.--");

                var navQuary = provider.GetService<DoubleMF.Data.Services.INAVData>();
                Console.WriteLine($"---NAV data update to db in progress--");
                var navResult = await navQuary.SaveDownloadeNAVdData(navData);
                Console.WriteLine($"---NAV data updated completed successfully : {navResult.ToString()}.--");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }

        protected static async Task UpdateDB2020(DateTime tillDate)
        {
            try
            {
                /*Using Repository Pattern to Fetch data from DB*/
                var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                var services = new ServiceCollection();
                var configuration = builder.Build();

                services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(configuration.GetConnectionString("DoubleMFSqliteDB"), options => options.MaxBatchSize(512)));
                services.AddDbContext<DoubleMFDBContext>(options => options.EnableSensitiveDataLogging());

                /*Adding AMC implementation to Dependency Injection*/
                services.AddTransient<DoubleMF.Data.Services.IAMCData, DoubleMF.Data.Services.AMCData>();
                services.AddTransient<DoubleMF.Data.Services.IMFData, DoubleMF.Data.Services.MFData>();
                services.AddTransient<DoubleMF.Data.Services.INAVData, DoubleMF.Data.Services.NAVData>();

                var provider = services.BuildServiceProvider();
                var amcQuary = provider.GetService<DoubleMF.Data.Services.IAMCData>();
                var mfQuary = provider.GetService<DoubleMF.Data.Services.IMFData>();
                var navQuary = provider.GetService<DoubleMF.Data.Services.INAVData>();

                DateTime StartDate = new DateTime(2020, 7, 15);
                DateTime EndDate = tillDate; // DateTime.Today.AddDays(-1);
                int DayInterval = 1;
                while (StartDate <= EndDate)
                {
                    StartDate = StartDate.AddDays(DayInterval);

                    Console.WriteLine($"---MF Latest NAV download in progress for {StartDate}.--");
                    var navData = new List<DoubleMF.Download.AMFDataModel>();
                    var latestNAV = new DoubleMF.Download.AMFiindia();

                    if (await latestNAV.NAVDataDownloadAsync(StartDate))
                        navData = await latestNAV.GetDownloadedDataAsync();
                    Console.WriteLine($"---MF Latest NAV downloaded item {navData.Count()} completed.--");

                    Console.WriteLine($"---AMC update to db in progress--");
                    var amcDone = await amcQuary.SaveDownloadedAMCData(navData);
                    Console.WriteLine($"---AMC updated completed successfully : {amcDone.ToString()}.--");

                    Console.WriteLine($"---MF Details update to db in progress--");
                    var mfDone = await mfQuary.SaveDownloadedData(navData);
                    Console.WriteLine($"---MF details updated completed successfully : {mfDone.ToString()}.--");


                    Console.WriteLine($"---NAV data update to db in progress--");
                    var navResult = await navQuary.SaveDownloadeNAVdData(navData);
                    Console.WriteLine($"---NAV data updated completed successfully : {navResult.ToString()}.--");

                    StartDate.AddDays(DayInterval);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}
