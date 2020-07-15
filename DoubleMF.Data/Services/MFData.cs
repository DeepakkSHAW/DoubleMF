using DoubleMF.Download;
using DoubleMF.Helper;
using DoubleMF.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DoubleMF.Data.Services
{
    public class MFData : IMFData
    {
        private DoubleMFDBContext _ctx;
        public MFData(DoubleMFDBContext context)
        {
            _ctx = context;
        }
        public async Task<bool> SaveDownloadedData(List<AMFDataModel> downloaded_data)
        {
            bool vReturn = false;
            var mutual_funds = new List<MutualFund>();

            try
            {
                var existing_amc = await _ctx.assetManagtComps.ToListAsync();
                var existing_mf = await _ctx.mutualFunds.ToListAsync();

                var found_amc = new AssetManagtComp();

                if (downloaded_data == null) return vReturn;

                foreach (var data in downloaded_data.Where(e => !e.SchemeCode.Contains('(')))
                {
                    if (data.Date == null)
                    {
                        found_amc = existing_amc.Where(e => e.AMCName == data.SchemeCode).FirstOrDefault();
                    }
                    else
                    {
                        if (data.SchemeCode.IsNumeric())
                        {
                            var mf_SchemeCode = data.SchemeCode.ToNumeric();
                            if (!existing_mf.Exists(e => e.MutualFundCode == mf_SchemeCode))
                            {
                                mutual_funds.Add(new MutualFund { AMC = found_amc, MutualFundName = data.SchemeName, MutualFundCode = mf_SchemeCode, DowbloadEnabled = false });
                            }
                        }
                    }
                }
                _ctx.mutualFunds.AddRange(mutual_funds);
                var dbActionCount = await _ctx.SaveChangesAsync();
                Debug.WriteLine($"MF DB Action count {dbActionCount}");
                vReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            return vReturn;
        }
        //public async Task<bool> SaveDownloadedData_delete(List<AMFDataModel> dowbloaded_data)
        //{
        //    var vReturm = false;
        //    var holdSchemeName = string.Empty;
        //    List<NetAssetValue> nav_values = new List<NetAssetValue>();
        //    string amc_name = string.Empty, mf_name = string.Empty;
        //    //----------------------
        //    var found_amc = _ctx.assetManagtComps.Where(e => e.AMCName == "DK").FirstOrDefault();
        //    var list_mf = new List<MutualFund>();
        //    var mf1 = new MutualFund();
        //    var mf2 = new MutualFund();

        //    mf1.MutualFundName = "mf1";
        //    mf2.MutualFundName = "mf2";
        //    list_mf.AddRange(new List<MutualFund> { mf1, mf2 });
        //    try
        //    {
        //        found_amc.MutualFunds = list_mf;
        //        _ctx.Update(found_amc);
        //        var ii = await _ctx.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        Debug.WriteLine(ex.Message);
        //        throw;
        //    }
        //    //----------------------
        //    if (dowbloaded_data == null) return vReturm;
        //    try
        //    {
        //        await _ctx.assetManagtComps.ToArrayAsync(); //DK: Remove this line
        //        foreach (var item in dowbloaded_data)
        //        {
        //            if (item.Date != null)
        //            {
        //                if (holdSchemeName != item.SchemeName)
        //                {
        //                    holdSchemeName = item.SchemeName;

        //                    /*Save Data to DB*/
        //                    if (nav_values.Count > 0)
        //                    {
        //                        var mf = new MutualFund() { MutualFundName = mf_name, AMC = new AssetManagtComp { AMCName = amc_name } };
        //                        mf.NetAssetValues = nav_values;

        //                        _ctx.AddRange(mf);
        //                        var dbAffected = await _ctx.SaveChangesAsync();

        //                    }


        //                    nav_values = new List<NetAssetValue>();
        //                    var countDash = item.SchemeName.Count(c => c == '-');


        //                    if (countDash == 1)
        //                    {
        //                        amc_name = item.SchemeName.GetBeforeToStart();
        //                        mf_name = item.SchemeName;
        //                    }
        //                    else if (countDash == 2)
        //                    {
        //                        amc_name = item.SchemeName.GetBeforeToStart();
        //                        mf_name = item.SchemeName.GetAfterToEnd();
        //                    }
        //                }
        //                else
        //                {
        //                    var nav_value = new NetAssetValue();
        //                    nav_value.Price = item.SalePrice.ToDouble();
        //                    nav_value.OnDate = item.Date.ToString().ToDate();

        //                    nav_values.Add(nav_value);
        //                }
        //                ////var nav_values = new List<NetAssetValue>();
        //                //var nav_value1 = new NetAssetValue();
        //                //var nav_value2 = new NetAssetValue();

        //                //nav_value1.Price = 123; nav_value1.OnDate = DateTime.Today.AddDays(-2);
        //                //nav_value2.Price = 465; nav_value1.OnDate = DateTime.Today.AddDays(-4);
        //                //nav_values.Add(nav_value1);
        //                //nav_values.Add(nav_value2);


        //                //// var mf = new MutualFund() { MutualFundName = "Div", AMC = new AssetManagtComp { AMCName = "DK" }, NetAssetValues = new List<NetAssetValue> { new NetAssetValue { Price = 1.2, OnDate = DateTime.Today.AddDays(-5) }, new NetAssetValue { Price = 2.4, OnDate = DateTime.Today.AddDays(-6) } } };
        //                //var mf1 = new MutualFund() { MutualFundName = "Div", AMC = new AssetManagtComp { AMCName = "DK" } };
        //                //mf1.NetAssetValues = nav_values;

        //                //_ctx.AddRange(mf1);
        //                //var ii = await _ctx.SaveChangesAsync();
        //                ////_ctx.mutualFunds.Add(mf);

        //            }
        //            else
        //            {
        //                //if (_ctx.assetManagtComps.Where(e => e.AMCName == item.SchemeCode).Count() == 0)
        //                //{
        //                //    var newAMC = new AssetManagtComp();
        //                //    newAMC.AMCName = item.SchemeCode;

        //                //    _ctx.assetManagtComps.Add(newAMC);
        //                //}
        //            }
        //        }
        //        var i = await _ctx.SaveChangesAsync();
        //        vReturm = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        System.Diagnostics.Debug.WriteLine(ex.Message);
        //        throw;
        //    }

        //    return vReturm;
        //}
    }
}
