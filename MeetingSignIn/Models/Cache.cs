using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeetingSignIn.Models
{
    public static class Cache
    {
        public static readonly List<Meeting> ActiveMeetings = new List<Meeting>();
        public static readonly Dictionary<string, List<Meeting>> Index = new Dictionary<string, List<Meeting>>();
        public static readonly List<Meeting> TempMeetings = new List<Meeting>();
    }
}
