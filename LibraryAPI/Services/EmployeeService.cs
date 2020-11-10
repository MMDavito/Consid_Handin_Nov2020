
using System;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using LibraryAPI.Domains;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Net;


namespace LibraryAPI.Services
{
    public class EmployeeService
    {
        ConnectionFactory connectionFactory;
        public EmployeeService()
        {
            connectionFactory = new ConnectionFactory();
        }

        public string getAll()
        {
            LinkedList<Employee> employees = new LinkedList<Employee>();
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM Employee ORDER BY id;
                    ";
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        int id = -1;
                        string firstName = null;
                        string lastName = null;
                        Decimal salary = -1.0m;
                        bool isCEO = false;
                        bool isManager = false;
                        Nullable<int> managerId = null;

                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        id = reader.GetInt32(0);
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("HELVETE HEMTADE ID: " + id);
                        firstName = reader.GetString(1);
                        lastName = reader.GetString(2);
                        salary = reader.GetDecimal(3);
                        isCEO = reader.GetBoolean(4);
                        isManager = reader.GetBoolean(5);
                        if (!reader.IsDBNull(6)) managerId = reader.GetInt32(6);


                        employees.AddLast(new Employee(id, firstName, lastName, salary, isCEO, isManager, managerId, null));
                    }
                    cnn.Close();
                }
                return JsonConvert.SerializeObject(employees);
            }
        }

        ///<summary>No time to explain without autoGen comments, and no time for that nore correct try-catch (only sql exceptions, should include system exceptions)</summary>
        public HttpResponseMessage insert(Employee employee)
        {
            int rowsAffected = -1;

            using (SqlConnection cnn = connectionFactory.cnn)
            {

                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())//Transaction would have been OP?
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    if (employee.isCEO)//This if-else could been avoided with better database constraint design (would been safer but taken longer)
                    {
                        sc.CommandText = @"
                    INSERT INTO Employee(
                        ""first_name"",
                        ""last_name"",
                        ""salary"",
                        ""is_ceo"",
                        ""is_manager"",
                        ""manager_id"")
                    SELECT 
                        @first_name, 
                        @last_name, 
                        @salary, 
                        @is_ceo, 
                        @is_manager, 
                        @manager_id
                    WHERE NOT
                        (SELECT COUNT(id)
                            FROM Employee WHERE is_ceo= 'true'
                        )> 0;
                    ";
                    }
                    else
                    {
                        sc.CommandText = @"
                    INSERT INTO Employee(
                        ""first_name"",
                        ""last_name"",
                        ""salary"",
                        ""is_ceo"",
                        ""is_manager"",
                        ""manager_id"")
                    VALUES(
                        @first_name, 
                        @last_name, 
                        @salary, 
                        @is_ceo, 
                        @is_manager, 
                        @manager_id
                        );
                    ";
                    }
                    try
                    {
                        //WILL ADD WITH VALUE BECAUSE LACKING TIME:
                        sc.Parameters.AddWithValue("@first_name", employee.firstName);
                        sc.Parameters.AddWithValue("@last_name", employee.lastName);
                        sc.Parameters.AddWithValue("@salary", employee.salary);
                        sc.Parameters.AddWithValue("@is_ceo", employee.isCEO);
                        sc.Parameters.AddWithValue("@is_manager", employee.isManager);
                        sc.Parameters.AddWithValue("@manager_id", employee.managerId);

                    }
                    catch (System.Exception e)
                    {cnn.Close();
                        if (HelperVariables.IS_DEBUG) { Console.WriteLine("SHITE EXCEPTION HAPPENED:\n" + e); }
                        return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                    }

                    try
                    {
                        rowsAffected = sc.ExecuteNonQuery();//Could be async but will probably not have time to understand cancelationTokens
                    }
                    catch (SqlException e)
                    {
                        cnn.Close();
                        if (HelperVariables.IS_DEBUG)
                        {
                            System.Console.WriteLine("Exception occured in category service");
                            System.Console.WriteLine(e);
                            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        switch (e.Number)
                        {
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate (no sutch constraints here).
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                            default:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If everything else.
                        }
                    }
                }
                cnn.Close();
            }
            if (rowsAffected <= 0)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
                return new HttpResponseMessage(HttpStatusCode.Created);
        }
        ///<summary>Updates category with given id, catches long input, uses transaction. Returns 200 updated, 404 not found or 400 baad input</summary>
        public HttpResponseMessage update(int id, Category category)
        {
            if (category.category == null || id < 0) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            int rowsAffected = -1;
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    UPDATE Category SET category = @category
                    WHERE id = @ID;
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

                    sc.Parameters.Add("@category", SqlDbType.NVarChar);
                    sc.Parameters["@category"].Value = category.category;

                    //TODO FIND WHERE TRYCATCH SHALL BE
                    try
                    {
                        rowsAffected = sc.ExecuteNonQuery();//Could be async but will probably not have time to understand cancelationTokens
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("RowsAffected: {0}", rowsAffected);
                    }
                    catch (SqlException e)
                    {
                        cnn.Close();
                        if (HelperVariables.IS_DEBUG)
                        {
                            System.Console.WriteLine("SQLException occured in category service");
                            System.Console.WriteLine(e);
                            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        switch (e.Number)
                        {
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                            default:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If everything else.
                        }
                    }
                    catch (System.Exception e)
                    {
                        cnn.Close();
                        if (HelperVariables.IS_DEBUG)
                        {
                            System.Console.WriteLine("Exception occured in category service when putting/updating stuff");
                            System.Console.WriteLine(e);
                            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);//Fail server or db
                    }
                }
                cnn.Close();
            }
            if (rowsAffected <= 0)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
                return new HttpResponseMessage(HttpStatusCode.OK);
        }
        public string getOne(int id)
        {
            LinkedList<Category> categories = new LinkedList<Category>();

            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM category WHERE id = @ID;
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        int tempId = reader.GetInt32(0);
                        string temp = reader.GetString(1);
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Name: " + temp);
                        categories.AddLast((new Category(id, temp)));
                    }

                    cnn.Close();
                }
            }
            return JsonConvert.SerializeObject(categories);
        }
        public HttpResponseMessage delete(int id)
        {
            if (id < 0) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            int rowsAffected = -1;
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    DELETE FROM Category WHERE id = @ID;
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

                    //TODO FIND WHERE TRYCATCH SHALL BE
                    try
                    {
                        rowsAffected = sc.ExecuteNonQuery();//Could be async but will probably not have time to understand cancelationTokens
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("RowsAffected: {0}", rowsAffected);
                    }
                    catch (SqlException e)
                    {
                        cnn.Close();
                        if (HelperVariables.IS_DEBUG)
                        {
                            System.Console.WriteLine("SQLException occured in category service");
                            System.Console.WriteLine(e);
                            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        switch (e.Number)
                        {
                            default:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If everything else.
                        }
                    }
                    catch (System.Exception e)
                    {
                        cnn.Close();
                        if (HelperVariables.IS_DEBUG)
                        {
                            System.Console.WriteLine("Exception occured in category service when deleteing stuff");
                            System.Console.WriteLine(e);
                            //return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);//Fail server or db
                    }
                }
                cnn.Close();
            }
            if (rowsAffected <= 0)
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            else
                return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
