using DoubleMF.Download;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DoubleMF.Data.Services
{
    public interface INAVData
    {
        Task<bool> SaveDownloadeNAVdData(List<AMFDataModel> downbloaded_data);
    }
}
