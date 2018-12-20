using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using log4net;
using AIM_Interface.Models;
using System.Linq;

namespace AIM_Interface.Common
{

    public class DataLogInsert
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string InsertLog(string DSC_API, int PK_API, string Status)
        {
            string result;
            int NextValue = 0;

            try
            {

                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand command = connection.CreateCommand();
                    connection.Open();


                    #region Get Next PK_APILOG Sequence ID

                    var strQuery = "SELECT APILOG_SEQ.NEXTVAL FROM DUAL";
                    OracleCommand objOracleCommand = new OracleCommand();
                    objOracleCommand = new OracleCommand(strQuery, connection);
                    NextValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                    PropertiesModel.LastPK_APILOGInserted = NextValue;

                    #endregion

                    var commandText = Services.GetDataTable("Select * from APILOG where PK_APILOG =" + NextValue + "");

                    #region Convert Datatable to An Array to get the columns names dynamically
                    var columnNames = commandText.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName.Replace("\r", ""))
                                 .ToArray();
                    #endregion

                    //for (int g = 0; g <= 3; g++)
                    //{

                    #region set a primary key column

                    DataColumn[] columns = new DataColumn[1];
                    columns[0] = commandText.Columns["PK_APILOG"];
                    commandText.PrimaryKey = columns;

                    #endregion set a primary key column

                    #region Give New Values to some fields in the datatable

                    DataSet myDataset = new DataSet();

                    DataRow newCustomersRow = commandText.NewRow();

                    newCustomersRow["PK_APILOG"] = NextValue;
                    newCustomersRow["TRANS_LOG"] = DSC_API;
                    newCustomersRow["D_API_B"] = DateTime.Now.ToString("dd/MMM/yyyy");
                    newCustomersRow["T_API_B"] = DateTime.Now.ToLocalTime().ToString("h:mm:ss");
                    newCustomersRow["API_SUCCES"] = Status;
                    newCustomersRow["PK_API"] = PK_API;
                    newCustomersRow["D_CREATE"] = DateTime.Now.ToString("dd/MMM/yyyy");
                    newCustomersRow["T_CREATE"] = DateTime.Now.ToLocalTime().ToString("h:mm:ss");
                    newCustomersRow["OP_CREATE"] = "APIBATCH";
                    newCustomersRow["OP_TRANS"] = "APIBATCH";

                    commandText.Rows.Add(newCustomersRow);

                    // }

                    #endregion


                    Services.BulkInsertDataLog(commandText, Services.ConvertStringArrayToStringJoin(columnNames), "APILOG");


                    #region insert log record without using Bulk Insert Method
                    //////var commandText = "insert into APILOG (PK_APILOG,TRANS_LOG,D_API_B,T_API_B,API_SUCCES,PK_API,D_CREATE,T_CREATE,OP_CREATE)  (SELECT APILOG_SEQ.NEXTVAL,'" + DSC_API + "','" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','" + Status + "'," + PK_API + ",'" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','APIBATCH' FROM dual) ";

                    //using (OracleCommand command1 = new OracleCommand(commandText, connection))
                    //{
                    //    int numberOfRecords = command1.ExecuteNonQuery();

                    //    #region Get Last Inserted Record ID
                    //    var strQuery = "SELECT APILOG_SEQ.CURRVAL FROM DUAL";
                    //    OracleCommand objOracleCommand = new OracleCommand();
                    //    objOracleCommand = new OracleCommand(strQuery, connection);
                    //    intValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                    //    #endregion
                    //    PropertiesModel.LastPK_APILOGInserted = intValue;
                    //   // Console.WriteLine(PropertiesModel.LastPK_APILOGInserted);
                    //    result = "Success, Records Inserted : " + numberOfRecords + ", PK_APILOG : " + intValue + "";
                    //}
                    #endregion

                    result = "Success, Log Records Inserted : , PK_APILOG : " + PropertiesModel.LastPK_APILOGInserted + "";//numberOfRecords

                    connection.Close();
                }

                // log.Info(result);

