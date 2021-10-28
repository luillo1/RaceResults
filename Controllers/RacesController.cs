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
    public class RacesController : ControllerBase
    {
        private readonly ILogger<RacesController> logger;

        public RacesController(ILogger<RacesController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public IEnumerable<Race> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new Race
            {
                Date = DateTime.Now
            })
            .ToArray();
        }
    }
}
