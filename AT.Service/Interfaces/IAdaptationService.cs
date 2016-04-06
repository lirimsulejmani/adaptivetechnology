using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using AT.Repository.Model;

namespace AT.Service.Interfaces
{
    public interface IAdaptationService
    {
        List<AdaptationTechnique> ListTechniques();
        void ChangesTechniquesToApply(Dictionary<int, bool> techniques);
    }
}
