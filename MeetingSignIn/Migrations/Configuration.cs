namespace MeetingSignIn.Migrations
{
    using MeetingSignIn.Models;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<MeetingSignIn.Models.MeetingContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MeetingSignIn.Models.MeetingContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            context.Meetings.AddOrUpdate(
                new Meeting()
                {
                    Theme = "meeting 1",
                    Owner = "owner1",
                    StartTime = new DateTime(2015, 7, 13, 12, 30, 0),
                    EndTime = new DateTime(2015, 7, 13, 13, 0, 0),
                    Members = new List<Member>() { new Member("alias1"), new Member("alias2") },
                    Completed = false
                },
                new Meeting()
                {
                    Theme = "meeting 2",
                    Owner = "owner2",
                    StartTime = new DateTime(2015, 7, 14, 12, 30, 0),
                    EndTime = new DateTime(2015, 7, 14, 17, 0, 0),
                    Members = new List<Member>() { new Member("alias1"), new Member("alias2") },
                    Completed = false
                });
            context.Persons.AddOrUpdate(
                new Person() { Alias = "alias1", Infomation = "I'm alias1" },
                new Person() { Alias = "alias2", Infomation = "I'm alias2" });
        }
    }
}
