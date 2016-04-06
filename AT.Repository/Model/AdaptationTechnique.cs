namespace AT.Repository.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AdaptationTechniques")]
    public partial class AdaptationTechnique
    {
            public int Id { get; set; }

            [Required]
            [StringLength(50)]
            [Display(Name = "EnrichmentTechique")]
            public string EnrichmentTechique { get; set; }

            public bool ApplyStatus { get; set; }
    }
}
