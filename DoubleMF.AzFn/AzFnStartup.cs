using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using DoubleMF.Download;
using DoubleMF.Data;
using System;

//*DK: Note It's very important to mention the FunctionsStartup with Assembly namespace*//
[assembly: FunctionsStartup(typeof(DoubleMF.AzFn.AzFnStartup))]

namespace DoubleMF.AzFn
{
    public interface IRepository
    {
        string GetData();
    }
    public class Repository : IRepository
    {
        public string GetData()
        {
            return $"DK Test - {DateTime.Now.ToString()}!";
        }
    }
    
    public class AzFnStartup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddScoped<IRepository, Repository>();
            builder.Services.AddScoped<AMFiindia>();

            //Debug.WriteLine(Configuration.GetConnectionString("DoubleMFSqliteDB"));
            var dbConnectionString = "DoubleMFSqliteDB";
            string conStr = System.Environment.GetEnvironmentVariable(dbConnectionString, EnvironmentVariableTarget.Process);
            //builder.Services.AddDbContext<DoubleMFDBContext>(options => options.UseSqlite(conStr, options => options.MaxBatchSize(512)));
            builder.Services.AddDbContext<DoubleMFDBContext>(_ => _.UseSqlite(conStr));
            builder.Services.AddDbContext<DoubleMFDBContext>(options => options.EnableSensitiveDataLogging());
            /*Adding AMC implementation to Dependency Injection to access DB Interfaces*/
            builder.Services.AddTransient<Data.Services.IAMCData, Data.Services.AMCData>();
            builder.Services.AddTransient<Data.Services.IMFData, Data.Services.MFData>();
            builder.Services.AddTransient<Data.Services.INAVData, Data.Services.NAVData>();

            // builder.Services.AddLogging();
            // Basic HTTP client factory usage
            //builder.Services.AddHttpClient();
            //////// Named HTTP Client
            //builder.Services.AddHttpClient("StartaAPI", s =>
            //{
            //    s.BaseAddress = new Uri(@"https://cit-u-stra-wb1.chisholm.edu.au/StrataWebClient_UAT/Api/Hyper/RetrieveTabularData");
            //    s.DefaultRequestHeaders.Add("X-Hyper-WebDataAccessKey", "TimesheetSvc:IVRpbWVzaGVldDE=");
            //    s.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            //    //s.DefaultRequestHeaders.Add(HttpMethod.Post.ToString(), "Post");
            //    s.DefaultRequestHeaders.Add(HeaderNames.ContentType, "application/json");
            //});
        }
    }
}
