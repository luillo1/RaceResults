using System;

namespace RaceResults.Models
{
    public class RaceResult
    {
        public string Id { get; set; }

        public Runner Runner { get; set; }

        public Race Race { get; set; }

        public TimeSpan Time { get; set; }
    }
}
