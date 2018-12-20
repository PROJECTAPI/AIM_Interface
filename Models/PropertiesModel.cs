using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Globalization;
using System.Text.RegularExpressions;

namespace AIM_Interface.Models
{
    public class ConnectionProperties
    {

        public static string connectionString = DbConnection.ConnectionString.GetConnection();
        public static string connectionStringMetadata = DbConnection.ConnectionString.GetMetadataConnection();

        public static string _connectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }
        public static string _connectionStringMetadata
        {
            get { return connectionStringMetadata; }
            set { connectionStringMetadata = value; }
        }

    }
    public class PropertiesModel
    {
        public bool quitNow { get; set; } = false;
        public static bool IsMainAPiLogvalue;
        public int ParameterApiID { get; set; } = 0;
        private static int myInt;
        private static int PK_APIValue;
        private static int MainPK_APILogValue;
        private static int myIntDETails;
        private static int CardID;
        private static string Error;
        private static string LogDetailsValue;
        private static string LogDetailsStatusValue;
        private static string Email_Subject;
        private static string Email_Message;
        private static string CSVFILENAME;
        private static string PrimaryKey;
        private static string TableName;
        private static string NextSequenceName;
        private static string QueryViewTableName;

        public static int PK_API
        {
            get { return PK_APIValue; }
            set { PK_APIValue = value; }
        }
        public static int MainPK_APILog
        {
            get { return MainPK_APILogValue; }
            set { MainPK_APILogValue = value; }
        }

        public static string LogDetailsMessage
        {
            get { return LogDetailsValue; }
            set { LogDetailsValue = value; }
        }
        public static bool IsMainAPiLog
        {
            get { return IsMainAPiLogvalue; }
            set { IsMainAPiLogvalue = value; }
        }


        public static string LogDetailsStatus
        {
            get { return LogDetailsStatusValue; }
            set { LogDetailsStatusValue = value; }
        }

        public static string Next_Sequence_Name
        {
            get { return NextSequenceName; }
            set { NextSequenceName = value; }
        }
        public static string TargetTable_Name
        {
            get { return TableName; }
            set { TableName = value; }
        }
        public static string QueryView_TableName
        {
            get { return QueryViewTableName; }
            set { QueryViewTableName = value; }
        }
        public static string CSV_FILE_NAME
        {
            get { return CSVFILENAME; }
            set { CSVFILENAME = value; }
        }
        public static string Primary_Key
        {
            get { return PrimaryKey; }
            set { PrimaryKey = value; }
        }
        public static int LastPK_APILOGInserted
        {
            get { return myInt; }
            set { myInt = value; }
        }
        public static int LastPK_APLDETLInserted
        {
            get { return myIntDETails; }
            set { myIntDETails = value; }
        }
        public static int Card_ID
        {
            get { return CardID; }
            set { CardID = value; }
        }
        public static string Message
        {
            get { return Error; }
            set { Error = value; }
        }
        public static string EmailSubject
        {
            get { return Email_Subject; }
            set { Email_Subject = value; }
        }
        public static string EmailMessage
        {
            get { return Email_Message; }
            set { Email_Message = value; }
        }
        public static string Appid { get; set; }

        public static string App_id
        {
            get { return Appid; }
            set { Appid = value; }
        }
        public static int Pkemail { get; set; }

        public static int Pk_email
        {
            get { return Pkemail; }
            set { Pkemail = value; }
        }
        public static string RecipientV { get; set; }

        public static string Email_Recipients
        {
            get { return RecipientV; }
            set { RecipientV = value; }
        }
        public static string CopyRequester { get; set; }
        public static string Copy_Requester
        {
            get { return CopyRequester; }
            set { CopyRequester = value; }
        }
        public static string CopyOwner { get; set; }

        public static string Copy_Owner
        {
            get { return CopyOwner; }
            set { CopyOwner = value; }
        }
        public static string Layoutname { get; set; }
        public static string Layout_name
        {
            get { return Layoutname; }
            set { Layoutname = value; }
        }
        public static string Appoptid { get; set; }
        public static string App_opt_id
        {
            get { return Appoptid; }
            set { Appoptid = value; }
        }
        public static string ContentType { get; set; }
        public static string Content_Type
        {
            get { return ContentType; }
            set { ContentType = value; }
        }
        public static string AdtlCC { get; set; }
        public static string Adtl_CC
        {
            get { return AdtlCC; }
            set { AdtlCC = value; }
        }
        public static string BlindCC { get; set; }

        public static string Blind_CC
        {
            get { return BlindCC; }
            set { BlindCC = value; }
        }
        public static string DisplayLayout { get; set; }
        public static string Display_Layout
        {
            get { return DisplayLayout; }
            set { DisplayLayout = value; }
        }
        public static string Showdnr { get; set; }
        public static string Show_dnr
        {
            get { return Showdnr; }
            set { Showdnr = value; }
        }
        public static string AttachmentV { get; set; }
        public static string Attachment
        {
            get { return AttachmentV; }
            set { AttachmentV = value; }
        }
        public static string CustomV { get; set; }
        public static string Custom
        {
            get { return CustomV; }
            set { CustomV = value; }
        }

        public class RegexUtilities
        {
            bool invalid = false;

            public bool IsValidEmail(string strIn)
            {
                invalid = false;
                if (String.IsNullOrEmpty(strIn))
                    return false;

                // Use IdnMapping class to convert Unicode domain names.
                try
                {
                    strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }

                if (invalid)
                    return false;

                // Return true if strIn is in valid e-mail format.
                try
                {
                    return Regex.IsMatch(strIn,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
            private string DomainMapper(Match match)
            {
                // IdnMapping class with default property values.
                IdnMapping idn = new IdnMapping();

                string domainName = match.Groups[2].Value;
                try
                {
                    domainName = idn.GetAscii(domainName);
                }
                catch (ArgumentException)
                {
                    invalid = true;
                }
                return match.Groups[1].Value + domainName;
            }
        }



    }

    public class EmailProperties
    {
        /// <summary>
        /// ////////////////////////////////////////////// eSirius Email properties can be set after class object is created.
        /// </summary>
        public bool em_Asynchronous { get; set; } = true; // True send email asynchronously, false send email synchronously
        public string em_Username { get; set; } = ""; // Authentication user name
        public string em_Password { get; set; } = ""; // Authentication password
        public bool em_UseSsl { get; set; } = false; // True secure connection using SSL/TLS, false no SSL/TSL.  Requires .NET Mail Mode
        public bool em_Debug { get; set; } = true; // Save email SMTP properties to a debug file.
        public string em_SenderName { get; set; } = "eSirius3GWebServer"; // The display name of the sender that displays in the mail client's
        public string em_StartDir { get; set; } = System.IO.Directory.GetCurrentDirectory(); // for debug file
        public string App_id { get; set; }
        public static string _Pk_email { get; set; }
        public static string Pk_email
        {
            get { return _Pk_email; }
            set { _Pk_email = value; }
        }
        public static string _Subject { get; set; } = "dsdsd";
        public static string Subject
        {
            get { return _Subject; }
            set { _Subject = value; }
        }
        public string Message { get; set; }
        public string Recipient { get; set; }
        public string Copy_Requester { get; set; }
        public string Copy_Owner { get; set; }
        public string Layout_name { get; set; }
        public string App_opt_id { get; set; }
        public string Content_Type { get; set; }
        //public string Pk_mdform { get; set; }
        //public string email_type { get; set; }
        public string Adtl_CC { get; set; }
        public string Blind_CC { get; set; }
        public string Display_Layout { get; set; }
        public string Show_dnr { get; set; }
        public string Attachment { get; set; }
        public string Custom { get; set; }
        //   public DateTime Date { get; set; }
        public int UserID { get; set; }
        public string SessionVariable { get; set; }


    }
}