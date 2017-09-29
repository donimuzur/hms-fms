using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FMS.Core
{
    public class EmailConfiguration
    {
        public string Host { get; set; }

        public int Port { get; set; }
        public string To { get; set; }
        public string User { get; set; }
        public string Password { get; set; }

        public string Subject { get; set; }

        public string Sender { get; set; }

        public string SenderDisplay { get; set; }
        public static EmailConfiguration GetConfig()
        {
            var section = System.Configuration.ConfigurationManager.GetSection("EMS/Email");
            if (section == null)
                throw new ConfigurationErrorsException("The Email configuration section has not been defined in the web.config/app.config");

            return (EmailConfiguration)ConfigurationManager.GetSection("EMS/Email");
        }

        public void LoadConfigValues(XmlNode node)
        {
            var attributeCollection = node.Attributes;

            if (attributeCollection != null && attributeCollection["To"] != null)
                To = attributeCollection["To"].Value;
            if (attributeCollection != null && attributeCollection["User"] != null)
                User = attributeCollection["User"].Value;
            if (attributeCollection != null && attributeCollection["Password"] != null)
                Password = attributeCollection["Password"].Value;
            if (attributeCollection != null && attributeCollection["Subject"] != null)
                Subject = attributeCollection["Subject"].Value;
            if (attributeCollection != null && attributeCollection["Sender"] != null)
                Sender = attributeCollection["Sender"].Value;
            if (attributeCollection != null && attributeCollection["SenderDisplay"] != null)
                SenderDisplay = attributeCollection["SenderDisplay"].Value;
            if (attributeCollection != null && attributeCollection["Host"] != null)
                Host = attributeCollection["Host"].Value;
            if (attributeCollection != null && attributeCollection["Port"] != null)
                Port = Convert.ToInt32(attributeCollection["Port"].Value);

        }
        
    }
    public class EmailConfigurationHandler : IConfigurationSectionHandler
    {
        public Object Create(Object parent, Object configContext, XmlNode node)
        {
            var config = new EmailConfiguration();
            config.LoadConfigValues(node);
            return config;
        }
    }
}
