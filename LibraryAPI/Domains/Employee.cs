using System;
namespace LibraryAPI.Domains
{
    public class Employee
    {
        public Nullable<int> id;
        public string firstName;
        public string lastName;
        public Decimal salary;
        public bool isCEO;
        public bool isManager;
        public Nullable<int> managerId; //Whom is managed by. NO ONE MANAGES CEO (should create bug where CEO == Manager of himself, thereby making impossible to delete)
                                        //WOULD USED THIS AND JOINED; BUT NO TIME, (possibly creating an domain object "manager" to use instead of employee, but NO TIME)        public Employee manager; //Whom is managed by. NO ONE MANAGES CEO (should create bug where CEO == Manager of himself, thereby making impossible to delete)


        public Nullable<int> rank; //To use when calculating salary when comming from frontend (where inputed an rank integer between 1 and 10).
        ///<summary>
        /// Will allow null as name and other things, because complete lack of time. Will however ignore ceo with managerId.
        ///<summary>
        public Employee(Nullable<int> id, string firstName, string lastName, Decimal salary, bool isCEO, bool isManager, Nullable<int> managerId, Nullable<int> rank)
        {
            if ((managerId != null && isCEO) || managerId == id) return;//Would be cool to manage yourself, but abit too exiting for deadline without tests
            this.id = id;
            this.firstName = firstName;
            this.lastName = lastName;
            this.salary = salary;
            this.isCEO = isCEO;
            this.isManager = isManager;
            this.managerId = managerId;
            this.rank = rank;

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
            return (salaryCoefficient() * ((Decimal)rank));
        }
    }
}