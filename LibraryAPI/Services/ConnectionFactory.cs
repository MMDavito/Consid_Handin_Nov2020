using Microsoft.Data.SqlClient;
namespace LibraryAPI.Services
{
    ///<summary>
    /// This Factory does not handle closing connections.
    /// This responsobility is left to the programer using it.
    ///</summary>
    public class ConnectionFactory
    {
        public SqlConnection cnn = null;
        public ConnectionFactory()
        {
            cnn = new SqlConnection(
                "Server=tcp:" + SECRET.dbServer + ",1433;" +
                "Database=" + SECRET.dbName + ";User ID=" + SECRET.userName + ";" +
                "Password=" + SECRET.password + ";Encrypt=False;" +
                "TrustServerCertificate=False;Connection Timeout=30;"
                );
        }
    }
}