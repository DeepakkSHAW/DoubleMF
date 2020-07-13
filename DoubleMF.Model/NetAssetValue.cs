using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoubleMF.Model
{
    [Table("T_NetAssetValue")]
    public class NetAssetValue
    {
        [Required]
        public int NetAssetValueId { get; set; }

        [Required]
        public double Price { get; set; }

        [Required]
        public DateTime OnDate { get; set; }
        [Required]
        public MutualFund MF { get; set; }


    }
}
