using DoubleMF.Data.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DoubleMF.AzFn
{
    internal class SharedFunction
    {
        internal static async Task<bool> MFCoreProcess(DateTime theDate, ILogger log, Download.AMFiindia amfIndia, IAMCData amcData, IMFData mfData, INAVData navData)
        {

            var downloadedNAVData = new List<Download.AMFDataModel>();
            //******Download MF Data from AMFIndia******//
            log.LogWarning($"---Latest NAV & Other data download in progress from amfiindia for Date {theDate.ToString("dd-MMM-yy")}.--");
            if (await amfIndia.NAVDataDownloadAsync(theDate))
            //if (1 == 1) //Dummy test without download
            {
                log.LogWarning($"---Serialization of the downloaded in progress.--");

                downloadedNAVData = await amfIndia.GetDownloadedDataAsync();

                log.LogWarning($"---MF Latest NAV downloaded item {downloadedNAVData.Count()} completed.--");

                //******Database Operation to Begin (updated AMC, MUtual Fund Schemes and there NAV date)******//
                log.LogWarning($"---AMC update to db in progress--");

                var amcDone = await amcData.SaveDownloadedAMCData(downloadedNAVData);
                log.LogWarning($"AMC updated is {(amcDone == true ? "completed" : "failed")}\n---Mutual Fund Details update to db in progress--");

                var mfDone = await mfData.SaveDownloadedData(downloadedNAVData);
                log.LogWarning($"Mutual Fund scheme updated is {(mfDone == true ? "completed" : "failed")}\n---NAV details  update to db in progress--");

                var navDone = await navData.SaveDownloadeNAVdData(downloadedNAVData);
                log.LogWarning($"---NAV data updated is {(navDone == true ? "completed" : "failed")}.--");
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
