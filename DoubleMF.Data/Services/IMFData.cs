using DoubleMF.Download;
using DoubleMF.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoubleMF.Data.Services
{
    public interface IMFData
    {
        Task<bool> SaveDownloadedData(List<AMFDataModel> downloaded_data);
        Task<List<MFSchemeDTO>> GetMFSchemeAsync();
        //Task<List<MFDetailDTO>> GetMFSchemeWithoutAMCAsync();
        Task<DoubleMFResponse> GetMFSchemeDetailsAsync(int PageIndex = 0, int PageSize = 10, string StartsWith = "", bool IsAMCIncluded = false);
        Task<DoubleMFResponse> GetMFSchemeDetailsAsync(int PageIndex = 0, int PageSize = 10, string AMCStartsWith = "");
    }
}
