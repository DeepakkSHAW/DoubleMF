using System;
using System.Collections.Generic;
using System.Text;

namespace DoubleMF.Model
{
    public class DoubleMFResponse
    {
        public bool Success { get; set; }
        public Result Result { get; set; }
    }
    public class Result
    {
        public int ActualCount { get; set; }
        public IList<object> Data { get; set; }
    }

    public class AMCDTO
    {
        public string AMCName { get; set; }
    }
    public class MFDetailDTO
    {
        public string MutualFundName { get; set; }
        public int MutualFundCode { get; set; }
    }
    public class MFSchemeDTO
    {
        public string AMCName { get; set; }
        public List<MFDetailDTO> MFDetailDTOs { get; set; }
    }


}
