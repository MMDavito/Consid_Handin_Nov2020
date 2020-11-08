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
    public class CategoryController : ControllerBase
    {
        CategoryService service = new CategoryService();
        [HttpGet("category/all")]//I prefer "/categories", but but...
        public string GetAll()
        {
            if (HelperVariables.IS_DEBUG) Console.WriteLine("HEJ ERA DJAVLAR! fRon categoryController");
            return service.getAll();
        }
        [HttpGet("category/{id:int}")]
        public string GetCategory(int id)//Could return 404 and the like, but won't
        {
            return service.getOne(id);
        }

        [HttpPost("/category")]
        public HttpResponseMessage CreateCategory([FromBody] string content)
        //        public HttpResponseMessage CreateCategory([FromBody] Category myCategory)

        {
            Console.WriteLine("HEJJE");
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Category to create:");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(content);

            Category myCategory = JsonConvert.DeserializeObject<Category>(content);

            if (HelperVariables.IS_DEBUG) Console.WriteLine("Category to create:");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myCategory);
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myCategory.category);

            return service.insert(myCategory);
            //return new HttpResponseMessage((HttpStatusCode)418);
            //return request.CreateResponse(HttpStatusCode.OK, user);
        }
        [HttpPut("category/{id:int}")]
        public HttpResponseMessage UpdateCategory(int id, [FromBody] string content)//Could return 404 and the like, but won't
        {
            Console.WriteLine("HEJJE");
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Category to update:");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(id);
            if (HelperVariables.IS_DEBUG) Console.WriteLine(content);

            Category myCategory = JsonConvert.DeserializeObject<Category>(content);

            if (HelperVariables.IS_DEBUG) Console.WriteLine("Category to update:");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myCategory);
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myCategory.category);

            return service.update(id, myCategory);
        }
        [HttpDelete("category/{id:int}")]
        public HttpResponseMessage DeleteCategory(int id)//Could return 404 and the like, but won't
        {
            return service.delete(id);
        }
    }
}