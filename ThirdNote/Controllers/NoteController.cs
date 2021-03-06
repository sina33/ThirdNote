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
        public ActionResult Index()
        {
            Dashboard indexViewData = new Dashboard()
            {
                Pinned = db.Notes.Where(n => n.Pin).ToList(),
                Sorted = db.Notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).ToList()
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

        // GET: Note/List
        public ActionResult List() 
        {
            return View(db.Notes.OrderByDescending(n => n.WrittenDate).ThenByDescending(n => n.Id).ToList());
        }

        // GET: Note/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            ViewBag.Tags = db.NoteTags.Where(nt => nt.NoteID == note.Id)
                .Join(db.Tags, 
                nt => nt.TagID, 
                t => t.ID, 
                (nt, t) => t);

            // stitching notes
            string snpattern = @"\bsn#\d+\b";
            note.Text = Regex.Replace(note.Text, snpattern, delegate (Match match)
            {
                int nId = Convert.ToInt32(match.ToString().Split('#')[1]);
                string nText = db.Notes.Find(nId).Text;//String.Format("{0}[n#{1}]", db.Notes.Find(nId).Text, nId);
                return nText+"n#"+nId;
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


            string npattern = @"\b(?<=n#)\d+\b"; ;
            HashSet<Note> PNotes = new HashSet<Note>();
            // this note has reference tag, PNotes (Parent Notes) are referred to, in this note
            if (db.NoteTags.Any(nt => nt.NoteID == note.Id /*&& nt.TagID == REF_TAG_ID*/))
            {
                Match mc = Regex.Match(note.Text, npattern, RegexOptions.IgnoreCase);
                while (mc.Success)
                {
                    PNotes.Add(db.Notes.Find(Convert.ToInt32(mc.Value)));
                    mc = mc.NextMatch();
                }
            }
            ViewBag.PNotes = PNotes;

            string idpattern = @"\b(?<=n#)" + id + @"\b";
            HashSet<Note> CNotes = new HashSet<Note>();
            // CNotes (Child Notes) are referring to this note
            foreach (Note no in db.Notes)
            {
                //bool isRef = db.NoteTags.Any(nt=>nt.TagID==REF_TAG_ID && nt.NoteID==note.Id) ? true : false;
                Match match = Regex.Match(no.Text, idpattern, RegexOptions.IgnoreCase);
                if (match.Success /*&& isRef*/)
                {
                    CNotes.Add(db.Notes.Find(no.Id));
                }
            }
            CNotes.RemoveWhere(n => n.Id == note.Id);  // remove self-citation
            ViewBag.CNotes = CNotes.ToArray();

            // Convert each reference to badge link
            string npatternFull = @"\bn#\d+\b";
            note.Text = Regex.Replace(note.Text, npatternFull, delegate (Match match)
            {
                int nId = Convert.ToInt32(match.ToString().Split('#')[1]);
                string nTitle = db.Notes.Find(nId).Title;
                string sub = "<a href='/Note/Details/" + nId.ToString() + "'><span class='badge bg-info'>#" + nId.ToString() + "</span></a>";
                return sub;
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
        public ActionResult Create([Bind(Include = "Id,Title,Text,CreatedDate,WrittenDate,Markdown,Hidden,Pin")] Note note, FormCollection formCollection)
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

            return RedirectToAction("Details", new { id = note.Id });
        }

        // GET: Note/Edit/5
        [ValidateInput(false)]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            var tagLabels = db.NoteTags
                .Where(nt => nt.NoteID == id)
                .Join(db.Tags,
                    nt => nt.TagID,
                    t => t.ID,
                    (nt, t) => t.Lable_fa
                ).ToList() ;
            ViewBag.tagString = String.Join(",", tagLabels);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "Id,Title,Text,CreatedDate,WrittenDate,Markdown,Hidden,Pin")] Note note, FormCollection formCollection)
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
            return RedirectToAction("Details", new { id = note.Id });

        }

        // GET: Note/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Note note = db.Notes.Find(id);
            if (note == null)
            {
                return HttpNotFound();
            }
            return View(note);
        }

        // POST: Note/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Note note = db.Notes.Find(id);
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

            List<Note> notes = db.Notes.OrderByDescending(n => n.WrittenDate).ToList();
            foreach (Note note in notes)
            {
                if (note.Markdown)
                {
                    notes.SingleOrDefault(n => n.Id == note.Id).Text = Markdig.Markdown.ToHtml(note.Text, pipeline);
                }
            }
            return View(notes);
        }

    }
}
