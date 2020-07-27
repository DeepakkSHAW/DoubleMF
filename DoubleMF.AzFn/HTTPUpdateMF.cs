using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DoubleMF.Helper;
using System.Collections.Generic;
using System.Linq;
using DoubleMF.Data;
using Microsoft.Extensions.DependencyInjection;
using DoubleMF.Data.Services;
using System.Web.Mvc;
using ContentResult = Microsoft.AspNetCore.Mvc.ContentResult;

namespace DoubleMF.AzFn
{
    public class HTTPUpdateMF
    {
        private readonly IRepository _repository;
        private readonly Download.AMFiindia _aMFiindia;
        private readonly IAMCData _amcQuary;
        private readonly IMFData _mfQuary;
        private readonly INAVData _navQuary;

        public HTTPUpdateMF(IRepository repository, Download.AMFiindia aMFiindia,
           IAMCData amcData, IMFData mfData, INAVData navData)
        {
            _repository = repository;
            _aMFiindia = aMFiindia;
            _amcQuary = amcData;
            _mfQuary = mfData;
            _navQuary = navData;
        }
        //DK: Note copy the dll 'e_sqlite3.dll' to folder 'D:\home\site\tools' after deployment
        //Also, it might needed to copy this dll to bin folder 'bin\Debug\netcoreapp3.1\bin\' during development and testing
        //Another alternative change the Function .netcre framework to 2.2 you don't need any dll to copy

        //[Disable]
        [FunctionName("UpdateMFOnDate")]
        public async Task<IActionResult> HttpFnUpdateMFOnDate(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Update Mutual Funds onDate NAV Sync kickoff via http request");

            string onDate = req.Query["OnDate"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            onDate = onDate ?? data?.OnDate;
            if (string.IsNullOrEmpty(onDate)) { return new ContentResult() { Content = "No Content", StatusCode = StatusCodes.Status204NoContent }; }

            DateTime theDate;
            if (DateTime.TryParse(onDate, out theDate))
            {
                log.LogWarning($"From Repp {_repository.GetData()}");
                try
                {
                    if (theDate > DateTime.Today) { return new ContentResult() { Content = "Sync on future date is not possible", StatusCode = StatusCodes.Status400BadRequest }; }
                    if ((DateTime.Today.Date - theDate.Date).Days > 365 * 2) { return new ContentResult() { Content = "Possible Sync allowed within 2 years only", StatusCode = StatusCodes.Status400BadRequest }; }
                    if (await SharedFunction.MFCoreProcess(theDate, log, _aMFiindia, _amcQuary, _mfQuary, _navQuary))
                    {
                        return new OkObjectResult($"Mutual Funds Synchronization on Data {theDate.ToString("dd-MMM-yy")} has completed successfully.");
                    }
                    else
                    {
                        return new ContentResult() { Content = $"Unable to download Mutual Fund data from AMFIndia on Dated:{theDate}", StatusCode = StatusCodes.Status500InternalServerError };
                    }

                    ////******Download MF Data from AMFIndia******//
                    //log.LogWarning($"---MF Latest NAV download in progress.--");
                    //if (await _aMFiindia.NAVDataDownloadAsync(theDate))
                    ////if (1 == 1) //Dummy test without download
                    //{
                    //    log.LogWarning($"---Serialization of the downloaded in progress.--");
                    //    var navData = new List<Download.AMFDataModel>();
                    //    navData = await _aMFiindia.GetDownloadedDataAsync();
                    //    log.LogWarning($"---MF Latest NAV downloaded item {navData.Count()} completed.--");

                    //    //******Database Operation to Begin (updated AMC, MUtual Fund Schemes and there NAV date)******//
                    //    log.LogWarning($"---AMC update to db in progress--");

                    //    var amcDone = await _amcQuary.SaveDownloadedAMCData(navData);
                    //    log.LogWarning($"AMC updated is {(amcDone == true ? "completed" : "failed")}\n---Mutual Fund Details update to db in progress--");

                    //    var mfDone = await _mfQuary.SaveDownloadedData(navData);
                    //    log.LogWarning($"Mutual Fund scheme updated is {(mfDone == true ? "completed" : "failed")}\n---NAV details  update to db in progress--");

                    //    var navDone = await _navQuary.SaveDownloadeNAVdData(navData);
                    //    log.LogWarning($"---NAV data updated is {(navDone == true ? "completed" : "failed")}.--");
                    //}
                    //else
                    //{
                    //    return new ContentResult() { Content = $"Unable to download Mutual Fund data from AMFIndia on Dated:{theDate}", StatusCode = StatusCodes.Status500InternalServerError };
                    //}

                }
                catch (Exception ex)
                {
                    log.LogError($"Error occurred while Syncing Mutual Funds.\nError: {ex.Message}");
                    throw;
                }
            }
            else
            {
                //return new StatusCodeResult(500);
                //return new OkObjectResult("Bad Input");
                return new ContentResult() { Content = "Invalid Date", StatusCode = StatusCodes.Status400BadRequest };
            }
        }

        //[Disable]
        [FunctionName("UpdateMFOnMonth")]
        public async Task<IActionResult> HttpFnUpdateMFOnMohth(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Update Mutual Funds onMonth NAV Sync kickoff via http request");

            string onMonth = req.Query["OnMonth"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            onMonth = onMonth ?? data?.onMonth;
            if (string.IsNullOrEmpty(onMonth)) { return new ContentResult() { Content = "No Content", StatusCode = StatusCodes.Status204NoContent }; }

            DateTime theMonth;
            if (DateTime.TryParse(onMonth, out theMonth))
            {
                try
                {
                    if (theMonth.Year > DateTime.Today.Year)
                    {
                        return new ContentResult() { Content = "Sync on future YEAR is not possible", StatusCode = StatusCodes.Status400BadRequest };
                    }
                    else if(theMonth.Year == DateTime.Today.Year)
                    { 
                        if(theMonth.Month >= DateTime.Today.Month)
                        {
                            return new ContentResult() { Content = "Sync on future MONTH is not possible", StatusCode = StatusCodes.Status400BadRequest };
                        }
                    }

                    var dateRange = theMonth.GetDatesExceptWeekends(theMonth.Day);
                    foreach (var theDate in dateRange)
                    {
                        //log.LogWarning(theDate.ToString("dd-MMM-yy dddd"));
                        if (await SharedFunction.MFCoreProcess(theDate, log, _aMFiindia, _amcQuary, _mfQuary, _navQuary))
                        {
                            //return new OkObjectResult($"Mutual Funds Synchronization on Data {theDate.ToString("dd-MMM-yy ddd")} has completed successfully.");
                            log.LogWarning($"Mutual Funds Synchronization on Data {theDate.ToString("dd-MMM-yy ddd")} has completed successfully.");
                        }
                        else
                        {
                            //return new ContentResult() { Content = $"Unable to download Mutual Fund data from AMFIndia on Dated:{theDate}", StatusCode = StatusCodes.Status500InternalServerError };
                            log.LogError($"Unable to download Mutual Fund data from AMFIndia on Dated:{theDate}");
                        }
                    }

                    return new OkObjectResult($"Mutual Funds Synchronization on Data {theMonth.ToString("MMMM-yyyy")} has completed successfully.");
                }
                catch (Exception ex)
                {
                    log.LogError($"Error occurred while Syncing Mutual Funds.\nError: {ex.Message}");
                    throw;
                }
            }
            else
            {
                return new ContentResult() { Content = "Invalid Date", StatusCode = StatusCodes.Status400BadRequest };
            }
        }
    }
}
