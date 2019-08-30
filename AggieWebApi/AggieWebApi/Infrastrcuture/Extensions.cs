using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class RegexUtilities
    {
        public static bool IsValidEmail(string strIn)
        {
            // Return true if strIn is in valid e-mail format.
            return Regex.IsMatch(strIn, @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            //@"^(?("")(""[^""]+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))" +
            //@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))$");
        }

        /// <summary>
        /// [Description("Return Property Name as string")]  
        /// [CreaedBy("Anik Sen")]
        /// [CreatedOn("02/10/2017")]
        /// </summary>
        /// <param name="en">propertyExpression</param>
        /// <returns>string</returns>
        public static string GetPropertyName<T>(System.Linq.Expressions.Expression<Func<T>> propertyExpression)
        {
            return (propertyExpression.Body as System.Linq.Expressions.MemberExpression).Member.Name;
        }

        /// <summary>
        /// [Description("UrlDecode to Specific iso-8859-1 encoding")]  
        /// [CreaedBy("Anik Sen")]
        /// [CreatedOn("03/10/2017")]
        /// </summary>
        /// <param name="en">inputString</param>
        /// <returns>string</returns>
        public static string UrlDecodeIso8859(string strvar)
        {
            Encoding enc = Encoding.GetEncoding("iso-8859-1");
            return HttpUtility.UrlDecode(strvar, enc);
        }

        public static string ToFileSize(int value)
        {
            string[] suffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
            for (int i = 0; i < suffixes.Length; i++)
            {
                if (value <= (Math.Pow(1024, i + 1)))
                {
                    return ThreeNonZeroDigits(value / Math.Pow(1024, i)) + " " + suffixes[i];
                }
            }
            return ThreeNonZeroDigits(value / Math.Pow(1024, suffixes.Length - 1)) + " " + suffixes[suffixes.Length - 1];
        }

        // Return the value formatted to include at most three
        // non-zero digits and at most two digits after the
        // decimal point. Examples:
        //         1
        //       123
        //        12.3
        //         1.23
        //         0.12
        private static string ThreeNonZeroDigits(double value)
        {
            if (value >= 100)
            {
                // No digits after the decimal.
                return value.ToString("0,0");
            }
            else if (value >= 10)
            {
                // One digit after the decimal.
                return value.ToString("0.0");
            }
            else
            {
                // Two digits after the decimal.
                return value.ToString("0.00");
            }
        }
    }

    public static class SizeCalculator
    {
        public static string ToSize(this Int64 value, SizeUnits unit)
        {
            return (value / (double)Math.Pow(1024, (Int64)unit)).ToString("0.00");
        }

        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }
    }

    public static class CommonCusromExtension
    {
        /// <summary>Performs a shallow clone of an object by copying property values using reflection.
        /// This is a very expensive way of cloning and shouldn't be used extensively, but is also useful for simple base class to derived class casting.
        /// [CreaedBy("Gouranga Ghosh")]
        /// [CreatedOn("08/08/2017")]
        /// </summary>
        /// </summary>
        /// <typeparam name="TSource">The type of the object to clone</typeparam>
        /// <typeparam name="TDestination">Type type of object to return from the clone. This can be used for simple base class to derived class casting</typeparam>
        /// <param name="objectToClone">The actual object to clone</param>
        /// <returns>Cloned object</returns>
        public static TDestination CloneParentToDeriveClass<TSource, TDestination>(this TDestination objTDestination, TSource objectToClone)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);


            TSource source = objectToClone; //CloneSerializable<TSource>(objectToClone);
            TDestination destination = (TDestination)destinationType.Assembly.CreateInstance(destinationType.FullName);


            PropertyInfo[] props = sourceType.GetProperties();
            foreach (var item in props)
            {
                if (item.CanWrite)
                {
                    object val = item.GetValue(source, null);
                    item.SetValue(destination, val, null);
                }
            }
            return destination;
        }

        /// <summary>Performs a shallow clone of an object by copying property values and adds encryption to it using reflection.
        /// This is a very expensive way of cloning and shouldn't be used extensively, but is also useful for Encrypting the int field to aur specific format.
        /// [CreaedBy("Gouranga Ghosh")]
        /// [CreatedOn("08/08/2017")]
        /// </summary>
        /// </summary>
        /// <typeparam name="TSource">The type of the object Whose field to be encrypt</typeparam>
        /// <typeparam name="TDestination">Type type of object to return from the clone after encryption</typeparam>
        /// <param name="objectToClone">The actual object whose field to be encrypt</param>
        /// <returns>Cloned object with Encrypted field</returns>
        public static TDestination ConvertIntToEncrypted<TSource, TDestination>(this TDestination objTDestination, TSource objectToClone)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);


            TSource source = objectToClone; //CloneSerializable<TSource>(objectToClone);
            TDestination destination = (TDestination)destinationType.Assembly.CreateInstance(destinationType.FullName);


            PropertyInfo[] props = sourceType.GetProperties();
            string[] propsname = props.Select(m => m.Name.ToLower()).ToArray<string>();
            object val = null;
            bool foundEncrypted = false;
            FieldEncryptionTypeAttribute myattr = null;
            object CustAttr = null;
            foreach (var item in props)
            {
                var isNumeric = false;
                val = item.GetValue(source, null);
                if (val != null)
                {
                    switch (Type.GetTypeCode(val.GetType()))
                    {
                        case TypeCode.Byte:
                        case TypeCode.SByte:
                        case TypeCode.UInt16:
                        case TypeCode.UInt32:
                        case TypeCode.UInt64:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Single:
                            isNumeric = true;
                            break;
                        default:
                            isNumeric = false;
                            break;
                    }

                    if (isNumeric)
                    {
                        foreach (var item1 in props)
                        {
                            var propattr = item1.GetCustomAttributes(typeof(FieldEncryptionTypeAttribute));
                            if (propattr != null)
                            {
                                CustAttr = (from row in propattr where row.GetType() == typeof(FieldEncryptionTypeAttribute) select row).FirstOrDefault(); ;
                                if (CustAttr != null)
                                {
                                    myattr = (FieldEncryptionTypeAttribute)CustAttr;
                                }
                            }
                            if (item1.Name.ToLower().Contains(item.Name.ToLower()) && myattr != null && myattr.FieldType == EncryptionType.Encrypted.GetHashCode())
                            {
                                if (item.CanWrite)
                                {

                                    item1.SetValue(destination, EncryptionHelper.Encrypt(val.ToString()), null);
                                    //item.SetValue(source, 0, null);
                                }
                                foundEncrypted = true;
                                break;
                            }
                            CustAttr = null;
                            myattr = null;
                        }

                    }
                    else if (!foundEncrypted)
                    {
                        if (item.CanWrite)
                        {
                            val = item.GetValue(source, null);
                            item.SetValue(destination, val, null);
                        }
                        val = null;
                    }
                    foundEncrypted = false;
                    val = null;

                }

            }
            return destination;
        }


        /// <summary>Performs a shallow clone of an object by copying property values using reflection.
        /// This is a very expensive way of cloning and shouldn't be used extensively, but is also useful for simple any class to other class casting.
        /// [CreaedBy("Gouranga Ghosh")]
        /// [CreatedOn("08/11/2017")]
        /// </summary>
        /// </summary>
        /// <typeparam name="TSource">The type of the object to clone</typeparam>
        /// <typeparam name="TDestination">Type type of object to return from the clone. This can be used for simple base class to derived class casting</typeparam>
        /// <param name="objectToClone">The actual object to clone</param>
        /// <returns>Cloned object</returns>
        public static TDestination CloneSecondToFirstClass<TSource, TDestination>(this TDestination objTDestination, TSource objectToClone)
        {
            Type sourceType = typeof(TSource);
            Type destinationType = typeof(TDestination);


            TSource source = objectToClone; //CloneSerializable<TSource>(objectToClone);
            TDestination destination = (TDestination)destinationType.Assembly.CreateInstance(destinationType.FullName);


            PropertyInfo[] props = sourceType.GetProperties();
            PropertyInfo[] propsDestination = destinationType.GetProperties();
            foreach (var item in props)
            {
                foreach (var itemDest in propsDestination)
                {
                    if (item.Name == itemDest.Name && (item.CanWrite && itemDest.CanWrite) && (item.PropertyType == itemDest.PropertyType))
                    {
                        object val = item.GetValue(source, null);
                        itemDest.SetValue(destination, val, null);
                        break;
                    }
                }
            }
            return destination;
        }

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class FieldEncryptionTypeAttribute : Attribute
    {
        public int FieldType { get; set; }
    }

    public enum EncryptionType
    {
        Normal = 1,
        Encrypted = 2
    }

}