
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using LibraryAPI.Domains;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;

namespace LibraryAPI.Services
{
    public class CategoryService
    {
        ConnectionFactory connectionFactory;
        public CategoryService()
        {
            connectionFactory = new ConnectionFactory();
        }

        public string getAll()
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
                    SELECT * FROM category ORDER BY id;
                    ";
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        int id = reader.GetInt32(0);
                        string temp = reader.GetString(1);
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Name: " + temp);
                        categories.AddLast((new Category(id, temp)));
                    }
                    cnn.Close();
                }
                return JsonConvert.SerializeObject(categories);
            }
        }

        ///<summary>Inserts into database, catches duplicates and long inputs (byte_count>200). Returns 201 created, 409 duplicate or 400 baad input</summary>
        public HttpResponseMessage insert(Category category)
        {
            if (category.category == null) return new HttpResponseMessage(HttpStatusCode.BadRequest);

            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    INSERT INTO Category VALUES(@category);
                    ";
                    sc.Parameters.Add("@category", SqlDbType.NVarChar);
                    sc.Parameters["@category"].Value = category.category;

                    //TODO FIND WHERE TRYCATCH SHALL BE
                    try
                    {
                        sc.ExecuteNonQuery();//Could be async but will probably not have time to understand cancelationTokens
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
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                            default:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If everything else.
                        }
                    }
                }
                cnn.Close();
            }
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
    }
}
