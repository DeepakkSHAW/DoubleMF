using DoubleMF.Download;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoubleMF.Data.Services
{
    public interface IMFData
    {
        Task<bool> SaveDownloadedData(List<AMFDataModel> downloaded_data);
    }
}
