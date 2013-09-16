using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Reflection;
using System.ComponentModel;
using ReplicatedSite.Exigo.WebService;

namespace ReplicatedSite
{
    public static class GlobalExtensions
    {
        /// <summary>
        /// Formats the string by capitalizing every word and trimming spaces off the front and end. Used for cleaning up data that will be saved to Exigo.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        public static string FormatForExigo(this string value)
        {
            return Regex.Replace(value, @"\w+", new MatchEvaluator(Capitalize)).TrimStart(' ').TrimEnd(' ');
        }

        /// <summary>
        /// Formats the string based on the procedures defined for the supplied ExigoDataFormatType. Used for cleaning up data that will be saved to Exigo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns>string</returns>
        public static string FormatForExigo(this string value, ExigoDataFormatType type)
        {
            string s = value.TrimStart(' ').TrimEnd(' ');
            s = FormatByType(s, type);
            return s;
        }

        /// <summary>
        /// Formats the value of the TextBox by capitalizing every word and trimming spaces off the front and end. Used for cleaning up data that will be saved to Exigo.
        /// </summary>
        /// <param name="textBox"></param>
        /// <returns>string</returns>
        public static string FormatForExigo(this TextBox textBox)
        {
            return Regex.Replace(textBox.Text, @"\w+", new MatchEvaluator(Capitalize)).TrimStart(' ').TrimEnd(' ');
        }

        /// <summary>
        /// Formats the value of the TextBox based on the procedures defined for the supplied ExigoDataFormatType. Used for cleaning up data that will be saved to Exigo.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns>string</returns>
        public static string FormatForExigo(this TextBox textBox, ExigoDataFormatType type)
        {
            string s = textBox.Text.TrimStart(' ').TrimEnd(' ');
            s = FormatByType(s, type);
            return s;
        }

        ///<summary>Gets the first week day following a date.</summary> 
        ///<param name="date">The date.</param> 
        ///<param name="dayOfWeek">The day of week to return.</param> 
        ///<returns>The first dayOfWeek day following date, or date if it is on dayOfWeek.</returns> 
        public static DateTime GetNextWeekDay(this DateTime date, DayOfWeek dayOfWeek)
        {
            return date.AddDays((dayOfWeek < date.DayOfWeek ? 7 : 0) + dayOfWeek - date.DayOfWeek);
        }

        /// <summary>
        /// Orders an IQueryable using a specified property and method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property, string method)
        {
            string methodName = string.Empty;
            switch(method)
            {
                case "asc":
                    methodName = "OrderBy";
                    break;
                case "desc":
                    methodName = "OrderByDescending";
                    break;
            }
            return ApplyOrder<T>(source, property, methodName);
        }
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodType)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach(string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodType
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });

            return (IOrderedQueryable<T>)result;
        }

        /// <summary>
        /// Applies an Equals expression to an IQueryable using a specified property and value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string property, object value)
        {
            var param = Expression.Parameter(typeof(T));

            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression leftExpression = arg;
            foreach(string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                leftExpression = Expression.Property(leftExpression, pi);
                type = pi.PropertyType;
            }

            dynamic typedValue = Convert.ChangeType(value, type);

            BinaryExpression condition = Expression.Equal(leftExpression, Expression.Constant(typedValue));

            return source.Where(Expression.Lambda<Func<T, bool>>(condition, param));
        }
        public static Expression<Func<TSource, bool>> ToOrExpression<TSource, TList>(this List<TList> list, string propertyName)
        {
            var param = Expression.Parameter(typeof(TSource));

            var cond = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(list[0]));

            for(int i = 1; i < list.Count; i++)
            {
                var orCond = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(list[i]));

                cond = Expression.Or(cond, orCond);
            }

            return Expression.Lambda<Func<TSource, bool>>(cond, param);
        }
        public static Expression<Func<TSource, bool>> ToAndExpression<TSource, TList>(this List<TList> list, string propertyName)
        {
            var param = Expression.Parameter(typeof(TSource));

            var cond = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(list[0]));

            for(int i = 1; i < list.Count; i++)
            {
                var orCond = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(list[i]));

                cond = Expression.And(cond, orCond);
            }

            return Expression.Lambda<Func<TSource, bool>>(cond, param);
        }

        #region Private Methods
        private static string Capitalize(Match matchString)
        {
            string strTemp = matchString.ToString();
            strTemp = char.ToUpper(strTemp[0]) + strTemp.Substring(1, strTemp.Length - 1).ToLower();
            return strTemp;
        }
        private static string FormatByType(string value, ExigoDataFormatType type)
        {
            string s = value;
            switch(type)
            {
                case ExigoDataFormatType.Phone:
                    s = s.Replace("-", "").Replace(" ", "").Replace(".", "").Replace("(", "").Replace(")", "");
                    break;
                case ExigoDataFormatType.TaxID:
                    s = s.Replace("-", "").Replace(" ", "").Replace(".", "");
                    break;
                case ExigoDataFormatType.Email:
                    s = s.Replace(" ", "").ToLower();
                    break;
                case ExigoDataFormatType.Username:
                    s = s.Replace(" ", "").ToLower();
                    break;
                case ExigoDataFormatType.WebAlias:
                    s = s.Replace(" ", "").ToLower();
                    break;
                case ExigoDataFormatType.CreditCardNumber:
                    s = s.Replace("-", "").Replace(" ", "").Replace(".", "");
                    break;
                case ExigoDataFormatType.BankAccountNumber:
                    s = s.Replace("-", "").Replace(" ", "").Replace(".", "");
                    break;
                case ExigoDataFormatType.BankRoutingNumber:
                    s = s.Replace("-", "").Replace(" ", "").Replace(".", "");
                    break;
                default:
                    throw new Exception(string.Format("'{0}' cannot be formatted for Exigo using ExigoDataFormatType.{0}", value, type.ToString()));
            }
            return s;
        }
        #endregion
    }

    /// <summary>
    /// The type of formatting to be used to the FormatForExigo() extension.
    /// </summary>
    public enum ExigoDataFormatType
    {
        Phone,
        TaxID,
        Email,
        Username,
        WebAlias,
        CreditCardNumber,
        BankAccountNumber,
        BankRoutingNumber
    }
}