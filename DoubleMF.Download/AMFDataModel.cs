using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleMF.Download
{
    public class AMFDataModel
    {
        public string SchemeCode { get; set; }
        public string SchemeName { get; set; }
        public string ISIN { get; set; }
        public string ISINDivReinvestment { get; set; }
        public double? NetAssetValue { get; set; }
        public string RepurchasePrice { get; set; }
        public string SalePrice { get; set; }
        public DateTime? Date { get; set; }
    }
    public class AMFDataMap : ClassMap<AMFDataModel>
    {
        public AMFDataMap()
        {
            Map(m => m.SchemeCode).Name("Scheme Code");
            Map(m => m.SchemeName).Name("Scheme Name");
            Map(m => m.ISIN).Name("ISIN Div Payout/ISIN Growth");
            Map(m => m.ISINDivReinvestment).Name("ISIN Div Reinvestment");
            Map(m => m.NetAssetValue).Name("Net Asset Value");
            Map(m => m.RepurchasePrice).Name("Repurchase Price");
            Map(m => m.SalePrice).Name("Sale Price");
            Map(m => m.Date).Name("Date");
        }
    }
}
