using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.IO;
using System.ComponentModel;
using System.Configuration;
using System.Net.Configuration;

namespace ApiProject.Common
{
    class clsEmail
    {
        public static void PlainText()
        {
            try
            {
                var smtp = new SmtpClient();
                var credential = (NetworkCredential)smtp.Credentials;
                string strHost = smtp.Host;
                int port = smtp.Port;
                string strUserName = credential.UserName;
                string strFromPass = credential.Password;
                smtp.EnableSsl = true;

                //create the mail message
                MailMessage mail = new MailMessage();

                //set the addresses
                var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
                // SmtpClient smtp = new SmtpClient(smtpSection.Network.Host);


                string EmailFrom = smtpSection.Network.UserName;

                mail.From = new MailAddress(EmailFrom);
                mail.To.Add("anemr@ntmcs.com");

                //set the content
                mail.Subject = "This is an email";
                mail.Body = "this is the body content of the email.";

                //send the message
                //  //SmtpClient smtp = new SmtpClient("127.0.0.1");

                smtp.Send(mail);
                Console.WriteLine("email sent");
                //Console.WriteLine(smtpSection.Network.Host);
                //Console.WriteLine(smtpSection.Network.UserName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
        }
        static void HtmlEmail()
        {
            //create the mail message

            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is a sample body with html in it. <b>This is bold</b> <font color=#336699>This is blue</font>";
            mail.IsBodyHtml = true;

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void MultiPartMime()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";

            //first we create the Plain Text part
            AlternateView plainView = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable by those clients that don't support html", null, "text/plain");
            //then we create the Html part
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString("<b>this is bold text, and viewable by those mail clients that support html</b>", null, "text/html");
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);


            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1"); //specify the mail server address
            smtp.Send(mail);
        }

