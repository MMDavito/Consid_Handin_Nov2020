using System;
using System.Configuration;
//using LibraryApi.Services.SECRET;
using System.Data.Entity.Infrastructure;
namespace LibraryAPI.Services{
    ///<summary>
    /// This Factory does not handle closing connections.
    /// This responsobility is left to the programer using it.
    ///</summary>
    public class ConnectionFactory{    
        SqlConnectionFactory sqlConnFactory;
        String connStr = "Data Source="+SECRET.dbServer+";User ID="+SECRET.userName+";Password="+SECRET.password+";"
        //connetionString = "Data Source=ServerName;Initial Catalog=DatabaseName;User ID=UserName;Password=Password";
        String databaseName = SECRET.dbName;
        
        ///<summary>
        ///This constructor initiates an connstring, connect using "connect"
        ///</summary>
        public ConnectionFactory(){
            sqlConnFactory = new SqlConnectionFactory(connStr);
        }

        public DbConnection connect(){
            DbConnection dbCon = sqlConnFactory.CreateConnection(databaseName);//Does this not throw/raise exceptions?
            return dbCon;
        }

//public SqlConnection connect()

}
}
