using System.ComponentModel;
using System.Data;
using System.Reflection;

namespace Monk_Task.Helpers
{
    public static class Extensions
    {
        public static string GetDescription(this object value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = Attribute.GetCustomAttribute(fieldInfo, typeof(DescriptionAttribute)) as DescriptionAttribute;
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public enum SuccessLogType
        {

            [Description("Updated Successfully")]
            Updated,
            [Description("Created Successfully")]
            Create,
            [Description("Added Successfully")]
            Added,
            [Description("Deleted Successfully")]
            Delete,
            [Description("No Coupons found")]
            NoData,
            [Description("InValid Code!")]
            Invalid

        }
        public enum ErrorLogType
        {

            [Description("Could not create coupon, Please try again later.")]
            NotCreated,

            [Description("Invalid coupon.")]
            NoCoupon,

            [Description("An error ocurred!")]
            Error,

        }
        public static DataTable ConvertToTableSQL<T>(this IEnumerable<T> data)
        {
            DataTable table = new DataTable();
            PropertyInfo[] properties = typeof(T).GetProperties();

            foreach (PropertyInfo property in properties)
            {
                table.Columns.Add(property.Name, Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType);
            }

            foreach (T item in data)
            {
                DataRow row = table.NewRow();

                foreach (PropertyInfo property in properties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }

                table.Rows.Add(row);
            }

            return table;
        }

    }
}
