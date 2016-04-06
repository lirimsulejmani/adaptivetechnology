using AT.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AT.Repository.Model;
using AT.Repository.Enums;

namespace AT.Service.Implementations
{
    public sealed class LinkEnrichmentTechniqueService : ILinkEnrichmentTechnique
    {
        private readonly DbModel _db = new DbModel();
        public List<TextToReadHyperlink> ListTextToRead()
        {
            return _db.TextToReadHyperlinks.ToList();
        }

        public int Add(TextToReadHyperlink entity)
        {
            _db.TextToReadHyperlinks.Add(entity);
            return entity.Id;
        }

        public int Modify(TextToReadHyperlink entity)
        {
           _db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
           _db.SaveChanges();
           return entity.Id;
        }

        public void Delete(TextToReadHyperlink entity)
        {
            _db.TextToReadHyperlinks.Remove(entity);
            _db.SaveChanges();
        }
    }
}
