using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using MeetingSignIn.Models;
using System.Collections;

namespace MeetingSignIn.Controllers
{
    [RoutePrefix("api/meeting")]
    public class MeetingsWebController : ApiController
    {
        private MeetingContext db = new MeetingContext();
        private List<Meeting> activeMeetings = new List<Meeting>();
        private List<Meeting> tempMeetings = new List<Meeting>();
        private Dictionary<string, List<Meeting>> index = new Dictionary<string, List<Meeting>>();
        private readonly object meetingLock = new object();
        private readonly object indexlock = new object();

        // POST: api/signin
        /// <summary>
        /// Sign in
        /// </summary>
        /// <param name="alias">alias</param>
        /// <returns>a list of meetings</returns>
        [Route("signin")]
        [ResponseType(typeof(List<Meeting>))]
        public IHttpActionResult PostSignin([FromBody]String alias)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (index.ContainsKey(alias))
            {
                return Ok(index[alias]);
            }
            else
            {
                return BadRequest("No meeting found");
            }
        }
        [Route("signin_a_meeting")]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostSigninMeeting([FromBody]String alias, int meetingId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Meeting meeting = activeMeetings.Find(m => m.Id == meetingId);
            if (meeting == null)
            {
                return BadRequest("Invalid meeting ID");
            }
            if (meeting.Signin(alias))
            {
                RemoveIndex(alias, meeting);
            }
            if (meeting.Completed)
            {
                lock (meetingLock)
                {
                    activeMeetings.Remove(meeting);
                }
            }
            return Ok(meeting);
        }
        [Route("start")]
        [ResponseType(typeof(List<Meeting>))]
        public async Task<IHttpActionResult> PostStart([FromBody]String alias)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            List<Meeting> meetings = await GetMeetingsFromSharePoint(alias);
            meetings = meetings.FindAll(m => !activeMeetings.Exists(am => am.Id == m.Id));

            lock (meetingLock)
            {
                tempMeetings.AddRange(meetings.FindAll(m => !tempMeetings.Exists(tm => tm.Id == m.Id)));
            }
            return Ok(meetings);
        }
        [Route("start_a_meeting")]
        [ResponseType(typeof(Meeting))]
        public IHttpActionResult PostStartMeeting([FromBody]String alias, int meetingId)
        {
            Meeting meeting = tempMeetings.Find(m => m.Id == meetingId);
            lock (meetingLock)
            {
                tempMeetings.Remove(meeting);
                activeMeetings.Add(meeting);
            }
            AddIndex(meeting);
            return Ok(meeting);
        }


        [NonAction]
        private void AddIndex(Meeting meeting)
        {
            Task.Run(
                () =>
                {
                    lock (indexlock)
                    {
                        meeting.Members.ForEach(
                            m =>
                            {
                                if (index[m.Alias] == null)
                                {
                                    index[m.Alias] = new List<Meeting>() { meeting };
                                }
                                else
                                {
                                    index[m.Alias].Add(meeting);
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
            lock (indexlock)
            {
                var value = index[alias];
                if (value != null)
                {
                    value.Remove(meeting);
                }
            }
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