        static void FriendlyFromName()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            //to specify a friendly 'from' name, we use a different ctor
            mail.From = new MailAddress(EmailFrom, "Steve James");
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);

        }

        static void FriendlyToName()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            //to specify a friendly 'from' name, we use a different ctor
            mail.From = new MailAddress(EmailFrom, "Steve James");

            //since the To,Cc, and Bcc accept addresses, we can use the same technique as the From address
            mail.To.Add(new MailAddress("anemr@ntmcs.com", "Beth Jones"));
            mail.CC.Add(new MailAddress("donna@yourcompany.com", "Donna Summers"));
            mail.Bcc.Add(new MailAddress("bob@yourcompany.com", "Bob Smith"));

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void MultipleRecipients()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            //to specify a friendly 'from' name, we use a different ctor
            mail.From = new MailAddress(EmailFrom, "Steve James");

            //since the To,Cc, and Bcc accept addresses, we can use the same technique as the From address
            //since the To, Cc, and Bcc properties are collections, to add multiple addreses, we simply call .Add(...) multple times
            mail.To.Add("anemr@ntmcs.com");
            mail.To.Add("you2@yourcompany.com");
            mail.CC.Add("cc1@yourcompany.com");
            mail.CC.Add("cc2@yourcompany.com");
            mail.Bcc.Add("blindcc1@yourcompany.com");
            mail.Bcc.Add("blindcc2@yourcompany.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void FriendlyNonAsciiName()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            //to specify a friendly non ascii name, we use a different ctor.
            //A ctor that accepts an encoding that matches the text of the name
            mail.From = new MailAddress(EmailFrom, "Steve Øbirk", Encoding.GetEncoding("iso-8859-1"));
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);

        }

        static void SetPriority()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //specify the priority of the mail message
            mail.Priority = MailPriority.High;

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void SetTheReplyToHeader()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //specify the priority of the mail message
            mail.ReplyTo = new MailAddress("SomeOtherAddress@mycompany.com");

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void CustomHeaders()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //to add custom headers, we use the Headers.Add(...) method to add headers to the
            //.Headers collection
            mail.Headers.Add("X-Company", "My Company");
            mail.Headers.Add("X-Location", "Hong Kong");


            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }
        static void ReadReceipts()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //To request a read receipt, we need add a custom header named 'Disposition-Notification-To'
            //in this example, read receipts will go back to 'someaddress@mydomain.com'
            //it's important to note that read receipts will only be sent by those mail clients that
            //a) support them
            //and
            //b)have them enabled.
            mail.Headers.Add("Disposition-Notification-To", "<someaddress@mydomain.com>");


            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }

        static void AttachmentFromFile()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this content is in the body";

            //add an attachment from the filesystem
            mail.Attachments.Add(new Attachment("c:\\temp\\example.txt"));

            //to add additional attachments, simply call .Add(...) again
            mail.Attachments.Add(new Attachment("c:\\temp\\example2.txt"));
            mail.Attachments.Add(new Attachment("c:\\temp\\example3.txt"));

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);

        }

        static void AttachmentFromStream()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this content is in the body";

            //Get some binary data
            byte[] data = GetData();

            //save the data to a memory stream
            MemoryStream ms = new MemoryStream(data);

            //create the attachment from a stream. Be sure to name the data with a file and
            //media type that is respective of the data
            mail.Attachments.Add(new Attachment(ms, "example.txt", "text/plain"));

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);
        }
        static byte[] GetData()
        {
            //this method just returns some binary data.
            //it could come from anywhere, such as Sql Server
            string s = "this is some text";
            byte[] data = Encoding.ASCII.GetBytes(s);
            return data;
        }

        static void LoadFromConfig()
        {
            //the from address, along with the server properties will be set in the app.config,
            //thus we don't need to specify them in code

            //create the mail message
            MailMessage mail = new MailMessage();

            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            SmtpClient smtp = new SmtpClient();
            smtp.Send(mail);

        }

        static void Authenticate()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");

            //to authenticate we set the username and password properites on the SmtpClient
            smtp.Credentials = new NetworkCredential("username", "secret");
            smtp.Send(mail);

        }

        static void ChangePort()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");

            //to change the port (default is 25), we set the port property
            smtp.Port = 587;
            smtp.Send(mail);
        }

        static void EmbedImages()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";

            //first we create the Plain Text part
            AlternateView plainView = AlternateView.CreateAlternateViewFromString("This is my plain text content, viewable by those clients that don't support html", null, "text/plain");

            //then we create the Html part
            //to embed images, we need to use the prefix 'cid' in the img src value
            //the cid value will map to the Content-Id of a Linked resource.
            //thus <img src='cid:companylogo'> will map to a LinkedResource with a ContentId of 'companylogo'
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString("Here is an embedded image.<img src=cid:companylogo>", null, "text/html");

            //create the LinkedResource (embedded image)
            LinkedResource logo = new LinkedResource("c:\\temp\\logo.gif");
            logo.ContentId = "companylogo";
            //add the LinkedResource to the appropriate view
            htmlView.LinkedResources.Add(logo);

            //add the views
            mail.AlternateViews.Add(plainView);
            mail.AlternateViews.Add(htmlView);


            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1"); //specify the mail server address
            smtp.Send(mail);
        }

        static void SSL()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;
            int Port = smtpSection.Network.Port;
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //Port 587 is another SMTP port
            // SmtpClient smtp = new SmtpClient("127.0.01", Port);
            smtp.EnableSsl = true;
            smtp.Send(mail);
        }

        static void SendAsync()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;
            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1"); //specify the mail server address
            //the userstate can be any object. The object can be accessed in the callback method
            //in this example, we will just use the MailMessage object.
            object userState = mail;

            //wire up the event for when the Async send is completed
            smtp.SendCompleted += new SendCompletedEventHandler(SmtpClient_OnCompleted);

            smtp.SendAsync(mail, userState);
        }
        public static void SmtpClient_OnCompleted(object sender, AsyncCompletedEventArgs e)
        {
            //Get the Original MailMessage object
            MailMessage mail = (MailMessage)e.UserState;

            //write out the subject
            string subject = mail.Subject;

            if (e.Cancelled)
            {
                Console.WriteLine("Send canceled for mail with subject [{0}].", subject);
            }
            if (e.Error != null)
            {
                Console.WriteLine("Error {1} occurred when sending mail [{0}] ", subject, e.Error.ToString());
            }
            else
            {
                Console.WriteLine("Message [{0}] sent.", subject);
            }
        }

        public static void PickupDirectory()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //if we are using the IIS SMTP Service, we can write the message
            //directly to the PickupDirectory, and bypass the Network layer
            // SmtpClient smtp = new SmtpClient();
            smtp.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            smtp.Send(mail);
        }

        public static void EmailWebPage()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";

            //screen scrape the html
            string html = ScreenScrapeHtml("http://localhost/example.htm");
            mail.Body = html;
            mail.IsBodyHtml = true;

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);

        }
        public static string ScreenScrapeHtml(string url)
        {
            WebRequest objRequest = System.Net.HttpWebRequest.Create(url);
            StreamReader sr = new StreamReader(objRequest.GetResponse().GetResponseStream());
            string result = sr.ReadToEnd();
            sr.Close();
            return result;
        }

        public static void NonAsciiMail()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("anemr@ntmcs.com");

            //set the content
            mail.Subject = "This is an email";

            //to send non-ascii content, we need to set the encoding that matches the
            //string characterset.
            //In this example we use the ISO-8859-1 characterset
            mail.Body = "this text has some ISO-8859-1 characters: âÒÕÇ";
            mail.BodyEncoding = Encoding.GetEncoding("iso-8859-1");

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            smtp.Send(mail);

        }

        public static void InnerExceptions()
        {
            var smtp = new SmtpClient();
            var credential = (NetworkCredential)smtp.Credentials;
            string strHost = smtp.Host;
            int port = smtp.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;
            smtp.EnableSsl = true;

            var smtpSection = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");
            string EmailFrom = smtpSection.Network.UserName;

            //create the mail message
            MailMessage mail = new MailMessage();

            //set the addresses
            mail.From = new MailAddress(EmailFrom);
            mail.To.Add("him@hiscompany.com");

            //set the content
            mail.Subject = "This is an email";
            mail.Body = "this is the body content of the email.";

            //send the message
            //SmtpClient smtp = new SmtpClient("127.0.0.1");
            try
            {
                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                Exception ex2 = ex;
                string errorMessage = string.Empty;
                while (ex2 != null)
                {
                    errorMessage += ex2.ToString();
                    ex2 = ex2.InnerException;
                }

                Console.WriteLine(errorMessage);
            }
        }
    }
}
