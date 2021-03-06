﻿using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using MeetingSignIn.Models;

namespace MeetingSignIn.Controllers
{
    public class MeetingsController : Controller
    {
        private static readonly object Indexlock = new object();
        private static readonly object MeetingLock = new object();
        private readonly List<Meeting> activeMeetings = Cache.ActiveMeetings;
        private readonly MeetingContext db = new MeetingContext();
        private readonly Dictionary<string, List<Meeting>> index = Cache.Index;
        private readonly List<Meeting> tempMeetings = Cache.TempMeetings;

        public ActionResult Signin(string alias)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult(1, ModelState.ToString());
            }

            lock (Indexlock)
            {
                if (index.ContainsKey(alias) && index[alias].Count > 0)
                {
                    return JsonResult(
                        new
                        {
                            result = new { status = 0, message = "meetings can be signed in" },
                            meetings = index[alias]
                        });
                }
            }
            return ErrorResult(4, "No meeting found with alias " + alias);
        }

        public ActionResult SigninMeeting(string alias, int meetingId)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult(1, ModelState.ToString());
            }
            var meeting = activeMeetings.Find(m => m.Id == meetingId);
            if (meeting == null)
            {
                return ErrorResult(5, "No active meeting found with meeting id " + meetingId);
            }
            if (meeting.Signin(alias))
            {
                RemoveIndex(alias, meeting);
            }
            if (meeting.Completed)
            {
                lock (MeetingLock)
                {
                    activeMeetings.Remove(meeting);
                }
            }
            return JsonResult(
                new
                {
                    result = new { status = 0, message = "sign in a meeting successful. meeting theme is" + meeting.Theme },
                    meeting = meeting
                });
        }

        public async Task<ActionResult> Start(string alias)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult(1, ModelState.ToString());
            }
            var meetings = await GetMeetingsFromSharePoint(alias);
            meetings = meetings.FindAll(m => !activeMeetings.Exists(am => am.Id == m.Id));
            if (meetings.Count == 0)
            {
                return ErrorResult(2, "No nonactive meeting found with alias" + alias);
            }
            lock (MeetingLock)
            {
                tempMeetings.AddRange(meetings.FindAll(m => !tempMeetings.Exists(tm => tm.Id == m.Id)));
            }
            return JsonResult(
                new
                {
                    result = new { status = 0, message = "meetings can be started" },
                    meetings = meetings
                });

        }

        public ActionResult StartMeeting(string alias, int meetingId)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult(1, ModelState.ToString());
            }
            var meeting = tempMeetings.Find(m => m.Id == meetingId);
            if (meeting == null)
            {
                return ErrorResult(3, "No meeting found with meeting id" + meetingId);
            }
            lock (MeetingLock)
            {
                tempMeetings.Remove(meeting);
                activeMeetings.Add(meeting);
                meeting.Newest = alias;
            }
            AddIndex(meeting);
            return JsonResult(
                new
                {
                    result = new { status = 0, message = "start a meeting successful. meeting theme is" + meeting.Theme },
                    meeting = meeting
                });
        }

        public ActionResult ShowMeeting(string alias)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return Content("invalid parameter");
            }
            var meeting = activeMeetings.Find(m => m.Owner.Equals(alias));
            if (meeting == null)
            {
                return HttpNotFound();
            }
            var persons = meeting.Members.Where(m => m.Signed).Select(
                m => m.Alias).ToList() ?? new List<string>();
            persons.Add(alias);
            var newest = db.Persons.Find(meeting.Newest);
            var owner = db.Persons.Find(meeting.Owner);
            ViewData["alias"] = alias;
            ViewData["persons"] = persons;
            ViewData["newest"] = newest;
            ViewData["owner"] = owner;
            return View();
            //    JsonResult(new
            //{
            //    persons = persons.ToList(),
            //    newest = newest,
            //    owner = owner
            //});
        }

        public ActionResult Login()
        {
            return this.View();
        }

        [NonAction]
        private void AddIndex(Meeting meeting)
        {
            Task.Run(
                () =>
                    {
                        lock (Indexlock)
                        {
                            meeting.Members.ForEach(
                                m =>
                                    {
                                        if (index.ContainsKey(m.Alias))
                                        {
                                            index[m.Alias].Add(meeting);
                                        }
                                        else
                                        {
                                            index[m.Alias] = new List<Meeting> { meeting };
                                        }
                                    });
                        }
                    });
        }

        [NonAction]
        private async Task<List<Meeting>> GetMeetingsFromSharePoint(string alias)
        {
            return await db.Meetings.Include(m => m.Members).Where(m => m.Owner.Equals(alias)).ToListAsync();
        }

        [NonAction]
        private void RemoveIndex(string alias, Meeting meeting)
        {
            lock (Indexlock)
            {
                var value = index[alias];
                if (value != null)
                    value.Remove(meeting);
            }
        }
        [NonAction]
        private JsonResult ErrorResult(int status, string str)
        {
            return JsonResult(
                new
                {
                    result = new { status = status, message = str }
                });
        }
        [NonAction]
        private JsonResult JsonResult(object data)
        {
            return Json(data, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
    }
}