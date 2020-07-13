using DoubleMF.Data;
using DoubleMF.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DoubleMFTest
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Double-MF Testing project");

            try
            {
                var navData = new List<DoubleMF.Download.AMFData>();

                var dbAMC = new DoubleMF.Data.Services.AMCData();
                var amc = await dbAMC.GetAMCSAsync();

                var latestNAV = new DoubleMF.Download.AMFiindia();
                // if(await latestNAV.NAVDataDownloadAsync(DateTime.Now))
                navData = await latestNAV.GetAMDataFparseAsync();

                Console.WriteLine(navData.Count());
                foreach (var item in navData)
                {
                    Console.WriteLine($"{item.SchemeCode} ### {item.SchemeName} ### {item.Date}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                //Microsoft.EntityFrameworkCore, Version=3.1.5.0
                //System.Threading.Tasks.Extensions, Version=4.2.0.0,
            }

            Console.ReadKey();
        }
    }
}
