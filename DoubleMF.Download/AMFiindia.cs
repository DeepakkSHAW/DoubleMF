using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DoubleMF.Download
{
    public class AMFiindia
    {

        private const string _amfURL = "http://portal.amfiindia.com/DownloadNAVHistoryReport_Po.aspx?frmdt=06-Apr-2020";
        private string _responseBody = null;

        public async Task<bool> NAVDataDownloadAsync(DateTime dt)
        {
            bool vReturn = false;
           
            var amrUriBuilder = new UriBuilder(_amfURL);
            var query = HttpUtility.ParseQueryString(amrUriBuilder.Query);
            query["frmdt"] = dt.AddDays(0).ToString("dd-MMM-yyyy"); //"06-Apr-2020";

            amrUriBuilder.Query = query.ToString();
            var amfUri = amrUriBuilder.ToString();

            try
            {
                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage response = await client.GetAsync(amrUriBuilder.Uri, HttpCompletionOption.ResponseContentRead))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            _responseBody = await response.Content.ReadAsStringAsync();
                            if (_responseBody.Contains("No data found"))
                            {
                                _responseBody = null;
                                vReturn = false;
                            }
                            else
                            {
                                vReturn = true;
                            }
                        }
                        else
                            _responseBody = null;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
            return vReturn;
        }

        public async Task<List<AMFDataModel>> GetDownloadedDataAsync()
        {
             //_responseBody = File.ReadAllText(@"..\..\..\..\Data\MF Analysis\MF.txt");
            var records = new List<AMFDataModel>();

            if (string.IsNullOrEmpty(_responseBody)) return records;

            try
            {
                using (var reader = new StringReader(_responseBody))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Configuration.RegisterClassMap<AMFDataMap>();
                    csv.Configuration.Delimiter = ";";
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.IgnoreBlankLines = true;
                    csv.Configuration.TrimOptions = TrimOptions.Trim;
                    csv.Configuration.MissingFieldFound = null;
                    //csv.Configuration.PrepareHeaderForMatch = (string header, int index) => header.ToLower();

                    records = csv.GetRecords<AMFDataModel>().ToList();

                    Debug.WriteLine(records.Count());
                    //foreach (var item in records)
                    //{
                    //    Debug.WriteLine($"{item.SchemeCode} ### {item.SchemeName} ### {item.Date}");
                    //}
                    //return records;
                }

                ///*Write to new CSV File*/
                //using (var writer = new StreamWriter(@"c:\Temp\MFFinal.csv"))
                //using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                //{
                //    csv.WriteRecords(records);
                //}
                return records;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

    }
}

