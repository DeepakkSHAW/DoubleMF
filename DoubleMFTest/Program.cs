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

namespace DoubleMFTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Double-MF Testing project");

            try
            {
                /* Download MF NAV Data from Internet */
                Console.WriteLine($"---MF Latest NAV download in progress.--");
                var navData = new List<DoubleMF.Download.AMFDataModel>();
                var latestNAV = new DoubleMF.Download.AMFiindia();
               // if (await latestNAV.NAVDataDownloadAsync(DateTime.Now.AddDays(-5)))
                    navData = await latestNAV.GetDownloadedDataAsync();
                Console.WriteLine($"---MF Latest NAV downloaded item {navData.Count()} completed.--");
                //foreach (var item in navData)
                //{
                //    Console.WriteLine($"{item.SchemeCode} ### {item.SchemeName} ### {item.Date}");
                //}

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
    }
}
