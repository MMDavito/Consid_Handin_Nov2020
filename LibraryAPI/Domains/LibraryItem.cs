using System;
namespace LibraryAPI.Domains
{
    public class LibraryItem
    {
        public Nullable<int> id;
        public int categoryId;
        public string title;
        public string author;
        public Nullable<int> pages;
        public Nullable<int> runTimeMinutes;
        public bool isBorrowable;
        public string borrower;
        public Nullable<DateTime> borrowDate;
        public string type;





        public readonly int maxByteLength = HelperVariables.maxByteLength;
        ///<summary>
        /// If unallowed for any reason it sets "Title" to null, should be checked to avoid issues.
        /// Will allow borrowing books from 10kbc to 10k in the future. Or something, no checks.
        ///TODO? Will not allow borrower name to be set without a borrowDate.
        /// Neighter will I controll if checkedin or out, that controll need to be made somewhere in buisness-controll, never in domain.
        ///
        /// Category_id should be checked and be valid from frontend, or it will return database error (httpstatus 400 bad input without detailed message)
        ///</summary>
        public LibraryItem(Nullable<int> id, int categoryId, string title, string author, Nullable<int> pages, Nullable<int> runTimeMinutes, bool isBorrowable, string borrower, Nullable<DateTime> borrowDate, string type)
        {
            //Set the specific variables so it is clear it needs to be set in specific cases.
            this.runTimeMinutes = null;
            this.pages = null;
            this.author = null;

            this.title = null;//Set to null so can return without setting it several times
            if (title == null || title.Length == 0 || title.Length > maxByteLength)
            {
                return;
            }
            //Switch case, check and set specific fields, return if unallowed, where Title is returned as nulll
            if (type == null) return;
            if (type.Equals("Book") || type.Equals("Reference Book"))
            {
                if (type.Equals("Reference Book") && (borrower != null || borrowDate != null) || isBorrowable) { return; }
                if (author == null || author.Length == 0 || author.Length > maxByteLength)
                {
                    return;
                }
                if (pages < 0) { return; }
                //ELSE is valid:
                this.author = author;
                this.pages = pages;
            }
            else if (type.Equals("DVD") || type.Equals("Audio Book"))
            {
                if (runTimeMinutes == null || runTimeMinutes <= 0) return;
                //ELSE IS VALID
                this.runTimeMinutes = runTimeMinutes;
            }
            else
            {
                System.Console.WriteLine("Default should never occur in library item domain unless wrong spelling of type.");
                return;
            }

            //Then set and controll the generic fields:
            this.id = id;
            this.title = title;//Already checked
            this.categoryId = categoryId;
            this.isBorrowable = isBorrowable;
            this.borrower = borrower;
            this.borrowDate = borrowDate;
            this.type = type;
        }
    }
}