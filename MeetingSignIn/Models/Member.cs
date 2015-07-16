using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace MeetingSignIn.Models
{
    public class Member
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Alias { get; set; }
        public bool Signed { get; set; }

        public int MeetingId { get; set; }

        public Member(String alias)
        {
            Alias = alias;
            Signed = false;
        }

        public Member()
        {
            
        }
    }
}