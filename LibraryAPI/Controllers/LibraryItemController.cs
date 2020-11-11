using System;
using Microsoft.AspNetCore.Mvc;
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
        LibraryItemService service = new LibraryItemService();

        //CategoryService service = new CategoryService();
        [HttpGet("library_item/all")]//I prefer "/categories", but but...
        //public string GetAll()
        public string GetAll()
        {
            return service.getAll();
        }
        [HttpGet("library_item/{id:int}")]
        public string GetOne(int id)//Could return 404 and the like, but won't
        {
            return service.getOne(id);
        }

        [HttpPost("library_item")]
        public HttpResponseMessage Create([FromBody] string content)
        //        public HttpResponseMessage CreateCategory([FromBody] Category myCategory)

        {//Firstly check so it does not set borrower or is Borrowable
            //return new HttpResponseMessage(HttpStatusCode.NotImplemented);
            LibraryItem temp = JsonConvert.DeserializeObject<LibraryItem>(content);
            //HelperVariables.skit=temp.category;
            Console.WriteLine("Hello want to create? " + content);
            return service.insert(temp);

        }
        ///<summary>
        /// This will NOT leave borrower and borrower date unchanged, unless changed to reference_book (always all null and unborrowable if reference-book)!
        ///Must test to change from reference-book to book, could crash system, or make books unborrowable.
        // Use checkin and checkout to borrow.
        ///</summary>
        [HttpPut("library_item/{id:int}")]
        public HttpResponseMessage Update(int id, [FromBody] string content)//Could return 404 and the like, but won't
        {
            LibraryItem myItem = JsonConvert.DeserializeObject<LibraryItem>(content);
            if (myItem.title == null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string msg = "Probably failed for not being borrowable, for reason of being reference-book, or something else";
                response.ReasonPhrase = msg;
                return response;
            }//Else:
            return service.update(id, myItem);

        }
        ///<summary>
        /// Could as well have used the regular put, but this would provide ways of implementing access-controll and authorization.
        /// This does not however provide authorization or authentication, so it is just stupid.
        ///</summary>
        [HttpPut("library_item/check_in/{id:int}")]
        public HttpResponseMessage CheckIn(int id, [FromBody] string content)
        {
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Content to check in: " + content);
            LibraryItem myItem = JsonConvert.DeserializeObject<LibraryItem>(content);
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Date of checkin: " + myItem.borrowDate);
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Title of checkin: " + myItem.title);
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Title of checkin =Null?: " + myItem.title == null);
            if (myItem.title == null)
            {
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                string msg = "Probably failed for not being borrowable, for reason of being reference-book, or something else";
                response.ReasonPhrase = msg;
                return response;
            }

            return service.checkIn(id, myItem);

        }
        ///<summary>
        /// Could as well have used the regular put, but this would provide ways of implementing access-controll and authorization.
        /// This does not however provide authorization or authentication, so it is just stupid.
        ///</summary>
        [HttpPut("library_item/check_out/{id:int}")]
        public HttpResponseMessage CheckOut(int id)
        {
            return service.checkOut(id);
        }
        [HttpDelete("library_item/{id:int}")]
        public HttpResponseMessage Delete(int id)
        {
            return service.delete(id);
        }
    }
}