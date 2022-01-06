using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThirdNote.Models;

namespace ThirdNote.Controllers
{
    public class RelapseController : Controller
    {
        private NotebookDbContext db = new NotebookDbContext();

        // GET: Relapses
        public ActionResult Index()
        {
            return View(db.Relapses.ToList());
        }

        // GET: Relapses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relapse relapse = db.Relapses.Find(id);
            if (relapse == null)
            {
                return HttpNotFound();
            }
            return View(relapse);
        }

        // GET: Relapses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Relapses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Subject,Date,Message")] Relapse relapse, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                if (relapse.Message == null)
                {
                    relapse.Message = "";
                }
                if (DateTime.TryParse(formCollection["MyDate"], out DateTime dt))
                {
                    relapse.Date = DateTime.Parse(formCollection["MyDate"]);
                }
                else
                {
                    relapse.Date = DateTime.Now;
                }
                

                db.Relapses.Add(relapse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(relapse);
        }

        // GET: Relapses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relapse relapse = db.Relapses.Find(id);
            if (relapse == null)
            {
                return HttpNotFound();
            }
            return View(relapse);
        }

        // POST: Relapses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Subject,Date,Message")] Relapse relapse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relapse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(relapse);
        }

        // GET: Relapses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Relapse relapse = db.Relapses.Find(id);
            if (relapse == null)
            {
                return HttpNotFound();
            }
            return View(relapse);
        }

        // POST: Relapses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Relapse relapse = db.Relapses.Find(id);
            db.Relapses.Remove(relapse);
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
