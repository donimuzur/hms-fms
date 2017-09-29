using System;
using System.Linq;
using System.Web.Mvc;

namespace FMS.Website.Code
{
    public class GenericHelpers<T> where T : class
    {
        public static SelectList GenerateList(IQueryable<T> source, Func<T, object> value, Func<T, object> label)
        {
            var selectList = from item in source.AsEnumerable()
                             select new SelectListItem
                             {
                                 Value = value(item).ToString(),
                                 Text = label(item).ToString()
                             };

            return new SelectList(
                selectList
                .GroupBy(item => item.Value)
                .Select(group => group.First()), "Value", "Text");
        }
    }
}