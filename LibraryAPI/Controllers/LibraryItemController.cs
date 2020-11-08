using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;//4 hours wasted thinking it was "System" instead of "Microsoft"
using LibraryAPI.Services;
using LibraryAPI.Domains;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json;


namespace LibraryAPI.Controllers
{
    [ApiController]
    public class LibraryItemController : ControllerBase
    {
        //CategoryService service = new CategoryService();
        [HttpGet("library_item/all")]//I prefer "/categories", but but...
        public string GetAll()
        {
            return null;
        }
        [HttpGet("library_item/{id:int}")]
        public string GetOne(int id)//Could return 404 and the like, but won't
        {
            return null;
        }

        [HttpPost("/libary_item")]
        public HttpResponseMessage Create([FromBody] string content)
        //        public HttpResponseMessage CreateCategory([FromBody] Category myCategory)

        {//Firstly check so it does not set borrower or is Borrowable
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

        }
        [HttpPut("library_item/{id:int}")]
        public HttpResponseMessage Update(int id, [FromBody] string content)//Could return 404 and the like, but won't
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

        }
        ///<summary>
        /// Could as well have used the regular put, but this would provide ways of implementing access-controll and authorization.
        /// This does not however provide authorization or authentication, so it is just stupid.
        ///</summary>
        [HttpPut("library_item/check_in/{id:int}")]
        public HttpResponseMessage CheckIn(int id, [FromBody] string content)//Could return 404 and the like, but won't
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

        }
        ///<summary>
        /// Could as well have used the regular put, but this would provide ways of implementing access-controll and authorization.
        /// This does not however provide authorization or authentication, so it is just stupid.
        ///</summary>
        [HttpPut("library_item/check_out/{id:int}")]
        public HttpResponseMessage CheckOut(int id, [FromBody] string content)//Could return 404 and the like, but won't
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);

        }
        [HttpDelete("library_item/{id:int}")]
        public HttpResponseMessage Delete(int id)//Could return 404 and the like, but won't
        {
            return new HttpResponseMessage(HttpStatusCode.NotImplemented);
        }
    }
}