using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AT.Repository.Model;

namespace AT.WebApp.Controllers
{
    public class LinkController : BaseController
    {
        private DbModel db = new DbModel();

        // GET: /Link/
        public ActionResult Index()
        {
            return View(db.TextToReadHyperlinks.ToList());
        }

        // GET: /Link/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextToReadHyperlink texttoreadhyperlink = db.TextToReadHyperlinks.Find(id);
            if (texttoreadhyperlink == null)
            {
                return HttpNotFound();
            }
            return View(texttoreadhyperlink);
        }

        // GET: /Link/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Link/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,HyperlinkText,LanguageCode")] TextToReadHyperlink texttoreadhyperlink,FormCollection form)
        {
            if (ModelState.IsValid)
            {
                string code = form["LanguageDropDownList"].ToString();
                texttoreadhyperlink.LanguageCode = int.Parse(code);
                db.TextToReadHyperlinks.Add(texttoreadhyperlink);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(texttoreadhyperlink);
        }

        // GET: /Link/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextToReadHyperlink texttoreadhyperlink = db.TextToReadHyperlinks.Find(id);
            if (texttoreadhyperlink == null)
            {
                return HttpNotFound();
            }
            return View(texttoreadhyperlink);
        }

        // POST: /Link/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,HyperlinkText,LanguageCode")] TextToReadHyperlink texttoreadhyperlink)
        {
            if (ModelState.IsValid)
            {
                db.Entry(texttoreadhyperlink).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(texttoreadhyperlink);
        }

        // GET: /Link/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TextToReadHyperlink texttoreadhyperlink = db.TextToReadHyperlinks.Find(id);
            if (texttoreadhyperlink == null)
            {
                return HttpNotFound();
            }
            return View(texttoreadhyperlink);
        }

        // POST: /Link/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TextToReadHyperlink texttoreadhyperlink = db.TextToReadHyperlinks.Find(id);
            db.TextToReadHyperlinks.Remove(texttoreadhyperlink);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
