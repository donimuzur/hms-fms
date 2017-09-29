using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FMS.Utils
{
    public class DataTableHelper
    {

        public static DataTable ConvertToDataTable(object[] inArray, DataTable dataTable)
        {
            if (inArray.Length == 0)
                return new DataTable();

            // Extract all our properties (public & static) from our type (generic)
            PropertyInfo[] proInfoArray = inArray[0].GetType().GetProperties();

            List<String> columnNames = new List<String>();

            for (int z = 0; z < dataTable.Columns.Count; z++)
                columnNames.Add(dataTable.Columns[z].ColumnName);

            foreach (var obj in inArray)
            {
                if (obj == null)
                    continue;

                DataRow row = dataTable.NewRow();
                String columnName = "";
                Boolean hasRowData = false;

                for (int i = 0; i < proInfoArray.Length; i++)
                {
                    var currentColumnName = columnNames.Where(w => w.Equals(proInfoArray[i].Name)).FirstOrDefault();

                    columnName = (currentColumnName != null ? currentColumnName : "");

                    if (!String.IsNullOrEmpty(columnName))
                    {
                        var tmp = obj.GetType().InvokeMember(proInfoArray[i].Name, BindingFlags.GetProperty, null, obj, null);
                        if (tmp != null)
                        {
                            if (dataTable.Columns[columnName].DataType == typeof(DateTime))
                            {
                                //try//Useless try catch !
                                //{
                                row[columnName] = DateTime.Parse(tmp.ToString(), CultureInfo.InvariantCulture);
                                //}
                                //catch (Exception)
                                //{
                                //    throw;
                                //}

                                if ((DateTime)row[columnName] == DateTime.MinValue)
                                    row[columnName] = DBNull.Value;
                            }
                            else
                                row[columnName] = tmp;
                        }
                        else
                        {
                            row[columnName] = DBNull.Value;
                        }

                        hasRowData = true;
                    }
                }
                if (hasRowData)
                    dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public static DataRow ConvertObjectToDataRow(object obj, DataTable dataTable)
        {
            DataRow row = dataTable.NewRow();

            if (obj == null)
                return row;

            // Extract all our properties (public & static) from our type (generic)
            PropertyInfo[] proInfoArray = obj.GetType().GetProperties();

            List<String> columnNames = new List<String>();

            for (int z = 0; z < dataTable.Columns.Count; z++)
                columnNames.Add(dataTable.Columns[z].ColumnName);

            String columnName = "";

            for (int i = 0; i < proInfoArray.Length; i++)
            {
                var currentColumnName = columnNames.Where(w => w.Equals(proInfoArray[i].Name)).FirstOrDefault();

                columnName = (currentColumnName != null ? currentColumnName : "");

                if (!String.IsNullOrEmpty(columnName))
                {
                    var tmp = obj.GetType().InvokeMember(proInfoArray[i].Name, BindingFlags.GetProperty, null, obj, null);
                    if (tmp != null)
                    {
                        if (dataTable.Columns[columnName].DataType == typeof(DateTime))
                        {
                            row[columnName] = DateTime.Parse(tmp.ToString(), CultureInfo.InvariantCulture);

                            if ((DateTime)row[columnName] == DateTime.MinValue)
                                row[columnName] = DBNull.Value;
                        }
                        else
                            row[columnName] = tmp;
                    }
                    else
                    {
                        row[columnName] = DBNull.Value;
                    }
                }
            }

            return row;
        }

        public static List<TSource> ConvertDataTableToList<TSource>(DataTable dataTable) where TSource : new()
        {
            var dataList = new List<TSource>();

            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic;

            //var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
            //                     select new
            //                     {
            //                         Name = aProp.Name,
            //                         Type = Nullable.GetUnderlyingType(aProp.PropertyType) ?? aProp.PropertyType
            //                     }).ToList();

            //var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
            //                         select new { Name = aHeader.ColumnName, Type = aHeader.DataType }).ToList();

            var objFieldNames = (from PropertyInfo aProp in typeof(TSource).GetProperties(flags)
                                 select new
                                 {
                                     Name = aProp.Name
                                 }).ToList();

            var dataTblFieldNames = (from DataColumn aHeader in dataTable.Columns
                                     select new { Name = aHeader.ColumnName }).ToList();

            var commonFields = objFieldNames.Intersect(dataTblFieldNames).ToList();

            foreach (DataRow dataRow in dataTable.Rows)
            {
                var aTSource = new TSource();
                foreach (var aField in commonFields)
                {
                    PropertyInfo propertyInfo = aTSource.GetType().GetProperty(aField.Name);
                    propertyInfo.SetValue(aTSource, Convert.ChangeType(dataRow[aField.Name], propertyInfo.PropertyType), null);
                }
                dataList.Add(aTSource);
            }
            return dataList;
        }
    }
}
