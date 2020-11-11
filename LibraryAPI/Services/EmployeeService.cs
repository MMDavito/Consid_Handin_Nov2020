
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

        ///<summary> Will allow for corruption if not checked in forehand so that if IS_CEO the manager_id is null.
        ///No time to explain without autoGen comments, and no time for that nore correct try-catch (only sql exceptions, should include system exceptions)</summary>
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

                    else if (employee.isManager)
                    {//Cannot allow manager to be managed by employee, but it can however be without manager.
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
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=0 AND e.is_ceo=0)
                        )> 0;
";//Where not regular employee (employee.isCeo ==false && employee.isManager == false), it should however allow manager_ID=null
                    }
                    else
                    {//Employee cant be managed by ceo
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
                    WHERE
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=1)
                        )> 0;
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
                        sc.Parameters.AddWithValue("@manager_id", (object)employee.managerId ?? DBNull.Value);

                    }
                    catch (System.Exception e)
                    {
                        cnn.Close();
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
                            System.Console.WriteLine("Exception occured in employee service");
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
        ///<summary>Will allow for corruption if not checked in forehand so that if IS_CEO the manager_id is null.
        /// Returns 200 updated, 404 not found or 400 baad input</summary>
        public HttpResponseMessage update(int id, Employee employee)
        {
            if (employee.firstName == null || id < 0) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            int rowsAffected = -1;
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    if (employee.isCEO)
                    {
                        sc.CommandText = @"
                    UPDATE Employee
                    SET
                        first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo='true',
                        is_manager='false',
                        manager_id=@manager_id
                    WHERE -- Following is to allow for updating manager to ceo OR employee to ceo, ALTERNITEVLY simply updating ceo (if is_ceo='true')
                    id=@ID AND(
                        is_ceo='true'
                        OR NOT 
                        (SELECT COUNT(id)
                            FROM Employee as e WHERE e.is_ceo= 'true'
                        )> 0);
                    ";
                    }
                    else if (employee.isManager)
                    {
                        sc.CommandText = @"
                     UPDATE Employee
                    SET
                        first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo='false',
                        is_manager='true',
                        manager_id=@manager_id
                    WHERE
                    id=@ID AND NOT
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=0 AND e.is_ceo=0)
                        )> 0;
";//Where not regular employee (employee.isCeo ==false && employee.isManager == false), it should however allow manager_ID=null
                    }
                    else
                    {
                        //Employee cant be managed by ceo
                        sc.CommandText = @"
                    UPDATE Employee
                    SET
                        first_name=@first_name,
                        last_name=@last_name,
                        salary=@salary,
                        is_ceo='false',
                        is_manager='false',
                        manager_id=@manager_id
                    WHERE
                        id=@ID
                        AND
                        (SELECT COUNT(e.id)
                            FROM Employee as e WHERE e.id=@manager_id 
                            AND (e.is_manager=1)
                        )> 0;
                    ";
                    }
                    //WILL ADD WITH VALUE BECAUSE LACKING TIME:
                    sc.Parameters.AddWithValue("@ID", id);
                    sc.Parameters.AddWithValue("@first_name", employee.firstName);
                    sc.Parameters.AddWithValue("@last_name", employee.lastName);
                    sc.Parameters.AddWithValue("@salary", employee.salary);
                    sc.Parameters.AddWithValue("@manager_id", (object)employee.managerId ?? DBNull.Value);//Should have added an if is ceo and skipped, but wanted same format as the insert
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
                            System.Console.WriteLine("SQLException occured in employee service");
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
                            System.Console.WriteLine("Exception occured in employee service when putting/updating stuff");
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
                    SELECT * FROM Employee WHERE id = @ID;
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
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
                foreach (Employee tempEmployee in employees)
                {
                    cnn.Open();//Could been async, but nothing realy is.
                               //should TODO add trycatchy thingy.
                    using (SqlCommand sc = new SqlCommand())
                    {
                        sc.Connection = cnn;
                        sc.CommandType = CommandType.Text;
                        sc.CommandText = @"
                    SELECT id,first_name FROM Employee WHERE manager_id = @ID;
                    ";
                        sc.Parameters.Add("@ID", SqlDbType.Int);
                        sc.Parameters["@ID"].Value = tempEmployee.id;
                        SqlDataReader reader = sc.ExecuteReader();
                        while (reader.Read())
                        {
                            int tempId = -1;
                            string tempName = null;
                            tempId = reader.GetInt32(0);
                            tempName = reader.GetString(1);
                            tempEmployee.listOfManaged.AddLast(new Employee.Managed(tempId, tempName));
                        }
                    }
                }
            }
            return JsonConvert.SerializeObject(employees);
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
                    DELETE FROM Employee WHERE id = @ID;
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
                            System.Console.WriteLine("SQLException occured in employee service");
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
                            System.Console.WriteLine("Exception occured in employee service when deleteing stuff");
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
        ///<summary>
        /// Tries to delete all employees that are managed by the manager with "manager_id"="id"
        /// Could be denied by the database constraint if one of its managed employees manages others.
        ///</summary>
        public HttpResponseMessage deleteManaged(int id)
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
                    DELETE FROM Employee WHERE managed_by = @ID;
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
                            System.Console.WriteLine("SQLException occured in employee service");
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
                            System.Console.WriteLine("Exception occured in employee service when deleteing stuff");
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
