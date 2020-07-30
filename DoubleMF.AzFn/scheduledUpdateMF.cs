using System;
using System.Threading.Tasks;
using DoubleMF.Data.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using DoubleMF.Helper;

namespace DoubleMF.AzFn
{
    public class ScheduledUpdateMF
    {
        private readonly Download.AMFiindia _aMFiindia;
        private readonly IAMCData _amcQuary;
        private readonly IMFData _mfQuary;
        private readonly INAVData _navQuary;

        public ScheduledUpdateMF(Download.AMFiindia aMFiindia,
           IAMCData amcData, IMFData mfData, INAVData navData)
        {
            _aMFiindia = aMFiindia ?? throw new ArgumentNullException(nameof(aMFiindia));
            _amcQuary = amcData ?? throw new ArgumentNullException(nameof(amcData));
            _mfQuary = mfData ?? throw new ArgumentNullException(nameof(mfData));
            _navQuary = navData ?? throw new ArgumentNullException(nameof(navData));
        }

        //[Disable]
        [FunctionName("ScheduledUpdateMF")]
        public async Task FnScheduledUpdateMF([TimerTrigger("%ScheduleUpdate%")]TimerInfo myTimer, ILogger log)
        //public async Task FnScheduledUpdateMF([TimerTrigger("*/2 * * * *")] TimerInfo myTimer, ILogger log) //For Testing only
        {
            log.LogInformation($"Scheduled Update Mutual Funds NAV Sync kickoff at: {DateTime.UtcNow.ToString("dd-MMM-yy hh:mm:ss")} UTC");
            var theDate = DateTime.UtcNow.UtcToIst();

            if (await SharedFunction.MFCoreProcess(theDate, log, _aMFiindia, _amcQuary, _mfQuary, _navQuary))
            {
                log.LogWarning($"Mutual Funds Synchronization on Data {theDate.ToString("dd-MMM-yy hh:mm:ss")} has completed successfully.");
            }
            else
            {
                log.LogError($"Unable to download Mutual Fund data from AMFIndia on Dated:{theDate} IST");
            }
        }
    }
}
