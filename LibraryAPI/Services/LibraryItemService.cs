
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
    public class LibraryItemService
    {
        ConnectionFactory connectionFactory;
        public LibraryItemService()
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
                    SELECT * FROM shite ORDER BY id;
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

        ///<summary>
        /// Inserts into database, catches duplicates and long inputs (byte_count>200). Returns 201 created, 409 duplicate or 400 baad input
        /// Will also return id of the created library_item.
        ///</summary>
        public HttpResponseMessage insert(LibraryItem lib_item)
        {
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                using (SqlCommand cmd = new SqlCommand("INSERT INTO shite output INSERTED.ID VALUES(@bajs)", cnn))
                {
                    try
                    {
                        System.Console.WriteLine("Will insert");
                        cmd.Parameters.AddWithValue("@bajs", HelperVariables.skit);
                        //cmd.Parameters.AddWithValue("@occ", Mem_Occ);
                        cnn.Open();

                        int modified = (int)cmd.ExecuteScalar();

                        if (cnn.State == System.Data.ConnectionState.Open)
                            cnn.Close();
                        System.Console.WriteLine("The inserted id was: " + modified);
                        //HttpResponseMessage response = new HttpResponseMessage();
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created); ;
                        //response.StatusCode = HttpStatusCode.Created;

                        //response.ReasonPhrase = "SUCCESS";

                        //response.Content = new StringContent("'id':"+modified.ToString());
                        var JsonCustomer = JsonConvert.SerializeObject(new Category(modified, HelperVariables.skit));
                        /*                        StringContent content = new StringContent(JsonCustomer, Encoding.UTF8, "application/json");

                                                System.Console.WriteLine(response.Content.ReadAsStringAsync().Result);
                                                System.Console.WriteLine(response.Content.Headers);
                        */
                        response.ReasonPhrase = JsonCustomer;


                        return response;
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
                }
            }
        }
    }
}
