using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MeetingSignIn.Models
{
    public class Meeting
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public String Theme { get; set; }
        public String Owner { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public List<Member> Members { get; set; }
        private int signinCount = 0;
        public bool Completed { get; set; }

        public bool Signin(string alias)
        {
            Member member = Members.Find(m => m.Alias.Equals(alias) && !m.Signed);
            if (member != null)
            {
                return false;
            }
            member.Signed = true;
            signinCount++;
            if (signinCount == Members.Count)
            {
                Completed = true;
            }
            return true;
        }
    }
}