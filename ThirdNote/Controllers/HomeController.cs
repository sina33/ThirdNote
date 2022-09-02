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

        public ActionResult Index()
        {
            Dashboard indexViewData = new Dashboard()
            {
                Pinned = db.Notes.Where(n => n.Pin && !n.Hidden).ToList(),
                Sorted = db.Notes.Where(p=>!p.Hidden).OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).ToList()
            };
            if (db.Relapses.Count() > 0)
            {
                var dates = db.Relapses.OrderByDescending(r => r.Date).Select(r => r.Date).ToList();
                var recentRelapseDate = DateTime.Now;
                List<int> Strikes = new List<int>();
                foreach (var item in dates)
                {
                    int offset = (recentRelapseDate - item).Days;
                    Strikes.Add(offset);
                    recentRelapseDate = item;
                }
                //Strikes.Reverse();
                indexViewData.Strikes = Strikes;
                //indexViewData.LastStrike = (DateTime.Now - db.Relapses.OrderByDescending(r => r.Date).First().Date).Days;
                System.Diagnostics.Debug.WriteLine(dates);
                System.Diagnostics.Debug.WriteLine(Strikes);
            }
            else
            {
                indexViewData.Strikes = new List<int> { 0 };
            }
            //return View(db.Notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).ToList());
            return View(indexViewData);
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