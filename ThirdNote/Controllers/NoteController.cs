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

namespace ThirdNote.Controllers
{
    public class NoteController : Controller
    {

        private NotebookDbContext db = new NotebookDbContext();
        private MarkdownPipeline pipeline = new Markdig.MarkdownPipelineBuilder()
            .UseAdvancedExtensions().UseEmphasisExtras()
            .UseSoftlineBreakAsHardlineBreak().Build();

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
                var dates = db.Relapses.OrderBy(r => r.Date).Select(r=>r.Date).ToList();
                var lastDate = dates.First();
                List<int> Strikes = new List<int>();
                foreach (var item in dates)
                {
                    int offset = (item - lastDate).Days;
                    Strikes.Add(offset);
                    lastDate = item;
                }
                Strikes.Add((DateTime.Now - lastDate).Days);
                Strikes.Reverse();
                indexViewData.Strikes = Strikes;
                //indexViewData.LastStrike = (DateTime.Now - db.Relapses.OrderByDescending(r => r.Date).First().Date).Days;
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
            ViewBag.tags = db.NoteTags.Where(nt => nt.NoteID == note.Id)
                .Join(db.Tags, 
                nt => nt.TagID, 
                t => t.ID, 
                (nt, t) => t);
            if(note.Markdown)
            {
                var result = Markdig.Markdown.ToHtml(note.Text, pipeline);
                note.Text = result;
            }
            return View(note);
        }

        // GET: Note/Create
        [ValidateInput(false)]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Note/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "Id,Title,Text,CreatedDate,WrittenDate,Markdown,Hidden,Pin")] Note note, FormCollection formCollection)
        {            
            if (ModelState.IsValid)
            {
                note.Text = string.IsNullOrEmpty(note.Text) ? "" : note.Text;

                if (DateTime.TryParse(formCollection["MyDate"], out DateTime dt))
                {
                    note.WrittenDate = formCollection["MyTime"] == null ? DateTime.Parse(formCollection["MyDate"]) 
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
                    if (tagLabel == string.Empty || tagLabel == null) continue;
                    
                    // this tagLabel Exists in Tags Table. Just add it's NoteTags record.
                    if (db.Tags.Any(x => x.Lable_en.Equals(tagLabel, StringComparison.OrdinalIgnoreCase) || x.Lable_fa.Equals(tagLabel) ))
                    {
                        Tag tag = db.Tags.Single(t => t.Lable_en.Equals(tagLabel, StringComparison.OrdinalIgnoreCase) || t.Lable_fa.Equals(tagLabel) );
                        // update tagLabel Case to the latest form
                        //tag.Label = tagLabel;
                        if(db.NoteTags.Count(nt => nt.NoteID == note.Id && nt.TagID == tag.ID) == 0)
                        {
                            db.NoteTags.Add(new NoteTag(note, tag));
                        }
                    }
                    else // Create Tag recored. Insert both Tag & NoteTag records to their tables.
                    {
                        Tag tag = new Tag
                        {
                            Label = tagLabel
                        };
                        db.Tags.Add(tag);
                        db.NoteTags.Add(new NoteTag(note, tag));
                        
                    }
                    //db.SaveChanges();
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
                    (nt, t) => t.Label
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
        [ValidateAntiForgeryToken]
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
                foreach (var omittedNT in existingNoteTags.Where(eNT => !inputTagLabels.Any(tl => eNT.Tag.Lable_en.Equals(tl, StringComparison.OrdinalIgnoreCase) || eNT.Tag.Lable_en.Equals(tl))))
                    db.NoteTags.Remove(omittedNT);
                // select inputTagLabels not included in existingNoteTags
                foreach (var newTagLabel in inputTagLabels.Where(t=> !existingNoteTags.Any(eNT => eNT.Tag.Lable_en.Equals(t, StringComparison.OrdinalIgnoreCase) || eNT.Tag.Lable_fa.Equals(t))))
                {
                    if (!db.Tags.Any(t => t.Lable_en.Equals(newTagLabel, StringComparison.OrdinalIgnoreCase) || t.Lable_en.Equals(newTagLabel)))
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
