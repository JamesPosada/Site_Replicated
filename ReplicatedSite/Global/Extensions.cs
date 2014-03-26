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
using System.Globalization;

namespace ReplicatedSite
{
    public static class GeneralExtensions
    {
        /// <summary>
        /// Masks the string with the provided mask (default: '*'), leaving a provided number of unmasked characters remaining (default: 4).
        /// </summary>
        /// <param name="unmaskedCharCount">The number of characters to leave unmasked. Defaults to 4.</param>
        /// <param name="mask">The character used as the mask</param>
        /// <returns>The masked string, or the original string if there were not enough characters to leave unmasked.</returns>
        public static string Mask(this string str, int unmaskedCharCount = 4, char mask = '*')
        {
            if (str.Length <= unmaskedCharCount) return str;
            return str.Substring(str.Length - unmaskedCharCount).PadLeft(str.Length, mask);
        }

        /// <summary>
        /// Cast an object to the provided type
        /// </summary>
        /// <typeparam name="T">The type to convert the source to</typeparam>
        /// <param name="source">The object to convert</param>
        /// <returns>The typed source</returns>
        public static T As<T>(this object source) where T : class
        {
            return (source as T);
        }

        /// <summary>
        /// A shortcut call for string.Format(), it formats strings. 
        /// </summary>
        /// <param name="format">The format of the string</param>
        /// <param name="args">The arguments to merge into the provided format</param>
        /// <returns>The formatted, merged string.</returns>
        public static string FormatWith(this string format, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(format, args);
        }

        /// <summary>
        /// A shortcut call for string.Format(), it formats strings. 
        /// </summary>
        /// <param name="provider">The format provider to use</param>
        /// <param name="format">The format of the string</param>
        /// <param name="args">The arguments to merge into the provided format</param>
        /// <returns>The formatted, merged string.</returns>
        public static string FormatWith(this string format, IFormatProvider provider, params object[] args)
        {
            if (format == null)
                throw new ArgumentNullException("format");

            return string.Format(provider, format, args);
        }

        /// <summary>
        /// Checks if the provided value can be found in the provided collection.
        /// </summary>
        /// <typeparam name="T">THe type of object we are checking.</typeparam>
        /// <param name="source">The value to check</param>
        /// <param name="list"The collection of items></param>
        /// <returns>True if the provided item is found in the collection.</returns>
        public static bool In<T>(this T source, params T[] list)
        {
            if (null == source) throw new ArgumentNullException("source");
            return list.Contains(source);
        }

        /// <summary>
        /// Parses an object into an Enum. This method will attempt to convert the provided object to a string.
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <returns>The typed Enum value</returns>
        public static T ToEnum<T>(this object value)
        {
            return GeneralExtensions.ToEnum<T>(value, true);
        }

        /// <summary>
        /// Parses an object into an Enum. This method will attempt to convert the provided object to a string.
        /// </summary>
        /// <typeparam name="T">The type of the Enum</typeparam>
        /// <param name="value">String value to parse</param>
        /// <param name="ignorecase">Whether the casing should be ignored when parsing the provided value</param>
        /// <returns>The typed Enum value</returns>
        public static T ToEnum<T>(this object value, bool ignorecase)
        {

            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            var valueAsString = value.ToString().Trim();

            if (valueAsString.Length == 0)
            {
                throw new ArgumentException("Must specify valid information for parsing in the string.", "value");
            }

            Type t = typeof(T);

            if (!t.IsEnum)
            {
                throw new ArgumentException("Type provided must be an Enum.", "T");
            }

            return (T)Enum.Parse(t, valueAsString, ignorecase);
        }

        /// <summary>
        /// Parse the provided decimal as the currency of the provided culture
        /// </summary>
        /// <param name="value">The amount</param>
        /// <param name="cultureName">The culture code to use when formatting</param>
        /// <returns>The formatted string result</returns>
        public static string ToCurrency(this decimal value, string cultureName)
        {
            CultureInfo currentCulture = new CultureInfo(cultureName);
            return (string.Format(currentCulture, "{0:C}", value));
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
        ///  Determines if the provided object can be parsed as the provided type. Essentially a condensed TryParse.
        /// </summary>
        /// <typeparam name="T">The type of object to attempt to parse the provided object as.</typeparam>
        /// <param name="objectToBeParsed">The object to attempt to parse.</param>
        /// <returns>Whether or not the object can be parsed as the provided type.</returns>
        public static bool CanBeParsedAs<T>(this object objectToBeParsed)
        {
            try
            {
                var castedObject = Convert.ChangeType(objectToBeParsed, typeof(T));
                return castedObject != null;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Return either the provided value, or the provided fallback
        /// </summary>
        /// <param name="value"></param>
        /// <param name="fallback"></param>
        /// <returns></returns>
        public static string IfNullOrEmpty(this string value, string fallback)
        {
            if (!string.IsNullOrEmpty(value)) return value;
            else return fallback;
        }

        /// <summary>
        /// Shortcut to determine if a string is null or empty
        /// </summary>
        /// <returns></returns>
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }

        /// <summary>
        /// Shortcut to determine if a string is not null or empty
        /// </summary>
        /// <returns></returns>
        public static bool IsNotEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrEmpty(value);
        }
    }


    public static class LinqExtensions
    {
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
            switch (method)
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
            foreach (string prop in props)
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
            foreach (string prop in props)
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

            for (int i = 1; i < list.Count; i++)
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

            for (int i = 1; i < list.Count; i++)
            {
                var orCond = Expression.Equal(Expression.Property(param, propertyName), Expression.Constant(list[i]));

                cond = Expression.And(cond, orCond);
            }

            return Expression.Lambda<Func<TSource, bool>>(cond, param);
        }

        /// <summary>
        /// Returns the only result, or a specified default value.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="list">The list of results to pull a single record from.</param>
        /// <param name="defaultValue">The default value if no single result can be returned.</param>
        /// <returns>Either a single result, or the provided default result.</returns>
        public static T SingleOr<T>(this IEnumerable<T> list, T defaultValue) where T : class
        {
            return list.SingleOrDefault() ?? defaultValue;
        }

        /// <summary>
        /// Returns the first result, or a specified default value.
        /// </summary>
        /// <typeparam name="T">The type of result to return.</typeparam>
        /// <param name="list">The list of results to pull first record from.</param>
        /// <param name="defaultValue">The default value if no single result can be returned.</param>
        /// <returns>Either a first result, or the provided default result.</returns>
        public static T FirstOr<T>(this IEnumerable<T> list, T defaultValue) where T : class
        {
            return list.FirstOrDefault() ?? defaultValue;
        }
    }
}