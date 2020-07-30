using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using DoubleMF.Data.Services;
using System.Collections.Generic;
using System.Linq;
using DoubleMF.Helper;
using Newtonsoft.Json.Linq;

namespace DoubleMF.AzFn
{
    public class HTTPGetDetailMF
    {
        private readonly Download.AMFiindia _aMFiindia;
        private readonly IAMCData _amcQuary;
        private readonly IMFData _mfQuary;
        private readonly INAVData _navQuary;

        public HTTPGetDetailMF(Download.AMFiindia aMFiindia,
           IAMCData amcData, IMFData mfData, INAVData navData)
        {
            _aMFiindia = aMFiindia ?? throw new ArgumentNullException(nameof(aMFiindia));
            _amcQuary = amcData ?? throw new ArgumentNullException(nameof(amcData));
            _mfQuary = mfData ?? throw new ArgumentNullException(nameof(mfData));
            _navQuary = navData ?? throw new ArgumentNullException(nameof(navData));
        }

        //[Disable]
        [FunctionName("GetAMC")]
        public async Task<ActionResult<List<Model.AMCDTO>>> FnGetAMC(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("requested for All AMC.");
            var sortedAMCs = (await _amcQuary.GetAllAMCAsync()).ToList();
            return new OkObjectResult(sortedAMCs);
        }

        //[Disable]
        [FunctionName("GetMFScheme")]
        //[FunctionName(nameof(GetMFScheme))]
        public async Task<ActionResult<List<Model.AssetManagtComp>>> FnGetMFScheme(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("requested for Mutual Fund Scheme");
            var sortedMFs = (await _mfQuary.GetMFSchemeAsync()).ToList();
            return new OkObjectResult(sortedMFs);
        }

        //[Disable]
        [FunctionName("GetMFSchemeDetails")]
        public async Task<ActionResult<List<Model.MFDetailDTO>>> FnGetMFSchemeDetails(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Requested for Mutual Fund Scheme details without AMC");
            int pageIndex = 1, pageSize = 10;
            string startsWith = string.Empty, amcIncluded = string.Empty;
            var isAmcIncluded = false;
            try
            {
                string pageindex = req.Query["pageindex"];
                string pagesize = req.Query["pagesize"];

                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                JObject data = JObject.Parse(requestBody);

                pageindex = pageindex ?? data?.GetValue("pageindex", StringComparison.OrdinalIgnoreCase)?.Value<string>();
                pagesize = pagesize ?? data?.GetValue("pagesize", StringComparison.OrdinalIgnoreCase)?.Value<string>();
                startsWith = data?.GetValue("startswith", StringComparison.OrdinalIgnoreCase)?.Value<string>();
                amcIncluded = data?.GetValue("amcIncluded", StringComparison.OrdinalIgnoreCase)?.Value<string>();

                //if (string.IsNullOrEmpty(startsWith)) { startsWith = string.Empty; }
                startsWith = startsWith ?? string.Empty;
                amcIncluded = amcIncluded ?? "false";

                if (!int.TryParse(pageindex, out pageIndex)) { return new ContentResult() { Content = "Invalid Page Index", StatusCode = StatusCodes.Status400BadRequest }; }
                if (!int.TryParse(pagesize, out pageSize)) { return new ContentResult() { Content = "Invalid Page Size", StatusCode = StatusCodes.Status400BadRequest }; }
                if (!bool.TryParse(amcIncluded, out isAmcIncluded)) { return new ContentResult() { Content = "Invalid value in AMC Included", StatusCode = StatusCodes.Status400BadRequest }; }

                //var sortedMFwithoutAMC = (await _mfQuary.GetMFSchemeDetailsAsync(PageIndex: pageIndex, PageSize: pageSize, StartsWith: startsWith, IsAMCIncluded: isAmcIncluded));
                var sortedMFwithoutAMC = await _mfQuary.GetMFSchemeDetailsAsync(PageIndex: pageIndex, PageSize: pageSize, AMCStartsWith: "P");
                return new OkObjectResult(sortedMFwithoutAMC);

            }
            catch (Exception ex)
            {
                log.LogError($"Error occurred while fetching Mutual Fund Scheme details without AMC.\nError: {ex.Message}");
                throw;
            }
        }
    }
}
