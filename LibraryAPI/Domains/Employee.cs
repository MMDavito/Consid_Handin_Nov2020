using System;
using System.Collections.Generic;
namespace LibraryAPI.Domains
{
    public class Employee
    {
         
        public class Managed
        {
            public int id;
            public string first_name;
            public Managed(int id, string first_name)
            {
                this.id = id;
                this.first_name = first_name;
            }
        }
        ///<summary>
        /// May use this when returning getOne() from service.
        /// May also just be waste of space.
        ///</summary>
        public Nullable<int> id;
        public string firstName;
        public string lastName;
        public Decimal salary;
        public bool isCEO;
        public bool isManager;
        public Nullable<int> managerId; //Whom is managed by. NO ONE MANAGES CEO (should create bug where CEO == Manager of himself, thereby making impossible to delete)
                                        //WOULD USED THIS AND JOINED; BUT NO TIME, (possibly creating an domain object "manager" to use instead of employee, but NO TIME)        public Employee manager; //Whom is managed by. NO ONE MANAGES CEO (should create bug where CEO == Manager of himself, thereby making impossible to delete)
        public Nullable<int> rank; //To use when calculating salary when comming from frontend (where inputed an rank integer between 1 and 10).
        public LinkedList<Managed> listOfManaged = new LinkedList<Managed>();//Will probably never be used

        ///<summary>
        /// Will allow null as name and other things, because complete lack of time. Will however ignore ceo with managerId.
        ///<summary>
        public Employee(Nullable<int> id, string firstName, string lastName, Decimal salary, bool isCEO, bool isManager, Nullable<int> managerId, Nullable<int> rank)
        {
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Tryin to add employee: " + firstName);
            if (rank != null && (rank < 1 || rank > 10))
            {
                if (HelperVariables.IS_DEBUG) Console.WriteLine("Failed tryin to add employee, can't be employeed without manager: " + firstName);
                return;//Would be cool to be manager of yourself, but abit too exiting for deadline without tests
            }

            if (!isCEO && !isManager && (managerId == null))
            {
                if (HelperVariables.IS_DEBUG) Console.WriteLine("Failed tryin to add employee, can't be employeed without manager: " + firstName);
                return;//Would be cool to be manager of yourself, but abit too exiting for deadline without tests
            }

            if (managerId != null && (isCEO || managerId == id))
            {
                if (HelperVariables.IS_DEBUG) Console.WriteLine("Failed tryin to add employee because ceo can't be managed, or somebody trying to self-manage: " + firstName);
                return;//Would be cool to be manager of yourself, but abit too exiting for deadline without tests
            }

            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.salary = salary;
            this.isCEO = isCEO;
            this.isManager = isManager;
            this.managerId = managerId;
            this.rank = rank;
            if (rank == null && salary > 0)
            {
                this.rank = (int)(salary / salaryCoefficient());//This is terrible design, still don't understand why not place the rank and get the salary by a view or something.
                //It is included to help the frontend.
            }
        }
        ///Should be in database, or anything else so server restart is not needed when changeing company policy:
        public Decimal salaryCoefficient()
        {
            if (isCEO)
            {
                return 2.725m;
            }
            else if (isManager)
            {
                return 1.725m;
            }
            else
            {
                //IS EMPLOYEE:
                return 1.125m;
            }
        }
        public Decimal calculateSalary()
        {//TODO FIND ERROR HERE:
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Rank is: " + rank);
            if (HelperVariables.IS_DEBUG) Console.WriteLine("Coeff is: " + salaryCoefficient());
            return (salaryCoefficient() * ((Decimal)rank));
        }
    }
}