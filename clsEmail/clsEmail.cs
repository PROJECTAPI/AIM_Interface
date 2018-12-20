using System.Net.Mail;
using System;
using System.Net;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using static AIM_Interface.Models.PropertiesModel;
using System.Data;
using System.Reflection;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.IO;

public class SMTP
{
    public static string StripHTML(string input)
    {
        return Regex.Replace(input, "<.*?>", String.Empty);
    }
    public static string GetStringSafe(IDataReader reader, int colIndex, string defaultValue)
    {
        if (!reader.IsDBNull(colIndex))
            return reader.GetString(colIndex);
        else
            return defaultValue;
    }

    public static List<T> DataReaderMapToList<T>(IDataReader dr)
    {
        List<T> list = new List<T>();
        T obj = default(T);
        while (dr.Read())
        {
            obj = Activator.CreateInstance<T>();
            foreach (PropertyInfo prop in obj.GetType().GetProperties())
            {
                if (!object.Equals(dr[prop.Name], DBNull.Value))
                {
                    prop.SetValue(obj, dr[prop.Name], null);
                }
            }
            list.Add(obj);
        }
        return list;
    }
    public static List<T> MapToList<T>(DbDataReader dr) where T : new()
    {
        List<T> RetVal = null;
        var Entity = typeof(T);
        var PropDict = new Dictionary<string, PropertyInfo>();
        try
        {
            if (dr != null && dr.HasRows)
            {
                RetVal = new List<T>();
                var Props = Entity.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                PropDict = Props.ToDictionary(p => p.Name.ToUpper(), p => p);
                while (dr.Read())
                {
                    T newObject = new T();
                    for (int Index = 0; Index < dr.FieldCount; Index++)
                    {
                        if (PropDict.ContainsKey(dr.GetName(Index).ToUpper()))
                        {
                            var Info = PropDict[dr.GetName(Index).ToUpper()];
                            if ((Info != null) && Info.CanWrite)
                            {
                                var Val = dr.GetValue(Index);
                                Info.SetValue(newObject, (Val == DBNull.Value) ? null : Val, null);
                            }
                        }
                    }
                    RetVal.Add(newObject);
                }
            }
        }
        catch (Exception)
        {
            throw;
        }
        return RetVal;
    }
    public string Send(string From, int App_id, string Subject, string Content_Type, string Message, int Pk_email, int Copy_Requester, int Copy_Owner, string Layout_name, int App_opt_id, string Adtl_CC, string Blind_CC, string Display_Layout, int Show_dnr, int Attachment, int Custom, string email_recipients)
    {

        try
        {
            RegexUtilities util = new RegexUtilities();


            string to = email_recipients;// "anemr@ntmcs.com";
            string fromDNR = "DO-NOT-REPLY@ntmcs.com";//"support@ntmcs.com";

            MailMessage mail = new MailMessage(From, to.Replace(";", ","));
            //Console.WriteLine(Show_dnr);
            if (Show_dnr == -1)
            {
                mail.From = new MailAddress(fromDNR); //From DO-NOT-REPLY
            }
            else
            {
                mail.From = new MailAddress(From); //From Email Id

            }

            mail.Subject = Subject;



            if (Content_Type.ToLower() == "html")
            {




                string logoImage = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../images/eSirius_Logo.png"));
                Attachment Logo = new Attachment(logoImage);
                mail.Attachments.Add(Logo);

                string contentID = "LogoImage";
                Logo.ContentId = contentID;


                //string backgroundImage = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../images/background-new-centered.jpg"));
                //Attachment background = new Attachment(backgroundImage);
                //mail.Attachments.Add(background);

                //string contentIDBG = "backgroundImage";
                //background.ContentId = contentIDBG;




                // StringBuilder tosend = new StringBuilder();

                string tosend = "";

                tosend = tosend + "<!DOCTYPE html PUBLIC ' -//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'><html xmlns='http://www.w3.org/1999/xhtml'>";
                tosend = tosend + "<head>";
                tosend = tosend + "<meta http-equiv='Content-Type' content='text / html; charset=utf-8' />";
                tosend = tosend + "<meta charset='UTF-8'>";
                tosend = tosend + "<meta name='viewport' content='width=device-width, initial-scale=1'>";
                tosend = tosend + "<link href='https://fonts.googleapis.com/css?family=Lato:400,700,300' rel='stylesheet' type='text/css'>";
                tosend = tosend + "<link href='http://www.ntmcs.com/css/master.css' rel='stylesheet'>";
                tosend = tosend + "<style>";
                tosend = tosend + "#containerbg";
                tosend = tosend + "{background-image:url(images/background-new-centered.jpg) !important; background-repeat: no-repeat; width: 100%; height: 900px; }";
                tosend = tosend + " </style>";
                tosend = tosend + "</head>";
                tosend = tosend + "<body style='padding-top:0px !important;  background-size:cover; '>";
                //  tosend = tosend + "<div style='z-index:0'><img src=\"cid:" + contentIDBG + "\" style='height:100%; width:100%;' />";
                tosend = tosend + "<table width='100%' border='0' id='containerbg' cellspacing='2' cellpadding='2'  style='vertical-align:top; margin-top:0px !important; background: url('images/background-new-centered.jpg') top left repeat-y'>";
                tosend = tosend + "<tr>";
                tosend = tosend + "<td align='center' height='80px' style='background-color:#FFF; padding:15px'>";
                tosend = tosend + "<img src=\"cid:" + contentID + "\" width='130' height='43' alt='Esirius3G' /></td> ";
                tosend = tosend + "</tr>";
                tosend = tosend + "<tr>";
                tosend = tosend + "<td style='padding:20px; font-family:Verdana; font-size:13px; height:500px !important' height='500px'>" + Message + "</td>";
                tosend = tosend + "</tr>";
                tosend = tosend + "<tr>";
                tosend = tosend + "<td align='center' height='90px' style='background-color:#fff; padding:10px; font-size:11px; font-family:Verdana, Geneva, sans-serif'>";
                tosend = tosend + " <table width='100%' border='0' cellspacing='2' cellpadding='2' style='font-family:verdana; font-size:11px'>";
                tosend = tosend + "<tr>";
                tosend = tosend + "<td style='width:25%' align='center'>";
                tosend = tosend + "<div style='width:100%;float:left'>";
                tosend = tosend + "NTM Consulting Services, Inc.<br />";
                tosend = tosend + "39300 Civic Center Dr.<br />";
                tosend = tosend + "Suite 250 <br />";
                tosend = tosend + "Fremont, CA 94538";
                tosend = tosend + "</div></td>";
                tosend = tosend + "<td style='width:25%' align='center' >";
                tosend = tosend + "<div style='width:100%;float:left'> Sales and Product Information <br />";
                tosend = tosend + "1.888.eSirius <br />";
                tosend = tosend + "<a href='mailto:esirius@ntmcs.com'> esirius@ntmcs.com </a></div></td>";
                tosend = tosend + "<td style='width:25%' align='center'>";
                tosend = tosend + "<div style='width:100%;float:left'> Technical Support <br />";
                tosend = tosend + "1.877.535.4686 <br />";
                tosend = tosend + "<a href='mailto:ntmsupport@ntmcs.com'>ntmsupport@ntmcs.com </a></div></td>";
                tosend = tosend + "<td style='width:25%' align='center' >";
                tosend = tosend + "<div style='width:100%;float:left'> General Administrative <br />";
                tosend = tosend + "1.510.744.3901 <br />";
                tosend = tosend + "<a href='mailto:ntmadmin@ntmcs.com'>ntmadmin@ntmcs.com </a>";
                tosend = tosend + "</div>";
                tosend = tosend + " </td>";
                tosend = tosend + "  </tr>";
                tosend = tosend + "</table>";
                tosend = tosend + "</td>";
                tosend = tosend + "</tr>";
                tosend = tosend + "</table>";
                tosend = tosend + "</body>";
                tosend = tosend + "</html>";


                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;

                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(tosend, null, MediaTypeNames.Text.Html);
                htmlView.ContentType = new System.Net.Mime.ContentType("text/html");
                mail.AlternateViews.Add(htmlView);


                mail.Body = htmlView.ToString();
                mail.IsBodyHtml = true;
            }
            else
            {

                mail.Body = StripHTML(Message);
                mail.IsBodyHtml = false;
            }


            string file = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../images/eSirius_Logo.png"));
            string file1 = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../images/background-new-centered.jpg"));

            try
            {
                Attachment data = new Attachment(file);
                mail.Attachments.Add(data);
                Attachment data1 = new Attachment(file1);
                mail.Attachments.Add(data1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}", ex.ToString());
            }

            // Add time stamp information for the file.
            ////ContentDisposition disposition = data.ContentDisposition;
            ////disposition.CreationDate = System.IO.File.GetCreationTime(file);
            ////disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
            ////disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
            // Add the file attachment to this e-mail message.



            ////////// Load SMTP Settings form Web.Config File
            var smtpMail = new SmtpClient();
            var credential = (NetworkCredential)smtpMail.Credentials;
            string strHost = smtpMail.Host;
            int port = smtpMail.Port;
            string strUserName = credential.UserName;
            string strFromPass = credential.Password;


            smtpMail.EnableSsl = true;

            //  mail.CC.Add(TO_addressList.ToString());



            ///////////// MailAddressCollection TO_addressList = new MailAddressCollection();

            string AllCCEmails = Adtl_CC;
            string AllBlindEmails = Blind_CC;

            //2.Prepare the CC Email Addresses list in Case there is Any
            if (!string.IsNullOrEmpty(AllCCEmails))
            {
                foreach (var CCEmails in AllCCEmails.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Console.Write(CCEmails);
                    if (util.IsValidEmail(CCEmails))
                    {
                        mail.CC.Add(new MailAddress(CCEmails));
                        Console.WriteLine("Valid: {0}", CCEmails);
                    }
                    else
                    {

                        Console.WriteLine("Invalid: {0}", CCEmails);
                    }

                }
                smtpMail.Send(mail);

            }

            //3.Prepare the BCC Email Addresses list in Case there is Any
            if (!string.IsNullOrEmpty(AllBlindEmails))
            {
                mail.CC.Clear();
                foreach (var BCCEmails in AllBlindEmails.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                {
                    // Console.Write(CCEmails);
                    if (util.IsValidEmail(BCCEmails))
                    {
                        mail.Bcc.Add(new MailAddress(BCCEmails));
                        //Console.WriteLine("Valid: {0}", BCCEmails);
                    }
                    else
                    {
                        Console.WriteLine("Invalid: {0}", BCCEmails);
                    }
                }

                smtpMail.Send(mail);

            }

            if (string.IsNullOrEmpty(AllCCEmails) && string.IsNullOrEmpty(AllBlindEmails))
            {
                // mail.To.Add(new MailAddress(to));


                Console.WriteLine("start to send email with attachment ...");

                try
                {
                    smtpMail.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in CreateMessageWithAttachment(): {0}",
                                ex.ToString());
                }
                // Display the values in the ContentDisposition for the attachment.
                //ContentDisposition cd = data.ContentDisposition;
                //Console.WriteLine("Content disposition");
                //Console.WriteLine(cd.ToString());
                //Console.WriteLine("File {0}", cd.FileName);




            }



        }
        catch (Exception e)
        {
            return e.Message;
        }

        return "success";

    }
}