using FMS.Logger.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace FMS.Logger
{
    public class WebLogger
    {
        public const string XML_FILE_NAME = "main-log.xml";

        public static void CreateLog(ExceptionInfo model, string pathLocation)
        {
            try
            {
                if (Directory.Exists(pathLocation))
                {

                    List<ExceptionInfo> listOfErrors = ReadLog(pathLocation);
                    using (FileStream fs = new FileStream(Path.Combine(pathLocation, XML_FILE_NAME), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (XmlWriter writer = XmlWriter.Create(fs, new XmlWriterSettings
                        {
                            Indent = true
                        }))
                        {
                            listOfErrors.Add(model);
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExceptionInfo>));
                            xmlSerializer.Serialize(writer, listOfErrors);
                        }

                    }
                }
            }
            catch (Exception ex) { Trace.WriteLine(ex); }
        }
        public static List<ExceptionInfo> ReadLog(string pathLocation)
        {
            List<ExceptionInfo> list = null;
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(pathLocation, XML_FILE_NAME), FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExceptionInfo>));
                    list = xmlSerializer.Deserialize(fs) as List<ExceptionInfo>;
                }
            }
            catch { list = null; }
            if (list == null)
                list = new List<ExceptionInfo>();
            return list;
        }
    }
}
