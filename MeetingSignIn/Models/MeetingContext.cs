using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MeetingSignIn.Models
{
    public class MeetingContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to drop and regenerate your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        public MeetingContext() : base("name=MeetingContext")
        {
        }

        public System.Data.Entity.DbSet<MeetingSignIn.Models.Meeting> Meetings { get; set; }
        public System.Data.Entity.DbSet<MeetingSignIn.Models.Person> Persons { get; set; }

    }
}
