
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
    public class LibraryItemService
    {
        ConnectionFactory connectionFactory;
        public LibraryItemService()
        {
            connectionFactory = new ConnectionFactory();
        }

        public string getAll()
        {
            LinkedList<LibraryItem> items = new LinkedList<LibraryItem>();
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM LibraryItem ORDER BY id;
                    ";
                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        string author = null;
                        Nullable<int> pages = null;
                        Nullable<int> run_time_minutes = null;
                        string borrower = null;
                        Nullable<DateTime> borrow_date = null;

                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        int id = reader.GetInt32(0);
                        System.Console.WriteLine("HELVETE HEMTADE ID: " + id);
                        int category_id = reader.GetInt32(1);
                        string title = reader.GetString(2);
                        if (!reader.IsDBNull(3)) author = reader.GetString(3);
                        if (!reader.IsDBNull(4)) pages = reader.GetInt32(4);
                        if (!reader.IsDBNull(5)) run_time_minutes = reader.GetInt32(5);
                        System.Console.WriteLine("Will now get borrowable");
                        bool is_borrowable = reader.GetBoolean(6);
                        System.Console.WriteLine("Borrowable is: " + is_borrowable);

                        if (!reader.IsDBNull(7)) borrower = reader.GetString(7);
                        if (!reader.IsDBNull(8)) borrow_date = reader.GetDateTime(8);
                        string type = reader.GetString(9);


                        string tempDate = borrow_date == null ? tempDate = null : tempDate = ((DateTime)borrow_date).ToString(HelperVariables.expectedFormat);

                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Title: " + title);
                        items.AddLast((new LibraryItem(id, category_id, title, author, pages, run_time_minutes, is_borrowable, borrower, tempDate, type)));
                    }
                    cnn.Close();
                }
                return JsonConvert.SerializeObject(items);
            }
        }
        public string getOne(int id)
        {
            LinkedList<LibraryItem> items = new LinkedList<LibraryItem>();

            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    SELECT * FROM LibraryItem WHERE id = @id;
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

                    SqlDataReader reader = sc.ExecuteReader();
                    while (reader.Read())
                    {
                        string author = null;
                        Nullable<int> pages = null;
                        Nullable<int> run_time_minutes = null;
                        string borrower = null;
                        Nullable<DateTime> borrow_date = null;

                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("\n");
                        System.Console.WriteLine("HELVETE HEMTADE ID: " + id);
                        int category_id = reader.GetInt32(1);
                        string title = reader.GetString(2);
                        if (!reader.IsDBNull(3)) author = reader.GetString(3);
                        if (!reader.IsDBNull(4)) pages = reader.GetInt32(4);
                        if (!reader.IsDBNull(5)) run_time_minutes = reader.GetInt32(5);
                        System.Console.WriteLine("Will now get borrowable");
                        bool is_borrowable = reader.GetBoolean(6);
                        System.Console.WriteLine("Borrowable is: " + is_borrowable);

                        if (!reader.IsDBNull(7)) borrower = reader.GetString(7);
                        if (!reader.IsDBNull(8)) borrow_date = reader.GetDateTime(8);
                        string type = reader.GetString(9);


                        string tempDate = borrow_date == null ? tempDate = null : tempDate = ((DateTime)borrow_date).ToString(HelperVariables.expectedFormat);

                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("ID: " + id + ", Title: " + title);
                        items.AddLast((new LibraryItem(id, category_id, title, author, pages, run_time_minutes, is_borrowable, borrower, tempDate, type)));
                    }
                    cnn.Close();
                }
                return JsonConvert.SerializeObject(items);
            }
        }



        ///<summary>
        /// Inserts into database, catches duplicates and long inputs (byte_count>200). Returns 201 created, 409 duplicate or 400 baad input
        /// Will also return id of the created library_item.
        ///</summary>
        public HttpResponseMessage insert(LibraryItem lib_item)
        {
            //Cannot add lib_items borrowed.
            //This may be hack because i always parse the same json nomatter what
            lib_item.borrower = null;
            lib_item.borrowDate = null;
            if (lib_item.type.Equals("Reference Book")) lib_item.isBorrowable = false;
            else lib_item.isBorrowable = true;
            //Now Insert the lib_item:
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                using (SqlCommand sc = new SqlCommand())
                {
                    try
                    {
                        sc.Connection = cnn;
                        sc.CommandType = CommandType.Text;
                        sc.CommandText = @"
INSERT INTO LibraryItem
    (
    ""category_id"",
    ""title"",
    ""author"",
    ""pages"",
    ""run_time_minutes"",
    ""is_borrowable"",
    ""borrower"",
    ""borrow_date"",
    ""type"") 
output INSERTED.ID
VALUES(
    @category_id,
    @title,
    @author,
    @pages,
    @run_time_minutes,
    @is_borrowable,
    @borrower,
    @borrow_date,
    @type
);
                        ";


                        System.Console.WriteLine("Will insert");
                        sc.Parameters.Add("@category_id", SqlDbType.Int);
                        sc.Parameters["@category_id"].Value = lib_item.categoryId;

                        sc.Parameters.Add("@title", SqlDbType.NVarChar);
                        sc.Parameters["@title"].Value = lib_item.title;

                        sc.Parameters.Add("@author", SqlDbType.NVarChar);
                        sc.Parameters["@author"].Value = (object)lib_item.author ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                        sc.Parameters.Add("@pages", SqlDbType.Int);
                        sc.Parameters["@pages"].Value = (object)lib_item.pages ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                        sc.Parameters.Add("@run_time_minutes", SqlDbType.Int);
                        sc.Parameters["@run_time_minutes"].Value = (object)lib_item.runTimeMinutes ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!

                        sc.Parameters.Add("@is_borrowable", SqlDbType.Bit);
                        sc.Parameters["@is_borrowable"].Value = lib_item.isBorrowable;


                        sc.Parameters.Add("@borrower", SqlDbType.NVarChar);
                        sc.Parameters["@borrower"].Value = (object)lib_item.borrower ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                        sc.Parameters.Add("@borrow_date", SqlDbType.Date);
                        sc.Parameters["@borrow_date"].Value = (object)lib_item.borrowDate ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                        sc.Parameters.Add("@type", SqlDbType.NVarChar);
                        sc.Parameters["@type"].Value = lib_item.type;


                        cnn.Open();

                        int modified = (int)sc.ExecuteScalar();

                        if (cnn.State == System.Data.ConnectionState.Open)
                            cnn.Close();
                        if (HelperVariables.IS_DEBUG) System.Console.WriteLine("The inserted id was: " + modified);
                        HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.Created); ;
                        var JsonCustomer = "{\"id\":" + modified + "}";
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
