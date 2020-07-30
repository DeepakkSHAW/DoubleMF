using DoubleMF.Download;
using DoubleMF.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DoubleMF.Data.Services
{
    public interface IAMCData
    {
        Task<IEnumerable<AMCDTO>> GetAllAMCAsync();
        Task<IEnumerable<AssetManagtComp>> GetAMCsAsync();
        Task<AssetManagtComp> GetAMCAsync(int AMCID);
        Task<bool> SaveDownloadedAMCData(List<AMFDataModel> downloaded_data);
    }
}