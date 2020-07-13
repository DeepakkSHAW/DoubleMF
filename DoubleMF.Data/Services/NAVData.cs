using DoubleMF.Download;
using DoubleMF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DoubleMF.Helper;

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

            try
            {
                foreach (var data in downbloaded_data.Where(e => (e.Date != null)))
                {
                    var mf_code = data.SchemeCode;
                    var nav_date = data.Date;
                    if (mf_code.IsNumeric())
                    {
                        var mf = await _ctx.mutualFunds.Where(e => e.MutualFundCode == data.SchemeCode.ToNumeric()).SingleOrDefaultAsync();
                        if (mf != null)
                        {
                            if (_ctx.netAssetValues.Where(e => (e.OnDate == data.Date) && (e.MF == mf)).Count() == 0)
                            {
                                var new_nav = new List<NetAssetValue>();
                                new_nav.Add(new NetAssetValue { Price = (double)data.NetAssetValue, OnDate = (DateTime)data.Date });

                                mf.NetAssetValues = new_nav;
                                _ctx.Update(mf);
                            }
                        }
                    }
                }
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
