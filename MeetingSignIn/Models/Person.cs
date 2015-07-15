using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MeetingSignIn.Models
{
    public class Person
    {
        [Key]
        public string Alias { get; set; }
        public string DepartMent { get; set; }
        public string Photo { get; set; }

        public string Infomation { get; set; }
    }
}