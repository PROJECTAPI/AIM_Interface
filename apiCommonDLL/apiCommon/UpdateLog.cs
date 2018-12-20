using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using AIM_Interface.Models;
using System.Linq;

namespace apiCommonDLL
{

    public class DataLogUpdate
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static string UpdateLog()
        {
            string result = "";

            try
            {
                using (OracleConnection connection = new OracleConnection())
                {
                    //DataTable dt = new DataTable();
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand command = connection.CreateCommand();
                    connection.Open();



                    var commandText = Services.GetDataTable("Select * from APILOG Where PK_APILOG = " + PropertiesModel.LastPK_APILOGInserted + "");

                    #region Convert Datatable to An Array to get the columns names dynamically
                    var columnNames = commandText.Columns.Cast<DataColumn>()
                                 .Select(x => x.ColumnName.Replace("\r", ""))
                                 .ToArray();
                    #endregion

                    #region set a primary key column

                    DataColumn[] columns = new DataColumn[1];
                    columns[0] = commandText.Columns["PK_APILOG"];
                    commandText.PrimaryKey = columns;

                    #endregion set a primary key column

                    #region Give New Values to some fields in the datatable

                    DataRow[] rows = commandText.Select();

                    for (int i = 0; i < rows.Length; i++)
                    {
                        rows[i]["D_API_E"] = DateTime.Now.ToString("dd/MMM/yyyy");
                        rows[i]["T_API_E"] = DateTime.Now.ToLocalTime().ToString("h:mm:ss");
                    }

                    #endregion


                    Services.BulkUpdateDataLog(commandText, Services.ConvertStringArrayToStringJoin(columnNames), "APILOG");

                    result = "Success, Log Records Updated , PK_APILOG : " + PropertiesModel.LastPK_APILOGInserted + "";//numberOfRecords

                    #region use single update No Bulk Method Called
                    //var commandText = "Update APILOG Set D_API_E = '" + DateTime.Now.ToString("dd/MMM/yyyy") + "',T_API_E ='" + $"{ DateTime.Now:h:mm:ss}" + "' Where PK_APILOG = " + PropertiesModel.LastPK_APILOGInserted + "";

                    //using (OracleCommand command1 = new OracleCommand(commandText, connection))
                    //{
                    //   int numberOfRecords = command1.ExecuteNonQuery();
                    // result = "Success, Records Updated : " + adapter + ", PK_APILOG : " + PropertiesModel.LastPK_APILOGInserted + "";
                    //}
                    #endregion

                    connection.Close();
                }

                //  log.Info(result);
                return result;
            }
            catch (Exception ex)
            {
                result = "Log Records Update has Failed";
                log.Info("Log Records Update has Failed : " + ex.Message);
                return result;

            }
        }
    }
}
