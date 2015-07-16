using System.Collections.Generic;
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
                return ErrorResult("invalid parameter");
            }

            lock (Indexlock)
            {
                if (index.ContainsKey(alias) && index[alias].Count > 0)
                {
                    return JsonResult(
                        new
                        {
                            result = true,
                            meetings = index[alias]
                        });
                }
            }
            return ErrorResult("No meeting found with alias " + alias);
        }

        public ActionResult SigninMeeting(string alias, int meetingId)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult("invalid parameter");
            }
            var meeting = activeMeetings.Find(m => m.Id == meetingId);
            if (meeting == null)
            {
                return ErrorResult("No active meeting found with meeting id " + meetingId);
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
                    result = true,
                    meeting = meeting
                });
        }

        public async Task<ActionResult> Start(string alias)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult("invalid parameter");
            }
            var meetings = await GetMeetingsFromSharePoint(alias);
            meetings = meetings.FindAll(m => !activeMeetings.Exists(am => am.Id == m.Id));
            if (meetings.Count == 0)
            {
                return ErrorResult("No nonactive meeting found with alias" + alias);
            }
            lock (MeetingLock)
            {
                tempMeetings.AddRange(meetings.FindAll(m => !tempMeetings.Exists(tm => tm.Id == m.Id)));
            }
            return JsonResult(
                new
                {
                    result = true,
                    meetings = meetings
                });

        }

        public ActionResult StartMeeting(string alias, int meetingId)
        {
            if (!ModelState.IsValid || alias == null)
            {
                return ErrorResult("invalid parameter");
            }
            var meeting = tempMeetings.Find(m => m.Id == meetingId);
            if (meeting == null)
            {
                return ErrorResult("No meeting found with meeting id" + meetingId);
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
                    result = true,
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
        private JsonResult ErrorResult(string str)
        {
            return JsonResult(
                new
                {
                    result = false, message = str
                });
        }
        [NonAction]
        private JsonResult JsonResult(object data)
        {
            return Json(data, "application/json", Encoding.UTF8, JsonRequestBehavior.AllowGet);
        }
    }
}