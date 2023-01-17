using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace browser_select.Helpers
{
    public class EnumHelper
    {
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
        public static IEnumerable<KeyValuePair<string, string>> GetAllValuesAndDescriptions<TEnum>() where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException("TEnum must be an Enumeration type");
            }

            return from e in Enum.GetValues(typeof(TEnum)).Cast<Enum>()
                   select new KeyValuePair<string, string>(e.ToString(), GetEnumDescription(e));
        }

    }
}
