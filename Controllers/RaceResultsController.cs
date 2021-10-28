using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RaceResults.Models;

namespace RaceResults.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RaceResultsController : ControllerBase
    {
        private readonly ILogger<RaceResultsController> logger;

        public RaceResultsController(ILogger<RaceResultsController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<RaceResult> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new RaceResult
            {
                Race = new Race
                {
                    Date = DateTime.Now
                }
            })
            .ToArray();
        }
    }
}
