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
    public class EmployeeController : ControllerBase
    {
        EmployeeService service = new EmployeeService();
        [HttpGet("employee/all")]//I prefer "/categories", but but...
        public string GetAll()
        {
            if (HelperVariables.IS_DEBUG) Console.WriteLine("HEJ ERA DJAVLAR! fRon EmployeeController");
            return service.getAll();
        }
        [HttpGet("employee/{id:int}")]
        public string GetOne(int id)//Could return 404 and the like, but won't
        {
            return service.getOne(id);
        }

        [HttpPost("/employee")]
        public HttpResponseMessage CreateEmployee([FromBody] string content)
        //        public HttpResponseMessage CreateCategory([FromBody] Category myCategory)

        {
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Employee to create");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(content);
            Employee myEmployee = null;
            try
            {
                myEmployee = JsonConvert.DeserializeObject<Employee>(content);

            }
            catch (System.Exception e)
            {
                if (HelperVariables.IS_DEBUG) { Console.WriteLine("SHITE EXCEPTION HAPPENED:\n" + e); }
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }

            myEmployee.salary = myEmployee.calculateSalary();

            if (HelperVariables.IS_DEBUG) Console.WriteLine("Employee to create");
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myEmployee);
            if (HelperVariables.IS_DEBUG) Console.WriteLine(myEmployee.firstName);
            if (myEmployee.firstName == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);//Something failed (eighter shit from frontend, or managerid existant on an ceo)
            return service.insert(myEmployee);
            //            return service.insert(myEmployee);

            //return new HttpResponseMessage((HttpStatusCode)418);
            //return request.CreateResponse(HttpStatusCode.OK, user);
        }
        [HttpPut("employee/{id:int}")]
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
        [HttpDelete("employee/{id:int}")]
        public HttpResponseMessage DeleteEmployee(int id)//Could return 404 and the like, but won't
        {
            return service.delete(id);
        }
    }
}