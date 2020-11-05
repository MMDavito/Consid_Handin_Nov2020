using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LibraryAPI.Controllers
{
    [ApiController]
    public class TemporaryDBController : ControllerBase
    {
        [HttpGet("people/all")]
        public ActionResult<IEnumerable<Shite>> GetAll()
        {
            return new[]
            {
            new Shite { Name = "Ana" },
            new Shite { Name = "Felipe" },
            new Shite { Name = "Emillia" }
        };
        }
    }

    public class Shite
    {
        public string Name { get; set; }
    }
}