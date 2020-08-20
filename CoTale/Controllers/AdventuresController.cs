using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using CoTale.Models;

namespace CoTale.Controllers
{
    [RequireHttps]
    public class AdventuresController : Controller
    {
        private AdventureDBContext db = new AdventureDBContext();

        // GET: Adventures
        public ActionResult Index()
        {
            var Adventures = from a in db.Adventures
                             select a;
            //Adventures = Adventures.OrderBy(a => a.Order);
            Adventures = Adventures.Where(a => a.Order == 0);
            return View(Adventures.ToList());
        }
        
        public ActionResult Next(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            int NextOrder;

            if (adventure == null)
            {
                //return HttpNotFound();
                NextOrder = 0;
            }
            else
            {
                adventure.Price = adventure.Price + 1;
                db.SaveChanges();
                NextOrder = adventure.Order + 1;
            }

            List<Adventure> NextAdventures = db.Adventures
                .Where(a => a.Order == NextOrder && a.ParentId == id)
                .OrderByDescending(a => a.Price)
                .ToList();

            Stack stories = new Stack();
            Adventure iter = db.Adventures.Find(id);
            while (iter != null)
            {
                stories.Push(iter.Title);
                iter = db.Adventures.Find(iter.ParentId);
            }
            ViewData["stories"] = stories;

            Stack brief = new Stack();
            iter = db.Adventures.Find(id);
            while (iter != null && brief.Count < 4)
            {
                brief.Push(iter.Title);
                iter = db.Adventures.Find(iter.ParentId);
            }
            ViewData["brief"] = brief;

            colAdventure res = new colAdventure();
            res.Adventures = NextAdventures.ToList();
            res.Adventure = new Adventure();
            return View(res);
        }
        public ActionResult Prev(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            id = adventure.ParentId;
            return RedirectToAction("Next", new { id });
        }

        // GET: Adventures/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            return View(adventure);
        }

        // GET: Adventures/Create
        /*
        public ActionResult Create(int? id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Adventure parent = db.Adventures.Find(id);
            if (parent == null)
            {
                return HttpNotFound();
            }

            Adventure adv = new Adventure();
            adv.ParentId = id.GetValueOrDefault();
            adv.Order = parent.Order + 1;
            return View(adv);
        }*/

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID, Price, Title, Order")] Adventure adventure, int? id)
        {
            Adventure parent = db.Adventures.Find(id);
            adventure.ParentId = id.GetValueOrDefault();
            adventure.Order = parent.Order + 1;

            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                db.Adventures.Add(adventure);
                db.SaveChanges();
                return RedirectToAction("Next", new { id });
            }
            return View(adventure);
            //return RedirectToAction("Next", new { id });
        }

        /*
        // POST: Adventures/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( int? id, FormCollection Form)
        {
            //nullchecks pls
//[Bind(Include = "ID, Price,Order")] Adventure adventure,
            Adventure parent = db.Adventures.Find(id);
            Adventure adventure = new Adventure();
            adventure.ParentId = id.GetValueOrDefault();
            adventure.Title = Form["tarea"];
            adventure.Order = parent.Order + 1;

            var errors = ModelState.Values.SelectMany(v => v.Errors);

            //if (ModelState.IsValid)
            if(TryValidateModel(adventure))
            {
                db.Adventures.Add(adventure);
                db.SaveChanges();
                return RedirectToAction("Next", new {id});
            }
            else//ask validation i guess
            {
                TempData["Message"] = "yo chjeck your shit bitchhh";
            }
            //return View(adventure);
            return RedirectToAction("Next", new { id });
        }
        */
        // GET: Adventures/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            return View(adventure);
        }

        // POST: Adventures/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Title,ParentId,Price,Order")] Adventure adventure)
        {
            if (ModelState.IsValid)
            {
                db.Entry(adventure).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(adventure);
        }

        // GET: Adventures/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Adventure adventure = db.Adventures.Find(id);
            if (adventure == null)
            {
                return HttpNotFound();
            }
            return View(adventure);
        }

        // POST: Adventures/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Adventure adventure = db.Adventures.Find(id);
            int parentId = adventure.ParentId;
            db.Adventures.Remove(adventure);
            db.SaveChanges();
            return RedirectToAction("Next", new { id = parentId });
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
