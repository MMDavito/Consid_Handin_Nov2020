using System;
namespace LibraryAPI.Domains
{
    public class Category
    {
        public Nullable<int> id;
        public string category;
        public Category(Nullable<int> id, string category)
        {
            this.id = id;
            this.category = category;
        }
    }
}