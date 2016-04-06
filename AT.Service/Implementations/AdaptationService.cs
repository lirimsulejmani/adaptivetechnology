using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT.Service.Interfaces;
using AT.Repository.Model;

namespace AT.Service.Implementations
{
    public class AdaptationService : IAdaptationService
    {
        private readonly DbModel _db = new DbModel();
        public List<AdaptationTechnique> ListTechniques()
        {
          var list =  _db.AdaptationTechniques.ToList();
          return list;
        }
        public void ChangesTechniquesToApply(Dictionary<int, bool> values)
        {
            var techniques = _db.AdaptationTechniques.ToList();
            foreach (var item in values)
            {
                var theTechnique = techniques.FirstOrDefault(t => t.Id == item.Key);
                if (theTechnique != null)
                {
                    theTechnique.ApplyStatus = item.Value;
                }
            }
            _db.SaveChanges();
        }

        public string[] AppliedTechniques()
        {
            return _db.AdaptationTechniques.Where(t=>t.ApplyStatus == true).Select(t=>t.EnrichmentTechique).ToArray();          
        }

        public string[] ListTextToRead()
        {
            return _db.TextToReadHyperlinks.Select(t=>t.HyperlinkText).ToArray();
        }
    }
}
