using System;
using System.ComponentModel.DataAnnotations;
using RaceResults.Common.Models;

namespace RaceResults.Api.ResponseObjects
{
    public class RaceResultResponse
    {
        public RaceResult RaceResult { get; set; }

        public Member Member { get; set; }

        public Race Race { get; set; }
    }
}
