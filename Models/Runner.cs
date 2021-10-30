using System.Collections.Generic;

namespace RaceResults.Models
{
    public class Runner
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<string> Nicknames { get; set; }
    }
}
