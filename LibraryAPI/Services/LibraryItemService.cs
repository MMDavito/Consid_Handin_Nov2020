
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
        ///<summary>
        /// The libraryItem must be borrowable both as argument and in database.
        /// If argument is not borrowable it will return http status 400 bad request.
        /// If the item in the database is unborrowable (or not found) it will return http status 404 not found.
        ///</summary>
        public HttpResponseMessage checkIn(int id, LibraryItem libraryItem)
        {
            int rowsAffected = -1;
            if (libraryItem.isBorrowable != true || libraryItem.type == null || libraryItem.borrowDate == null || libraryItem.type.Equals("Reference Book")) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    UPDATE LibraryItem SET 
                    borrower = @borrower,
                    borrow_date = @borrow_date,
                    is_borrowable = 'false'

                    WHERE 
                    id = @ID
                    AND is_borrowable = 'true'; 
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

                    sc.Parameters.Add("@borrower", SqlDbType.NVarChar);
                    sc.Parameters["@borrower"].Value = libraryItem.borrower;

                    sc.Parameters.Add("@borrow_date", SqlDbType.Date);
                    sc.Parameters["@borrow_date"].Value = ((DateTime)libraryItem.borrowDate).ToString(HelperVariables.expectedFormat);

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
                        {/*
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                        */
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

        ///<summary>
        /// No controll except it being reference book or not is made before setting it to borrower null, borrowdate null and isborrowable to true
        /// 404 is returned if not found or trying to make reference book borrowable (a.k.a checkin it out)
        ///</summary>
        public HttpResponseMessage checkOut(int id)
        {
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
                    UPDATE LibraryItem SET 
                    borrower = null,
                    borrow_date = null,
                    is_borrowable = 'true'

                    WHERE 
                    id = @ID
                    AND NOT type = 'Reference Book'; 
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

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
                        {/*
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                        */
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
        ///<summary>
        /// No controll made, could implement not allowing borrowed boooks to be deleted without being checked out.
        /// Although being an good controll, I do not have time to implement it on frontend.
        ///</summary>
        public HttpResponseMessage delete(int id)
        {
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
                    DELETE FROM LibraryItem 
                    WHERE 
                    id = @ID
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;

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
                        {/*
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                        */
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
        ///<summary>
        /// The libraryItem must be borrowable both as argument and in database.
        /// If argument is not borrowable it will return http status 400 bad request.
        /// If the item in the database is unborrowable (or not found) it will return http status 404 not found.
        ///</summary>
        public HttpResponseMessage update(int id, LibraryItem libraryItem)
        {
            Console.WriteLine("Will now update");
            int rowsAffected = -1;
            //Require correct input:
            if (libraryItem.borrower != null || libraryItem.borrowDate != null
            || (libraryItem.type.Equals("Reference Book") && libraryItem.isBorrowable == true)
            || libraryItem.isBorrowable == false
            ) return new HttpResponseMessage(HttpStatusCode.BadRequest);
            Console.WriteLine("Will now connect");

            //Else try updateing:
            using (SqlConnection cnn = connectionFactory.cnn)
            {
                cnn.Open();//Could been async, but nothing realy is.
                           //should TODO add trycatchy thingy.
                using (SqlCommand sc = new SqlCommand())
                {
                    sc.Connection = cnn;
                    sc.CommandType = CommandType.Text;
                    sc.CommandText = @"
                    UPDATE LibraryItem SET 
                    category_id=@category_id,
                    title=@title,
                    author=@author,
                    pages=@pages,
                    run_time_minutes=@run_time_minutes,
                    is_borrowable = @is_borrowable,
                    borrower = null,
                    borrow_date = null,
                    type=@type
                    
                    WHERE 
                    id = @ID
                    AND borrower IS NULL; 
                    ";
                    sc.Parameters.Add("@ID", SqlDbType.Int);
                    sc.Parameters["@ID"].Value = id;
                    System.Console.WriteLine("Will insert");

                    sc.Parameters.Add("@category_id", SqlDbType.Int);
                    sc.Parameters["@category_id"].Value = libraryItem.categoryId;

                    sc.Parameters.Add("@title", SqlDbType.NVarChar);
                    sc.Parameters["@title"].Value = libraryItem.title;

                    sc.Parameters.Add("@author", SqlDbType.NVarChar);
                    sc.Parameters["@author"].Value = (object)libraryItem.author ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                    sc.Parameters.Add("@pages", SqlDbType.Int);
                    sc.Parameters["@pages"].Value = (object)libraryItem.pages ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!;

                    sc.Parameters.Add("@run_time_minutes", SqlDbType.Int);
                    sc.Parameters["@run_time_minutes"].Value = (object)libraryItem.runTimeMinutes ?? DBNull.Value;//TODO CONTINUE DOING THIS TO ALL!

                    sc.Parameters.Add("@is_borrowable", SqlDbType.Bit);
                    sc.Parameters["@is_borrowable"].Value = libraryItem.isBorrowable;

                    sc.Parameters.Add("@type", SqlDbType.NVarChar);
                    sc.Parameters["@type"].Value = libraryItem.type;

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
                        {/*
                            case 2627:
                                return new HttpResponseMessage(HttpStatusCode.Conflict);//If duplicate.
                            case 2628:
                                return new HttpResponseMessage(HttpStatusCode.BadRequest);//If invalid input/longer than 200Bytes.
                        */
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

    }
}