                // DataLogUpdate.UpdateLog(intValue);
                return result;

            }
            catch (Exception ex)
            {
                result = "Log Record Insert has Failed";
                log.Info("Log Record Insert has Failed : " + ex.Message);
                return result;

            }

        }
        public static string InsertLogDetails(string DSC_API, int PK_API, string Status)
        {
            string result;
            int NextValue = 0;

            try
            {

                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand command = connection.CreateCommand();
                    connection.Open();


                    #region Get Next PK_APILOG Sequence ID

                    var strQuery = "SELECT APILDETL_SEQ.NEXTVAL FROM DUAL";
                    OracleCommand objOracleCommand = new OracleCommand();
                    objOracleCommand = new OracleCommand(strQuery, connection);
                    NextValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                    PropertiesModel.LastPK_APLDETLInserted = NextValue;

                    #endregion

                    var commandText = Services.GetDataTable("Select * from APILDETL where PK_APLDETL =" + NextValue + "");

                    #region Convert Datatable to An Array to get the columns names dynamically
                    var columnNames = commandText.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName.Replace("\r", ""))
                                 .ToArray();
                    #endregion

                    //for (int g = 0; g <= 3; g++)
                    //{

                    #region set a primary key column

                    DataColumn[] columns = new DataColumn[1];
                    columns[0] = commandText.Columns["PK_APLDETL"];
                    commandText.PrimaryKey = columns;

                    #endregion set a primary key column

                    #region Give New Values to some fields in the datatable

                    DataSet myDataset = new DataSet();

                    DataRow newCustomersRow = commandText.NewRow();

                    newCustomersRow["PK_APLDETL"] = NextValue;
                    newCustomersRow["DETL_LOG"] = DSC_API;
                   // newCustomersRow["D_API_B"] = DateTime.Now.ToString("dd/MMM/yyyy");
                   // newCustomersRow["T_API_B"] = DateTime.Now.ToLocalTime().ToString("h:mm:ss");
                    newCustomersRow["TP_LOG"] = Status;
                    newCustomersRow["PK_APILOG"] = PK_API;
                    newCustomersRow["D_CREATE"] = DateTime.Now.ToString("dd/MMM/yyyy");
                    newCustomersRow["T_CREATE"] = DateTime.Now.ToLocalTime().ToString("h:mm:ss");
                    newCustomersRow["OP_CREATE"] = "APIBATCH";

                    commandText.Rows.Add(newCustomersRow);

                    // }

                    #endregion


                    Services.BulkInsertDataLog(commandText, Services.ConvertStringArrayToStringJoin(columnNames), "APILDETL");


                    #region insert log record without using Bulk Insert Method
                    //////var commandText = "insert into APILOG (PK_APILOG,TRANS_LOG,D_API_B,T_API_B,API_SUCCES,PK_API,D_CREATE,T_CREATE,OP_CREATE)  (SELECT APILOG_SEQ.NEXTVAL,'" + DSC_API + "','" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','" + Status + "'," + PK_API + ",'" + DateTime.Now.ToString("dd/MMM/yyyy") + "','" + $"{ DateTime.Now:h:mm:ss}" + "','APIBATCH' FROM dual) ";

                    //using (OracleCommand command1 = new OracleCommand(commandText, connection))
                    //{
                    //    int numberOfRecords = command1.ExecuteNonQuery();

                    //    #region Get Last Inserted Record ID
                    //    var strQuery = "SELECT APILOG_SEQ.CURRVAL FROM DUAL";
                    //    OracleCommand objOracleCommand = new OracleCommand();
                    //    objOracleCommand = new OracleCommand(strQuery, connection);
                    //    intValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                    //    #endregion
                    //    PropertiesModel.LastPK_APILOGInserted = intValue;
                    //   // Console.WriteLine(PropertiesModel.LastPK_APILOGInserted);
                    //    result = "Success, Records Inserted : " + numberOfRecords + ", PK_APILOG : " + intValue + "";
                    //}
                    #endregion

                    result = "Success, Log Records Inserted : , PK_APLDETL : " + PropertiesModel.LastPK_APLDETLInserted + "";//numberOfRecords

                    connection.Close();
                }

                // log.Info(result);

                // DataLogUpdate.UpdateLog(intValue);
                return result;

            }
            catch (Exception ex)
            {
                result = "Log Record Insert has Failed";
                log.Info("Log Record Insert has Failed : " + ex.Message);
                return result;

            }

        }
    }
}
