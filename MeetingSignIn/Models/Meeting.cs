using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Contexts;
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
        private int _signinCount = 0;
        public bool Completed { get; set; }
        public string Newest { get; set; }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool Signin(string alias)
        {
            var member = Members.Find(m => m.Alias.Equals(alias) && !m.Signed);
            if (member == null)
            {
                return false;
            }
            member.Signed = true;
            Newest = alias;
            _signinCount++;
            if (_signinCount == Members.Count)
            {
                Completed = true;
            }
            return true;
        }
    }
}