using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DunkeyAPI.Comparer
{
    public class CategoryComparer : IComparer<Category>
    {
        public int Compare(Category x, Category y)
        {
            // TODO: Handle x or y being null, or them not having names
            return x.Name.CompareTo(y.Name);
        }
    }
}