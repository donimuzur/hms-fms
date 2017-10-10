using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using FMS.BusinessObject.Business;
using FMS.Core;
using EnumHelper = FMS.Utils.EnumHelper;

namespace FMS.Website.Helpers
{
    public static class HtmlHelperExtention
    {
        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty, TEnum>(
                    this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression,
                    TEnum selectedValue, object htmlattr = null)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            var selected = Convert.ToInt32(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model ?? 0);
            var list = new List<SelectListItem>();
            for (var i = 0; i < values.Count; i++)
            {
                var choose = i == selected;
                list.Add(new SelectListItem
                {
                    Text = values[i].ToString(),
                    Value = Convert.ToString(i),
                    Selected = choose
                });
            }
            return htmlHelper.DropDownListFor(expression, list, htmlattr);
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty, TEnum>(
                   this HtmlHelper<TModel> htmlHelper,
                   Expression<Func<TModel, TProperty>> expression,
                   TEnum selectedValue, int datatype, string htmlclass)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>().ToList();
            var selected = Convert.ToInt32(ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData).Model ?? 0);
            var list = new List<SelectListItem>();
            for (var i = 0; i < values.Count; i++)
            {
                var choose = i == selected;
                list.Add(new SelectListItem
                {
                    Text = values[i].ToString(),
                    Value = Convert.ToString(i),
                    Selected = choose
                });
            }
            if (datatype == 1)
            {
                list[0].Text = Constans.InList;
                return htmlHelper.DropDownListFor(expression, list, new { disabled = "diabled", @class = htmlclass });
            }

            return htmlHelper.DropDownListFor(expression, list, new { @class = htmlclass });
        }

        public static MvcHtmlString EnumDropDownListFor<TModel, TProperty, TEnum>(
                    this HtmlHelper<TModel> htmlHelper,
                    Expression<Func<TModel, TProperty>> expression,
                    TEnum selectedValue,
            string optionLabel = null, object htmlattr = null)
        {
            var values = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            IEnumerable<SelectListItem> items;

            //todo : remove this section .. no need use languange ..?
            if (values.ToString().Contains("Languages"))
            {
                items = from value in values
                        select new SelectListItem()
                        {
                            Text = value.ToString(),
                            Value = value.ToString()
                        };

            }
            else
            {
                items = from value in values
                        select new SelectListItem()
                        {
                            Text = EnumHelper.GetDescription((Enum)Enum.Parse(typeof(TEnum), value.ToString())),
                            Value = Enum.Parse(typeof(TEnum), value.ToString()).ToString()
                        };
            }
            if (optionLabel == null)
                return htmlHelper.DropDownListFor(expression, items, htmlattr);

            return htmlHelper.DropDownListFor(expression, items, optionLabel, htmlattr);
        }
        
        public static string UserName(this HtmlHelper htmlHelper)
        {
            var user = (Login)HttpContext.Current.Session[Core.Constans.SessionKey.CurrentUser];
            if (user == null)
            {
                return "Testing";
            }
            return user != null ? user.USER_ID : "";
        }

        public static string UserRole(this HtmlHelper htmlHelper)
        {
            var user = (Login)HttpContext.Current.Session[Core.Constans.SessionKey.CurrentUser];
            if (user == null)
            {
                return "User";
            }
            return user != null ? user.UserRole.ToString() : "";
        }

        public static string BooleanToString(this HtmlHelper htmlHelper, bool active, string text)
        {
            return active ? text : "";
        }
        public static string BooleanToString(this HtmlHelper htmlHelper, bool active, string truetext, string falsetext)
        {
            return active ? truetext : falsetext;
        }
        public static bool IsTargetSetting(this HtmlHelper htmlHelper)
        {
            var controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            return controller == "Target";
        }
        public static string CompareText(this HtmlHelper htmlHelper, string sourcetext, string comparetext)
        {
            return sourcetext == comparetext ? "active" : "";
        }

        public static string MenuActiveDashboard(this HtmlHelper htmlHelper)
        {
            var controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            if (Constans.MenuActiveDashboard.ToLower() == controller.ToLower())
            {
                return "active";
            }

            return string.Empty;
        }
        
    }
}