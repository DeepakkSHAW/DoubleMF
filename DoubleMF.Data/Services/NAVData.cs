using DoubleMF.Download;
using DoubleMF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DoubleMF.Helper;
using Microsoft.EntityFrameworkCore.Internal;

namespace DoubleMF.Data.Services
{
    public class NAVData : INAVData
    {
        private DoubleMFDBContext _ctx;
        public NAVData(DoubleMFDBContext context)
        {
            _ctx = context;
        }
        public async Task<bool> SaveDownloadeNAVdData(List<AMFDataModel> downbloaded_data)
        {
            bool vReturn = false;
            var nav_data = new List<NetAssetValue>();
            try
            {
                var theDates = downbloaded_data.GroupBy(g => g.Date);
                var theDate = theDates.Select(e => e.Key);
                
                var existing_nav = await _ctx.netAssetValues.Where(s => s.OnDate == theDate.ToArray()[1]) .ToListAsync();
                var existing_mf = await _ctx.mutualFunds.ToListAsync();

                foreach (var data in downbloaded_data.Where(e => (e.Date != null)))
                {
                    if (data.Date != null)
                    {
                        if (data.SchemeCode.IsNumeric())
                        {
                            var mf_SchemeCode = data.SchemeCode.ToNumeric();
                            if (!existing_nav.Exists(e => (e.OnDate == data.Date) && (e.MF.MutualFundCode == mf_SchemeCode)))
                            {
                                var mf = existing_mf.Where(e => (e.MutualFundCode == mf_SchemeCode) && (e.DowbloadEnabled == false)).FirstOrDefault();
                                if (mf != null)
                                {
                                    nav_data.Add(new NetAssetValue { MF = mf, Price = (double)data.NetAssetValue, OnDate = (DateTime)data.Date });
                                }
                            }
                        }
                    }
                }
                _ctx.netAssetValues.AddRange(nav_data);
                var dbActionCount = await _ctx.SaveChangesAsync();
                Debug.WriteLine($"NAV DB Action count {dbActionCount}");
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
