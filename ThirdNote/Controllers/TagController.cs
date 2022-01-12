using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThirdNote.Models;
using System.Data.Entity.Migrations;
using System.Data.Entity;

namespace ThirdNote.Controllers
{
    public class TagController : Controller
    {
        private static readonly NotebookDbContext db = new NotebookDbContext();

        // GET: Tag
        public ActionResult Index(string sort, string columns, string lang)
        {
            // delete tags with no reference in noteTag     
            var OrphanTags = db.Tags.Where(t => t.NoteTags.Count == 0);
            foreach (var item in OrphanTags)
                db.Tags.Remove(item);
            db.SaveChanges();

            switch (sort)
            {
                case "alpha":
                    return View(db.Tags.OrderBy(tag => "en".Equals(lang)?tag.Lable_en:tag.Lable_fa).ToList());
                case "num":
                    return View(db.Tags.OrderByDescending(tag => tag.NoteTags.Count)
                        .ThenBy(tag => "en".Equals(lang)?tag.Lable_en:tag.Lable_fa).ToList());
                case "group":
                    return View(db.Tags.OrderBy(tag => ("en".Equals(lang) ? tag.Lable_en : tag.Lable_fa))
                        .ThenBy(tag => "en".Equals(lang) ? tag.Lable_en : tag.Lable_fa).ToList());
                default:
                    return View(db.Tags.OrderBy(tag => ("en".Equals(lang) ? tag.Lable_en : tag.Lable_fa))
                        .ThenBy(tag => "en".Equals(lang) ? tag.Lable_en : tag.Lable_fa).ToList());
            }

            if (String.IsNullOrEmpty(sort) || sort.Equals("alpha"))
            {
                return View(db.Tags.OrderBy(tag => lang.Equals("en")?tag.Lable_en:tag.Lable_fa).ToList());
            } else if (sort.Equals("num"))
            {
                return View(db.Tags.OrderByDescending(tag=>tag.NoteTags.Count)
                    .ThenBy(tag => lang.Equals("en") ? tag.Lable_en : tag.Lable_fa).ToList());
            }
            return View(db.Tags.OrderBy(tag => lang.Equals("en") ? tag.Lable_en : tag.Lable_fa).ToList());
        }

        // GET: Tag/Details/5
        public ActionResult Details(int id)
        {
            var data = db.NoteTags
                .Where( nt => nt.TagID == id)
                .Join(db.Notes,
                    nt => nt.NoteID,
                    n => n.Id,
                    (nt, n) => n
                ).ToList();
            var tag = db.Tags.Where(t => t.ID == id).First();
            ViewBag.Tag = tag.Label;
            ViewBag.TagId = tag.ID;
            return View(data);
        }

        // GET: Tag/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Tag/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Tag/Edit/5
        public ActionResult Edit(int id)
        {
            Tag tag = db.Tags.Find(id);
            return View(tag);
        }

        // POST: Tag/Edit/5
        [HttpPost]
        public ActionResult Edit([Bind(Include = "ID,Label,Lable_en,Lable_fa")] Tag tag, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                tag.Label = formCollection["Label"].ToString();
                tag.Lable_en = formCollection["Lable_en"].ToString();
                tag.Lable_fa = formCollection["Lable_fa"].ToString();
                //db.Tags.AsNoTracking().Where(t => t.ID == tag.ID);
                //db.Entry(tag).State = System.Data.Entity.EntityState.Modified;
                db.Set<Tag>().AddOrUpdate(tag);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View();
        }

        // GET: Tag/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Tag/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        public ActionResult Search()
        {
            var tagLabels = db.Tags.Select(t => t.Label).ToList();
            return Json(tagLabels, JsonRequestBehavior.AllowGet);
        }  
    }
}
