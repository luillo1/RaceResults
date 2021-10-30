using System;

namespace RaceResults.Models
{
    public enum Distance
    {
        FiveK,
        TenK,
        HalfMarathon,
        Marathon
    }

    public class Race
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public Distance Distance { get; set; }
    }
}
