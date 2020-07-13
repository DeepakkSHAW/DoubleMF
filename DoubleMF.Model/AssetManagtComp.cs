using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DoubleMF.Model
{
    [Table("T_AssetManagtCO")]
    public class AssetManagtComp
    {
        [Required]
        public int AMCId { get; set; }

        [Required(ErrorMessage = "AMC Title required")]
        [StringLength(100, ErrorMessage = "AMC Name too long (100 char).")]
        public string AMCName { get; set; }

        public IEnumerable<MutualFund> MutualFunds { get; set; }

        [Column("inDate")]
        public DateTime? InDate { get; set; } = DateTime.Today;

    }
}
