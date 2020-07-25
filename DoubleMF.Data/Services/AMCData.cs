using DoubleMF.Model;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using DoubleMF.Download;
using DoubleMF.Helper;
using System.Diagnostics;

namespace DoubleMF.Data.Services
{
    public class AMCData : IAMCData
    {
        private DoubleMFDBContext _ctx;
        public AMCData(DoubleMFDBContext context)
        {
            _ctx = context;
            //used IOption as weel to pass the configuration connection string
        }
        public async Task<IEnumerable<AssetManagtComp>> GetAMCsAsync()
        {
            try
            {
                //var quary = from c in _ctx.AssetManagtComps select c;
                var quary = _ctx.assetManagtComps;
                return await quary.ToListAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<AssetManagtComp> GetAMCAsync(int AMCID)
        {
            try
            {
                return await _ctx.assetManagtComps.Where(e => e.AMCId == AMCID).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }

        public async Task<bool> SaveDownloadedAMCData(List<AMFDataModel> downloaded_data)
        {
            bool vReturn = false;

            var amc_data = new List<AssetManagtComp>();
            
            try
            {
                var existing_amc = await _ctx.assetManagtComps.ToListAsync();

                if (downloaded_data == null) return vReturn;
                var addAMCs = new List<AssetManagtComp>();

                foreach (var data in downloaded_data.Where(e => (e.Date == null) && (!e.SchemeCode.Contains('('))).GroupBy(g => g.SchemeCode)) //Filter, may required to change in future
                {
                    //Debug.WriteLine(data.Key);
                    if (existing_amc.Where(e => e.AMCName == data.Key).Count() == 0)
                    {    
                        addAMCs.Add(new AssetManagtComp { AMCName = data.Key });
                    }
                    _ctx.assetManagtComps.AddRange(addAMCs);
                }
                var dbActionCount = await _ctx.SaveChangesAsync();

                Debug.WriteLine($"AMC DB Action count {dbActionCount}");
                vReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return vReturn;
        }
    }
}
