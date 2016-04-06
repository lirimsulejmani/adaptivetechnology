using AT.WebApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AT.Service.Implementations;
using AT.Service.Interfaces;
using AT.Repository.Model;

namespace AT.WebApp.Controllers
{
    public class AdminController : BaseController
    {
        private AdaptationService service = new AdaptationService();

        public AdminController()
        {
        }

        // GET: Adaptation
        public ViewResult Index()
        {
            var model = new AdminViewModel {
                AdaptationTechniques =  service.ListTechniques()
                };

            return View(model.AdaptationTechniques);
        }

        [HttpPost]
        public ViewResult ApplyAdaptationTechniques(IEnumerable<AdaptationTechnique> model)
        {
            if (model.Count()>0)
            {
                var values = new Dictionary<int, bool>();
                foreach (var item in model)
                {
                    values.Add(item.Id, item.ApplyStatus);
                }
                service.ChangesTechniquesToApply(values);
                Success("Applied successfully.", true);
                return View("Index",model);
            }
            else
            {
                Danger("Looks like something went wrong. Please check your form.");
                return View("Index",model);
            }

        }
    }
}