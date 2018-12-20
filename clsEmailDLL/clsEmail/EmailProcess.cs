using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Data.SQLite;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using apiCommonDLL;
using static AIM_Interface.Models.PropertiesModel;
using AIM_Interface.Models;

namespace clsEmailDLL
{

   public class EmailProcess
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Action finished = () =>
        {
            DataLogUpdate.UpdateLog();
            Console.WriteLine("Finished Sending Email");

        };
        public static string ProcessEmail(int pk_email, string Email_Recipients, string tableName, int ApiID)//Dictionary<string, object> QueryViewData
        {
            //Console.ReadLine();
            string result;
            List<Application_Email_Layouts> Layout = new List<Application_Email_Layouts>();
            List<object> QueryViewData = new List<object>();
            Dictionary<string, object> DataDictionary = new Dictionary<string, object>();

            try
            {
                Layout.Clear();
                // Console.WriteLine("Card Id: " + Card_ID + " Successfully De-Activated");
                result = "Success";


                #region QueryView Data from table to send data by email

                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand command = connection.CreateCommand();
                    connection.Open();
                    // log.Info(string.Format("State: {0}", connection.State));
                    // Console.WriteLine(string.Format("Email State: {0}", connection.State));

                    string sqlData = "SELECT * FROM " + tableName + "";
                    command.CommandText = sqlData;

                    OracleDataReader readerData = command.ExecuteReader(CommandBehavior.SingleRow);

                    while (readerData.Read())
                    {
                        DataDictionary = Enumerable.Range(0, readerData.FieldCount).ToDictionary(readerData.GetName, readerData.GetValue);//readerData.FieldCount replace 1 to get all the records
                        QueryViewData.Add(DataDictionary);
                    }
                }

                #endregion

                #region Process Send Email



                using (SQLiteConnection SQLconnect = new SQLiteConnection())
                {


                    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                    SQLconnect.Open();
                    Console.WriteLine("Metdata Connection State: {0}", SQLconnect.State);


                    using (SQLiteCommand fmd = SQLconnect.CreateCommand())
                    {
                        fmd.CommandText = @"SELECT * FROM APPLICATION_EMAIL_LAYOUTS where PK_EMAIL = " + pk_email + "";
                        SQLiteDataReader r = fmd.ExecuteReader();

                        while (r.Read())
                        {
                            Layout.Add(new Application_Email_Layouts()
                            {
                                //string FileNames = (string)r["EMAIL_MESSAGE"];

                                PK_EMAIL = r.GetInt32(r.GetOrdinal("PK_EMAIL")),
                                EMAIL_TP_CONTENT = r.IsDBNull(r.GetOrdinal("EMAIL_TP_CONTENT")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_TP_CONTENT")),
                                EMAIL_SUBJECT = r.IsDBNull(r.GetOrdinal("EMAIL_SUBJECT")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_SUBJECT")),
                                EMAIL_RECIPIENTS = r.IsDBNull(r.GetOrdinal("EMAIL_RECIPIENTS")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_RECIPIENTS")),
                                EMAIL_COPY_RQSTR = r.IsDBNull(r.GetOrdinal("EMAIL_COPY_RQSTR")) ? 0 : r.GetInt32(r.GetOrdinal("EMAIL_COPY_RQSTR")),
                                EMAIL_COPY_OWNR = r.IsDBNull(r.GetOrdinal("EMAIL_COPY_OWNR")) ? 0 : r.GetInt32(r.GetOrdinal("EMAIL_COPY_OWNR")),
                                EMAIL_ADTL_CC = r.IsDBNull(r.GetOrdinal("EMAIL_ADTL_CC")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_ADTL_CC")),
                                EMAIL_BLIND_CC = r.IsDBNull(r.GetOrdinal("EMAIL_BLIND_CC")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_BLIND_CC")),
                                EMAIL_SHOW_DNR = r.IsDBNull(r.GetOrdinal("EMAIL_SHOW_DNR")) ? 0 : r.GetInt32(r.GetOrdinal("EMAIL_SHOW_DNR")),
                                EMAIL_ATTACH_FLG = r.IsDBNull(r.GetOrdinal("EMAIL_ATTACH_FLG")) ? 0 : r.GetInt32(r.GetOrdinal("EMAIL_ATTACH_FLG")),
                                APP_OPT_ID = r.IsDBNull(r.GetOrdinal("APP_OPT_ID")) ? 0 : r.GetInt32(r.GetOrdinal("APP_OPT_ID")),
                                EMAIL_CUSTOM = r.IsDBNull(r.GetOrdinal("EMAIL_CUSTOM")) ? 0 : r.GetInt32(r.GetOrdinal("EMAIL_CUSTOM")),
                                EMAIL_LAYOUT_NM = r.IsDBNull(r.GetOrdinal("EMAIL_LAYOUT_NM")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_LAYOUT_NM")),
                                APP_ID = r.IsDBNull(r.GetOrdinal("APP_ID")) ? 0 : r.GetInt32(r.GetOrdinal("APP_ID")),
                                EMAIL_DSC_LAYOUT = r.IsDBNull(r.GetOrdinal("EMAIL_DSC_LAYOUT")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_DSC_LAYOUT")),
                                EMAIL_MESSAGE = r.IsDBNull(r.GetOrdinal("EMAIL_MESSAGE")) ? String.Empty : r.GetString(r.GetOrdinal("EMAIL_MESSAGE"))

                            });
                        }
                        // AllData.Add(Email_Subject);
                        //  Console.WriteLine(Layout);




                        foreach (var itemRecord in Layout)
                        {
                            // Console.WriteLine(itemRecord.EMAIL_MESSAGE);


                            string str = itemRecord.EMAIL_MESSAGE; //"This is a test of the new "+EMailModel.Name+" email system on "+DateTime.Now+", hi zouzou "+EMailModel.Last+"";
                                                                   //string strResult = "";
                                                                   //string parameters = "";

                            //  Console.WriteLine(str);
                            string BodyMessage = "";
                            //string str2= str.Replace("/~", "/+").Replace;
                            // var matches = "";//Working:::::::::: string.Format(result, name, DateTime.Now); //"This is a test of the new " + EMailModel.Name + " email system on " + DateTime.Now + "";

                            var reg = new Regex("~{.*?}~");
                            var matches = reg.Matches(str);



                            List<object> a = new List<object>();
                            //  Dictionary<string, string> dictionary = new Dictionary<string, string>();
                            for (int i = 0; i < matches.Count; i++)
                            {

                                var value = matches[i];

                                // Console.WriteLine(value);

                                if (value.Value.ToLower().Contains("date"))
                                {
                                    str = str.Replace(value.Value, "{" + i + ":t}");
                                }
                                else
                                {

                                    str = str.Replace(value.Value, "{" + i + "}");
                                }

                                // Console.WriteLine(str);





                                /////////////// To Grab the uSER Role
                                /////////////// /////////////// /////////////// ///////////////if (userSession.User != null)
                                /////////////// /////////////// /////////////// /////////////// {
                                //SYSUSESS session = userSession.Session;
                                //if (session != null)
                                //{
                                //    //var dashBoardIds = userSession.User.SYSWALRT
                                //    //   .Where(ua => ua.PK_SYSROLE == session.PK_SYSROLE && ua.PK_SITE == session.PK_SITE && ua.APP_ID == id)
                                //    //   .Select(sa => sa.PK_ALERT);

                                //    Console.WriteLine(session);


                                //}



                                /////////////// /////////////// /////////////// /////////////// SYSUSESS session = userSession.Session;

                                /////////////// /////////////// /////////////// /////////////// IList<SYSWURL> userRoles = await _userSiteRoleRepo.GetByUserAndSiteId(session.PK_WUSR.GetValueOrDefault(), session.PK_SITE.GetValueOrDefault());
                                // SYSWURL newRole = userRoles.FirstOrDefault(ur => ur.PK_SYSROLE == model.RoleId);

                                //if (newRole != null)
                                //{
                                //    session.PK_SYSROLE = newRole.PK_SYSROLE;
                                //    await _sessionRepo.UpdateAsync(session);

                                //   // return await GetTopBar();
                                //}
                                //else
                                //{

                                //  session.PK_SYSROLE = newRole.PK_SYSROLE;
                                //  return new ResultWithErrors("Cannot change role");
                                // }
                                //return new ResultWithErrors("Invalid session");
                                /////////////// /////////////// /////////////// /////////////// Console.WriteLine(session.PK_SYSROLE);
                                /////////////// /////////////// /////////////// ///////////////  }

                                ///////////////////////// Fill The Values as Strings After Loading Them

                                List<EmailProperties> Props = new List<EmailProperties>();

#pragma warning disable CS0219 // The variable 'EMailModel' is assigned but its value is never used
                                var EMailModel = "";
#pragma warning restore CS0219 // The variable 'EMailModel' is assigned but its value is never used
                                var item = value.Value.Replace("~{", "").Replace("}~", "");
                                //strResult += strResult;
                                // var ModelValues = "";

                                // string ModelValuesDate;
                                bool FoundInQueryView = false;

                                foreach (var items in DataDictionary)
                                {

                                    if (items.Key == item)
                                    {
                                        a.Add(items.Value);
                                        FoundInQueryView = true;
                                        break;
                                    }
                                }


                                if (item.ToLower() == "date")
                                {

                                    object value1 = DateTime.Now.ToString("f");
                                    a.Add(value1);


                                }
                                else if (item.ToLower() == "userid")
                                {
                                    object value1 = "";//userSession.User.USR_ID.Trim();
                                    a.Add(value1);

                                }
                                else if (typeof(EmailProperties).GetProperty(item) != null)
                                {

                                    Console.WriteLine(" Property Found");



                                    // PropertyInfo pi = Props.GetType().GetProperty(item);
                                    object value1 = (object)(typeof(EmailProperties).GetProperty(item).GetValue(typeof(EmailProperties), null));// (object)(pi.GetValue(Props, null));

                                    a.Add(value1);

                                }
                                //else if (typeof(EmailProperties).GetProperty(item) == null)
                                //{


                                //}
                                else if (typeof(EmailProperties).GetProperty(item) == null && FoundInQueryView == false)
                                {
                                    object value1 = "// ERROR: Variable " + item + " not found ! //";
                                    a.Add(value1);
                                }
                                //////////else if (typeof(ACCOUNTS).GetProperty(item) != null)
                                //////////{

                                //////////    Console.WriteLine("" + item + "  Property  Found in Accounts Class");


                                //////////    ACCOUNTS AccData = _siriusDb.ACCOUNTS.FirstOrDefault(cs => cs.RESP_ID == 1041);
                                //////////    ////ACCOUNTS response = new ACCOUNTS();
                                //////////    ////response.ACCOUNT_NO = AccData.ACCOUNT_NO;


                                //////////    ///////////////////// Check if it is a field in a table

                                //////////    ////if (_siriusDb.ACCOUNTS.Any(o => o.ACCOUNT_NO == idToMatch))
                                //////////    ////{
                                //////////    ////    // Match!
                                //////////    ////}




                                //////////    ////if (item.ToLower() == "account_no")
                                //////////    ////{

                                //////////    PropertyInfo pi = AccData.GetType().GetProperty(item);
                                //////////    object value1 = (object)(pi.GetValue(AccData, null));

                                //////////    a.Add(value1);
                                //////////    ////Type t = pi.PropertyType;
                                //////////    ////Console.WriteLine(t);

                                //////////    ////////if (pi.PropertyType == typeof(string))
                                //////////    ////////{
                                //////////    ////////    string value1 = (string)(pi.GetValue(AccData, null));
                                //////////    ////////    a.Add(value1);
                                //////////    ////////} else {
                                //////////    ////////////object value1 = (object)(pi.GetValue(AccData, null));

                                //////////    ////////////a.Add(value1);
                                //////////    /////////////////////////////}



                                //////////    // ModelValues = AccData.ACCOUNT_NO.Trim();


                                //////////    /////  Console.WriteLine(ModelValues);
                                //////////}

                            }


                            var resultt = a.Cast<object>().ToArray();
                            BodyMessage = string.Format(str, resultt);


                            //////////////////////////////if (ModelState.IsValid)
                            //////////////////////////////{

#pragma warning disable CS0436 // The type 'SMTP' in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs' conflicts with the imported type 'SMTP' in 'AIM_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs'.
#pragma warning disable CS0436 // The type 'SMTP' in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs' conflicts with the imported type 'SMTP' in 'AIM_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs'.
                            SMTP smtp = new SMTP();
#pragma warning restore CS0436 // The type 'SMTP' in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs' conflicts with the imported type 'SMTP' in 'AIM_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs'.
#pragma warning restore CS0436 // The type 'SMTP' in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs' conflicts with the imported type 'SMTP' in 'AIM_Interface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. Using the type defined in 'C:\inetpub\wwwroot\AIM_Interface\clsEmailDLL\clsEmail\clsEmail.cs'.
                            string FromEmail = "support@ntmcs.com";


                            string mailReturn = smtp.Send(FromEmail, itemRecord.APP_ID, itemRecord.EMAIL_SUBJECT, itemRecord.EMAIL_TP_CONTENT, BodyMessage, itemRecord.PK_EMAIL, itemRecord.EMAIL_COPY_RQSTR, itemRecord.EMAIL_COPY_OWNR, itemRecord.EMAIL_LAYOUT_NM, itemRecord.APP_OPT_ID, itemRecord.EMAIL_ADTL_CC, itemRecord.EMAIL_BLIND_CC, itemRecord.EMAIL_DSC_LAYOUT, itemRecord.EMAIL_SHOW_DNR, itemRecord.EMAIL_ATTACH_FLG, itemRecord.EMAIL_CUSTOM, Email_Recipients);

                        }


                    }
                    #endregion
                    SQLconnect.Close();

                }
                return result;

            }
            catch (Exception ex)
            {
                result = "Error!";
                //result = "Log Record Insert has Failed";
                log.Info("Fetching metadata Message Field Failed : " + ex.Message);
                #region Creating Log Record After Failure in Inserting all Records
                DataLogInsert.InsertLog("Error, Sending email Failed : " + ex.Message + "", ApiID, "No");
                #endregion
                // Console.WriteLine("Fetching metadata Message Field Failed : " + ex.Message);
                return result;

            }
            finally
            {
                finished();
            }
        }
    }
}
