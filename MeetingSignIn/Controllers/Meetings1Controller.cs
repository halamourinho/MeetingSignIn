using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MeetingSignIn.Models;

namespace MeetingSignIn.Controllers
{
    public class Meetings1Controller : Controller
    {
        private MeetingContext db = new MeetingContext();
        
        public ActionResult Login()
        {
            //ViewData["aliasName"] = Request["name"];
            //return RedirectToAction("Index");
            return View();
        }

        // GET: Meetings1
        public async Task<ActionResult> Index()
        {
           ViewData["alias"] = Request["name"];
           ViewData["persons"] = new List<string>() {"aaa","bbb","ccc" };
           ViewData["newest"] = new Person() {Alias="aa",Photo= "Content/200712231523651_2.jpg", DepartMent="szzz",Infomation="aaaaaaa"};
           return View(await db.Persons.ToListAsync());
        }

        // GET: Meetings1/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = await db.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // GET: Meetings1/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Meetings1/Create
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Theme,Owner,StartTime,EndTime,Completed")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Meetings.Add(meeting);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(meeting);
        }

        // GET: Meetings1/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = await db.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // POST: Meetings1/Edit/5
        // 为了防止“过多发布”攻击，请启用要绑定到的特定属性，有关 
        // 详细信息，请参阅 http://go.microsoft.com/fwlink/?LinkId=317598。
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Theme,Owner,StartTime,EndTime,Completed")] Meeting meeting)
        {
            if (ModelState.IsValid)
            {
                db.Entry(meeting).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(meeting);
        }

        // GET: Meetings1/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Meeting meeting = await db.Meetings.FindAsync(id);
            if (meeting == null)
            {
                return HttpNotFound();
            }
            return View(meeting);
        }

        // POST: Meetings1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Meeting meeting = await db.Meetings.FindAsync(id);
            db.Meetings.Remove(meeting);
            await db.SaveChangesAsync();
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
