using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using QC =Microsoft.Data.SqlClient;//4 hours wasted thinking it was "System" instead of "Microsoft"
using Microsoft.Data.SqlClient;//4 hours wasted thinking it was "System" instead of "Microsoft"
using LibraryAPI.Services;
namespace LibraryAPI.Controllers
{
    [ApiController]
    public class TemporaryDBController : ControllerBase
    {
        TEMPORARY_dbSERVICE service = new TEMPORARY_dbSERVICE();
        [HttpGet("database/all")]
        public ActionResult<IEnumerable<string>> GetAll()
        {
            
            if(HelperVariables.IS_DEBUG)Console.WriteLine("HEJ ERA DJAVLAR!");
            return service.getAllNames();
            /*
            return new[]
            {
             "Ana",
            "Felipe",
            "Emillia"
        };
        */
        }
    }

    public class Shite
    {
        public string Name { get; set; }
    }
}