using AggieGlobal.WebApi.Controllers.Common;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AggieGlobal.WebApi.Infrastructure
{
    public class NotificationConfiguration
    {
        private static NotificationConfiguration singletonInstance = new NotificationConfiguration();
        private XDocument xdoc;

        private NotificationConfiguration()
        {
            xdoc = XDocument.Load(Path.Combine(Utility.CurrentDirectory, ConfigurationManager.AppSettings[ApplicationConstant.NotificationConfigPath]));
        }

        public static NotificationConfiguration SingletonInstance
        {
            get
            {
                return singletonInstance;
            }
        }

        public NotificationConfigurationSettings GetNotificationConfiguration(int type)
        {
            var notificationElement = xdoc.Descendants("Notification").Where(n => n.Attribute("Type").Value.Equals(type.ToString()));

            var messageElement = notificationElement.Descendants("Message");
            var subject = messageElement != null ? messageElement.Attributes("Subject").FirstOrDefault() : null;

            var resellerMessageElement = notificationElement.Descendants("ResellerMessage");
            var resellerMessageSubject = resellerMessageElement != null ? resellerMessageElement.Attributes("Subject").FirstOrDefault() : null;

            var accountOwnerMessageElement = notificationElement.Descendants("AccountOwnerMessage");
            var accountOwnerMessageSubject = accountOwnerMessageElement != null ? accountOwnerMessageElement.Attributes("Subject").FirstOrDefault() : null;

            var recipientsElement = notificationElement.Descendants("Recipients");
            var email = recipientsElement != null ? recipientsElement.Attributes("Email").FirstOrDefault() : null;

            var notificationConfigurationSettings = new NotificationConfigurationSettings();
            notificationConfigurationSettings.MessageSubject = subject != null ? subject.Value : string.Empty;
            notificationConfigurationSettings.ResellerMessageSubject = resellerMessageSubject != null ? resellerMessageSubject.Value : string.Empty;
            notificationConfigurationSettings.AccountOwnerMessageSubject = accountOwnerMessageSubject != null ? accountOwnerMessageSubject.Value : string.Empty;
            notificationConfigurationSettings.RecipientsEmail = email != null ? email.Value : string.Empty;

            return notificationConfigurationSettings;
        }
    }

    public class NotificationConfigurationSettings
    {
        public string MessageSubject { get; set; }
        public string ResellerMessageSubject { get; set; }
        public string AccountOwnerMessageSubject { get; set; }
        public string RecipientsEmail { get; set; }
    }
}
