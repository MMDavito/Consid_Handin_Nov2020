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





        ///<summary>
        /// If invalid for any reason it sets "Title" to null, should be checked to avoid issues.
        /// Will allow borrowing books from 10kbc to 10k in the future. Or something, no checks.
        ///TODO? Will not allow borrower name to be set without a borrowDate.
        /// Neighter will I controll if checkedin or out, that controll need to be made somewhere in buisness-controll, never in domain.
        ///
        /// Category_id should be checked and be valid from frontend, or it will return database error (httpstatus 400 bad input without detailed message)
        ///</summary>
        public LibraryItem(Nullable<int> id, int categoryId, string title, string author, Nullable<int> pages, Nullable<int> runTimeMinutes, bool isBorrowable, string borrower, string borrowDate, string type)
        {
            Console.WriteLine("DATE BORORWED(probs not string): " + borrowDate);
            //Set the specific variables so it is clear it needs to be set in specific cases.
            this.runTimeMinutes = null;
            this.pages = null;
            this.author = null;
            Console.WriteLine("Hello type: " + type);

            this.title = null;//Set to null so can return without setting it several times
            if (title == null || title.Length == 0 || title.Length > HelperVariables.maxByteLength)
            {
                return;
            }
            //Switch case, check and set specific fields, return if unallowed, where Title is returned as nulll
            if (type == null) return;
            else this.type = type;
            if (type.Equals("Book") || type.Equals("Reference Book"))
            {
                if (type.Equals("Reference Book") && (borrower != null || borrowDate != null || isBorrowable))
                {
                    if (HelperVariables.IS_DEBUG) Console.WriteLine("Turns out me am reference book");//TODO REMOVE DEBUG
                    if (HelperVariables.IS_DEBUG) Console.WriteLine("is type referencebook?: " + (type.Equals("Reference Book")));//TODO REMOVE DEBUG

                    return;
                }
                if (author == null || author.Length == 0 || author.Length > HelperVariables.maxByteLength)
                {
                    if (HelperVariables.IS_DEBUG) Console.WriteLine("Hello author is fucked up: " + author);
                    return;
                }
                if (pages < 0)
                {
                    return;
                }
                //ELSE is valid:
                this.author = author;
                this.pages = pages;
                if (HelperVariables.IS_DEBUG) Console.WriteLine("Hello book with title: " + title);
            }
            else if (type.Equals("DVD") || type.Equals("Audio Book"))
            {
                if (runTimeMinutes == null || runTimeMinutes <= 0)
                {
                    return;
                }
                //ELSE IS VALID
                this.runTimeMinutes = runTimeMinutes;
            }
            else
            {
                System.Console.WriteLine("Default should never occur in library item domain unless wrong spelling of type.");
                return;
            }
            //Cannot controll "isBorrowable" here because lazy and no time for unit testing.
            //Better to check that in service
            if (borrowDate != null)
            {
                if (borrower == null || borrower.Length == 0 || borrower.Length > HelperVariables.maxByteLength)
                {
                    return;
                }
            }
            else if (borrower != null)
            {//Borrow date is null, but borrower is not
                if (HelperVariables.IS_DEBUG) System.Console.WriteLine("Borowdate is null, but borrower has length: " + borrower.Length);
                return;
                //could check length..... 
            }
            //Then set and controll the generic fields:
            this.id = id;
            this.title = title;//Already checked
            this.categoryId = categoryId;

            this.isBorrowable = isBorrowable;
            this.borrower = borrower;
            DateTime theDate;
            bool result = DateTime.TryParseExact(
                borrowDate,
                HelperVariables.expectedFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None,
                out theDate);
            if (result)
            {
                this.borrowDate = theDate;
            }
            else
            {
                return;
            }
        }

        ///<summary>
        /// Will leave borowDate null if reference book or the like
        /// Should check in querry on checkin in, so it is not already borrowed (never trust frontend)
        ///</summary>
        public void checkIn(string borrower, DateTime borrowDate)
        {
            if (type.Equals("Reference Book")) { return; }

            if (borrowDate != null)
            {
                if (borrower == null || borrower.Length == 0 || borrower.Length > HelperVariables.maxByteLength) return;
            }
            else if (borrower != null)
            {//Borrow date is null, but borrower is not
                if (HelperVariables.IS_DEBUG) System.Console.WriteLine("Borowdate is null, but borrower has length: " + borrower.Length);
                return;
                //could check length..... 
            }
            this.borrowDate = borrowDate;
            this.borrower = borrower;
            this.isBorrowable = false;

        }

        ///<summary>
        /// Does no checks, except not being reference book, will set borrowdate and borrower to null, and "isBorrowable" to true.
        /// Should check in querry on checkin in, so it is not already borrowed (never trust frontend)
        ///</summary>
        public void checkOut()
        {
            if (type.Equals("Reference Book")) { return; }
            //Else no checks:
            this.borrowDate = null;
            this.borrower = null;
            this.isBorrowable = true;
        }
    }
}