using System;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using FMS.Logger.Models;
using System.Diagnostics;
using System.Collections.Generic;

namespace FMS.Logger
{
    public class CoreLogger
    {
        public const string XML_FILE_NAME = "log.xml";

        public static void CreateLog(ExceptionModel model, string pathLocation)
        {
            try
            {
                if(Directory.Exists(pathLocation))
                {

                    List<ExceptionModel> listOfErrors = ReadLog(pathLocation);
                    using (FileStream fs = new FileStream(Path.Combine(pathLocation, XML_FILE_NAME), FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (XmlWriter writer = XmlWriter.Create(fs, new XmlWriterSettings
                        {
                            Indent = true
                        }))
                        {
                            listOfErrors.Add(model);
                            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExceptionModel>));
                            xmlSerializer.Serialize(writer, listOfErrors);
                        }

                    }
                }
            }
            catch(Exception ex) { Trace.WriteLine(ex); }
        }
        public static List<ExceptionModel> ReadLog(string pathLocation)
        {
            List<ExceptionModel> list = null;
            try
            {
                using (FileStream fs = new FileStream(Path.Combine(pathLocation, XML_FILE_NAME), FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<ExceptionModel>));
                    list = xmlSerializer.Deserialize(fs) as List<ExceptionModel>;
                }
            }
            catch { list = null; }
            if (list == null)
                list = new List<ExceptionModel>();
            return list;
        }
    }
}
