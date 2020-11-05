using System;
namespace LibraryAPI.Services
{
    /// <summary>
    /// This class would been gitignored if it was any form of realism.
    /// There is a possibility there are smarter ways to do it than helper classes (Possibly XML?).
    /// </summary>
    public static class SECRET
    {
        public readonly static string dbServer = "localhost";
//        public readonly static string dbName = "testDB";//For tests
          public readonly static string dbName = "library";//For tests
        
        public readonly static string userName = "SA";
        public readonly static string password = "1Helvete4Lajf";
    }
}