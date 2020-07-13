using DoubleMF.Download;
using DoubleMF.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoubleMF.Data.Services
{
    public interface IAMCData
    {
        Task<AssetManagtComp> GetAMCAsync(int AMCID);
        Task<IEnumerable<AssetManagtComp>> GetAMCsAsync();
        Task<bool> SaveDownloadedAMCData(List<AMFDataModel> downloaded_data);
    }
}