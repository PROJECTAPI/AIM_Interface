using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using log4net;
using AIM_Interface.Models;

namespace AIM_Interface.clsbusiness
{

    public class IACUC
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string iacuc(int Card_ID, string Message)
        {
            string result;
            //   var connectionString = DbConnection.ConnectionString.GetConnection();

            try
            {

                Console.WriteLine("IACUC with Card Id: " + Card_ID + " Successfully Accessed");
                result = "Success";

                #region Send Email

                // clsEmail.PlainText();

                #endregion

                // int intValue = 0;
                //using (OracleConnection connection = new OracleConnection())
                //{
                //    connection.ConnectionString = connectionString;
                //    OracleCommand command = connection.CreateCommand();
                //    connection.Open();

                //    var commandText = "insert into APILOG (PK_APILOG,TRANS_LOG,D_API_B,T_API_B,API_SUCCES,PK_API,D_CREATE,T_CREATE,OP_CREATE)  (SELECT APILOG_SEQ.NEXTVAL,'" + DSC_API + "','" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','" + Status + "'," + PK_API + ",'" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','APIBATCH' FROM dual) ";

                //    using (OracleCommand command1 = new OracleCommand(commandText, connection))
                //    {
                //        int numberOfRecords = command1.ExecuteNonQuery();

                //        #region Get Last Inserted Record ID
                //        var strQuery = "SELECT APILOG_SEQ.CURRVAL FROM DUAL";
                //        OracleCommand objOracleCommand = new OracleCommand();
                //        objOracleCommand = new OracleCommand(strQuery, connection);
                //        intValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                //        #endregion
                //        PropertiesModel.LastPK_APILOGInserted = intValue;
                //       // Console.WriteLine(PropertiesModel.LastPK_APILOGInserted);
                //        result = "Success, Records Inserted : " + numberOfRecords + ", PK_APILOG : " + intValue + "";
                //    }
                //    connection.Close();
                //}

                //log.Info(result);

                //// DataLogUpdate.UpdateLog(intValue);
                return result;

            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
                result = "Error!";
                //result = "Log Record Insert has Failed";
                //log.Info("Log Record Insert has Failed : " + ex.Message);
                return result;

            }
        }
    }
}
