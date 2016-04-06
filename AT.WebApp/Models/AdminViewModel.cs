using AT.Repository.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AT.WebApp.Models
{
    public class AdminViewModel
    {
        public AdminViewModel()
        {
            AdaptationTechniques = new List<AdaptationTechnique>();
        }
        public List<AdaptationTechnique> AdaptationTechniques { get; set; }
        public int Id { get; set; }
        public bool ApplyStatus { get; set; }
        public string EnrichmentTechique { get; set; }
    }
}
