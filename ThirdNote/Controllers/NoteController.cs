using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ThirdNote.Models;
using ThirdNote.ViewModels;
using Markdig;
using Humanizer;
using System.Globalization;
using System.Text.RegularExpressions;

namespace ThirdNote.Controllers
{
    [OutputCacheAttribute(VaryByParam = "*", Duration = 0, NoStore = true)]
    public class NoteController : Controller
    {
        public static int ACCORDION_NOTE_ID = 4113;
        public static int REF_TAG_ID = 1049;
        private readonly NotebookDbContext db = new NotebookDbContext();
        private readonly MarkdownPipeline pipeline = App_Start.MarkdownConfig.GetPipeline();

        // GET: Note
        public ActionResult Index(string sortOrder)
        {
            Home NoteIndexViewData = new Home()
            {
                Private = db.Notes.OrderByDescending(n => n.WrittenDate),
                Sorted =  db.Notes.Where(n => !n.Hidden).OrderByDescending(n => n.WrittenDate)
                //Sorted = db.Notes.Where(n => !n.Hidden)
            };
            if (db.Relapses.Count() > 0)
            {
                var dates = db.Relapses.OrderByDescending(r => r.Date).Select(r=>r.Date).ToList();
                var recentRelapseDate = DateTime.Now;
                List<int> Strikes = new List<int>();
                foreach (var item in dates)
                {
                    int offset = (recentRelapseDate - item).Days;
                    Strikes.Add(offset);
                    recentRelapseDate = item;
                }
                //Strikes.Reverse();
                NoteIndexViewData.Strikes = Strikes;
                //indexViewData.LastStrike = (DateTime.Now - db.Relapses.OrderByDescending(r => r.Date).First().Date).Days;
                //System.Diagnostics.Debug.WriteLine(dates);
                //System.Diagnostics.Debug.WriteLine(Strikes);
            }
            else
            {
                NoteIndexViewData.Strikes = new List<int> { 0 };
            }
            //return View(db.Notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).ToList());
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
                    notes = notes.OrderBy(n => n.WrittenDate).ThenBy(n => n.Id);
                    break;
                case "date_desc":
                    notes = notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id);
                    break;
                case "likes":
                    notes = notes.OrderByDescending(n => n.ViewCount).ThenByDescending(n => n.WrittenDate);
                    break;
                case "private":
                    notes = db.Notes.OrderByDescending(n => n.Hidden).ThenByDescending(n => n.WrittenDate);
                    break;
                case "markdown":
                    notes = notes.OrderByDescending(n => n.Markdown).ThenByDescending(n => n.WrittenDate);
                    break;
                default:
                    notes = notes.OrderByDescending(n => n.WrittenDate);
                    break;
            }
            NoteIndexViewData.Sorted = notes;
            return View(NoteIndexViewData);
        }

        // GET: Note/List
        public ActionResult List(string sortOrder) 
        {
            Home indexViewData = new Home()
            {
                Private = db.Notes.OrderByDescending(n => n.Hidden).OrderByDescending(n=>n.WrittenDate),
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
                    notes = notes.OrderBy(n => n.WrittenDate).ThenBy(n => n.Id);
                    break;
                case "date_desc":
                    notes = notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id);
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
            return View(notes.ToList());
            //return View(db.Notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).Where(p => !p.Hidden).ToList());
            //return View(db.Notes.OrderByDescending(n => n.ViewCount).ThenByDescending(n => n.WrittenDate));
        }

        // GET: Note/Details/5
        public ActionResult Details(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Where(n=>n.Name==name).SingleOrDefault();
            if (note == null)
            {
                return HttpNotFound(name + " not found");
            }
            // Like
            {
                note.ViewCount += 1;
                db.SaveChanges();
            }
            ViewBag.Tags = db.NoteTags.Where(nt => nt.NoteID == note.Id)
                .Join(db.Tags, 
                nt => nt.TagID, 
                t => t.ID, 
                (nt, t) => t);

            // stitching notes
            string snpattern = @"\bsn#\w{8}\b";
            note.Text = Regex.Replace(note.Text, snpattern, delegate (Match match)
            {
                string nName = (match.Value.Split('#').Last());
                Note x = db.Notes.Where(n => n.Name == nName).SingleOrDefault();
                if(x != null)
                {
                    string nText = x.Text;//String.Format("{0}[n#{1}]", db.Notes.Find(nId).Text, nId);
                    return nText + "n#" + nName;
                }
                return nName;
                //return (db.Notes.Find(nId).Markdown && !note.Markdown ? Markdown.ToHtml(note.Text, pipeline) : nText);
            });
            if (note.Markdown)
            {
                var result = Markdown.ToHtml(note.Text, pipeline);
                note.Text = result;
            } else
            {
                note.Text = HttpUtility.HtmlEncode(note.Text);
            }


            string npattern = @"\b(?<=n#)\w{8}\b";
            HashSet<Note> PNotes = new HashSet<Note>();
            Match mc = Regex.Match(note.Text, npattern, RegexOptions.IgnoreCase);
            while (mc.Success)
            {
                Note x = db.Notes.Where(n => n.Name == mc.Value).SingleOrDefault();
                if (x != null)
                {
                    PNotes.Add(x);
                }
                mc = mc.NextMatch();
            }
            ViewBag.PNotes = PNotes;

            string idpattern = @"\b(?<=n#)" + note.Name + @"\b";
            HashSet<Note> CNotes = new HashSet<Note>();
            // CNotes (Child Notes) are referring to this note
            foreach (Note no in db.Notes)
            {
                Match match = Regex.Match(no.Text, idpattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    CNotes.Add(no);
                }
            }
            CNotes.RemoveWhere(n => n.Id == note.Id);  // remove self-citation
            ViewBag.CNotes = CNotes.ToArray();

            // Convert each reference to badge link
            string npatternFull = @"\bn#\w{8}\b";
            note.Text = Regex.Replace(note.Text, npatternFull, delegate (Match match)
            {
                string nName = match.Value.Split('#').Last();
                Note x = db.Notes.Where(n => n.Name == nName).SingleOrDefault();
                if(x != null)
                {
                    string nTitle = x.Title;
                    string sub = "<a id='badge-link bg-gradient' href='/Note/Details/" + nName + "'><span class='badge text-black shadow-sm'><i class='fa fa-hashtag'></i>" + nName + "</span></a>";
                    //string sub = string.Format("<button type='button' class='badge bg-info' data-bs-toggle='tooltip' data-bs-placement='bottom' title='{1}'><a href='/Note/Details/{0}' class=''>#{2}</a></ button >", nName, nTitle, nName);
                    return sub;
                }
                return match.Value;
            });
            //ViewBag.TimeAgo = note.WrittenDate.Humanize(false,null,new CultureInfo("fa"));
            ViewBag.TimeAgo = note.WrittenDate.Humanize(false);
            return View(note);
        }

        // GET: Note/Create
        [HttpGet]
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Note/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Name,Title,Text,CreatedDate,WrittenDate,Markdown,Hidden,Pin")] Note note, FormCollection formCollection)
        {            
            if (ModelState.IsValid)
            {
                note.Text = string.IsNullOrEmpty(note.Text) ? "" : note.Text;

                if (DateTime.TryParse(formCollection["MyDate"], out DateTime dt))
                {
                    note.WrittenDate = (formCollection["MyTime"] == null) ? DateTime.Parse(formCollection["MyDate"]) 
                        : DateTime.Parse(formCollection["MyDate"] + " " + formCollection["MyTime"]);
                }
                else
                {
                    note.WrittenDate = note.CreatedDate;
                }
                db.Notes.Add(note);
                char[] delimiters = new char[] { ',', '،' };
                foreach (var tagLabel in formCollection["tagsInput"].Split(delimiters).Select(t => t.Trim()))
                {
                    if (string.IsNullOrEmpty(tagLabel)) continue;
                    
                    // this tagLabel Exists in Tags Table. Just add it's NoteTags record.
                    if (db.Tags.Any(x => x.Lable_en.Equals(tagLabel, StringComparison.OrdinalIgnoreCase) || x.Lable_fa.Equals(tagLabel) ))
                    {
                        Tag tag = db.Tags.Single(t => t.Lable_en.Equals(tagLabel, StringComparison.OrdinalIgnoreCase) || t.Lable_fa.Equals(tagLabel) );
                        if(db.NoteTags.Count(nt => nt.NoteID == note.Id && nt.TagID == tag.ID) == 0)
                        {
                            db.NoteTags.Add(new NoteTag(note, tag));
                        }
                    }
                    else // Create Tag record. Insert both Tag & NoteTag records to their tables.
                    {
                        Tag tag = new Tag
                        {
                            Label = tagLabel
                        };
                        db.Tags.Add(tag);
                        db.NoteTags.Add(new NoteTag(note, tag));
                    }
                }
                db.SaveChanges();
            }

            return RedirectToAction("Details", new { name = note.Name });
        }

        // GET: Note/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Where(n => n.Name.Equals(name)).First();
            if (note == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound); //HttpNotFound();
            }
            var tagLabels = db.NoteTags
                .Where(nt => nt.NoteID == note.Id)
                .Join(db.Tags,
                    nt => nt.TagID,
                    t => t.ID,
                    (nt, t) => t.Lable_fa
                ).ToList() ;
            ViewBag.tagString = String.Join(",", tagLabels);

            return View(note);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Name,Title,Text,CreatedDate,WrittenDate,Markdown,Hidden,Pin,ViewCount")] Note note, FormCollection formCollection)
        {
            if (ModelState.IsValid)
            {
                //db.NoteTags.RemoveRange(db.NoteTags.Where(nt => nt.NoteID == note.Id));
                char[] delimiters = new char[] { ',', '،' };
                var inputTagLabels = formCollection["tagsInput"].Split(delimiters).Select(t => t.Trim()).Where(w => !w.Equals(String.Empty));
                var existingNoteTags = db.NoteTags.Include(nt => nt.Tag).Where(nt => nt.NoteID == note.Id);

                //remove omitted note-tags
                foreach (var omittedNT in existingNoteTags.Where(eNT => !inputTagLabels.Any(tl => eNT.Tag.Lable_en.Equals(tl, StringComparison.OrdinalIgnoreCase) || eNT.Tag.Lable_fa.Equals(tl))))
                    db.NoteTags.Remove(omittedNT);
                // select inputTagLabels not included in existingNoteTags
                foreach (var newTagLabel in inputTagLabels.Where(t => !existingNoteTags.Any(eNT => eNT.Tag.Lable_en.Equals(t, StringComparison.OrdinalIgnoreCase) || eNT.Tag.Lable_fa.Equals(t))))
                {
                    if (!db.Tags.Any(t => t.Lable_en.Equals(newTagLabel, StringComparison.OrdinalIgnoreCase) || t.Lable_fa.Equals(newTagLabel)))
                    {   // Create both Tag and NoteTag
                        Tag tag = db.Tags.Add(new Tag { Label = newTagLabel });
                        db.NoteTags.Add(new NoteTag(note, tag));
                    }
                    else
                    {   // Only add to NoteTags
                        Tag tag = db.Tags.First(t => t.Lable_en.Equals(newTagLabel, StringComparison.OrdinalIgnoreCase) || t.Lable_fa.Equals(newTagLabel, StringComparison.OrdinalIgnoreCase));
                        db.NoteTags.Add(new NoteTag(note, tag));
                        //tag.Label = tagLabel;   // update case format
                                                //db.Entry(tag).State = EntityState.Modified;
                    }
                }

                note.Text = string.IsNullOrEmpty(note.Text) ? "" : note.Text;
                //if (note.WrittenDate == null)
                //{
                //    note.WrittenDate = note.CreatedDate;
                //}
                if (DateTime.TryParse(formCollection["MyDate"], out DateTime dt))
                {
                    if (formCollection["MyTime"] == null)
                    {
                        note.WrittenDate = DateTime.Parse(formCollection["MyDate"]);
                    }
                    else
                    {
                        note.WrittenDate = DateTime.Parse(formCollection["MyDate"] + " " + formCollection["MyTime"]);
                    }
                }
                else
                {
                    note.WrittenDate = note.CreatedDate;
                }
                db.Entry(note).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Details", new { name = note.Name });

        }

        // GET: Note/Delete/5
        public ActionResult Delete(string name)
        {
            if (name == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Where(n => n.Name.Equals(name)).First();
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string name)
        {
            Note note = db.Notes.Where(n=>n.Name.Equals(name)).First();
            db.Notes.Remove(note);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: Note/Timeline
        public ActionResult Timeline()
        {

            //return View(db.Notes.OrderByDescending(n => n.WrittenDate).ToList());

            List<Note> notes = db.Notes.OrderByDescending(n => n.WrittenDate).Where(no => no.Hidden != true).ToList();
            foreach (Note note in notes)
            {
                if (note.Markdown)
                {
                    notes.SingleOrDefault(n => n.Id == note.Id).Text = Markdig.Markdown.ToHtml(note.Text, pipeline);
                }
            }
            return View(notes);
        }

        public RedirectToRouteResult Show(string name)
        {
            Note note = db.Notes.Where(n => n.Name.Equals(name)).First();
            return RedirectToAction("Details", new { note.Name });
        }

    }
}
