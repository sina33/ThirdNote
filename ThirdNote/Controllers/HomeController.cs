using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ThirdNote.ViewModels;
using ThirdNote.Models;

namespace ThirdNote.Controllers
{
    public class HomeController : Controller
    {

        private NotebookDbContext db = new NotebookDbContext();

        public ActionResult Index(string sortOrder)
        {
            Home HomeViewData = new Home()
            {
                Private = db.Notes.Where(n => !n.Hidden).OrderByDescending(n => n.WrittenDate),
                Sorted = db.Notes.Where(n => !n.Hidden).OrderByDescending(n => n.WrittenDate)
                //Sorted = db.Notes.Where(n => !n.Hidden)
            };
            var notes = db.Notes.Where(n => !n.Hidden);

            switch (sortOrder)
            {
                case "name":
                    notes = notes.OrderBy(n => n.Name).ThenByDescending(n => n.WrittenDate);
                    break;
                case "pin":
                    notes = notes.OrderByDescending(n => n.Pin).ThenByDescending(n => n.WrittenDate);
                    break;

                case "date":
                    notes = notes.OrderBy(n => n.WrittenDate);
                    break;
                case "date_desc":
                    notes = notes.OrderByDescending(n => n.WrittenDate);
                    break;
                case "likes":
                    notes = notes.OrderByDescending(n => n.ViewCount).ThenByDescending(n => n.WrittenDate);
                    break;
                case "markdown":
                    notes = notes.OrderByDescending(n => n.Markdown).ThenByDescending(n => n.WrittenDate);
                    break;
                default:
                    notes = notes.OrderByDescending(n => n.WrittenDate);
                    break;
            }
            HomeViewData.Sorted = notes;
            return View(HomeViewData);

            //return RedirectToAction("Index", "Note");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}