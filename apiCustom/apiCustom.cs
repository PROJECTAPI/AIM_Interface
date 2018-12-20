using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using System.Threading.Tasks;
using AIM_Interface.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Reflection;
using System.Data.Common;
using System.Collections;
using System.Data.SQLite;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace AIM_Interface.Common
{

    public class allmethods
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        string result;
        Action finished = () =>
{
    DataLogUpdate.UpdateLog();
    Console.WriteLine("Finished");
};
        internal void apiCustom2018(int ApiID)
        {
            string ErrorMsg = "";
            try
            {
                Console.WriteLine("Hello from RS Lapsed Activities API Class !, ApiID: " + ApiID);
                result = string.Empty;
                string LogDetails = string.Empty;
                PropertiesModel.IsMainAPiLog = false;
                PropertiesModel.PK_API = ApiID;


                using (OracleConnection connection = new OracleConnection())
                {

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #region Creating Log Record After finishing all the processes Successfully
                    DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    PropertiesModel.MainPK_APILog = PropertiesModel.LastPK_APILOGInserted;
                    #endregion
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    string CurrentDate = DateTime.Now.ToString("d/MMM/yyyy");
                    DateTime TodaysDate = DateTime.Parse(CurrentDate);

                    string CurrentDateValue = DateTime.Now.ToString("d/MMM/yyyy");
                    DateTime YesterdayDate = DateTime.Parse(CurrentDateValue).AddDays(-1);

                    connection.ConnectionString = ConnectionProperties.connectionString;

                    #region Use this block to update record by record and insert log for every record
                    //string select = "select * from RSSCHED where D_SCHEDULE = '"+ YesterdayDate.ToString("d/MMM/yyyy") + "' AND (PK_RSSTAT = 1 OR PK_RSSTAT = 2) AND TP_PATTERN = 'Daily'";
                    //OracleDataAdapter da = new OracleDataAdapter(select, connection);
                    //DataSet ds = new DataSet();
                    //ds.DataSetName = "CancelDailies";

                    //da.Fill(ds, "CancelDailies");


                    #region COnvert Query to Datatable
                    //var CancelDailies = Services.GetDataTable("select * from RSSCHED  where D_SCHEDULE = '" + YesterdayDate.ToString("d/MMM/yyyy") + "' AND (PK_RSSTAT = 1 OR PK_RSSTAT = 2) AND TP_PATTERN = 'Daily'");
                    #endregion



                    #region Convert Datatable to An Array to get the columns names dynamically
                    //var columnNames = CancelDailies.Columns.Cast<DataColumn>()
                    //             .Select(x => x.ColumnName.Replace("\r", ""))
                    //             .ToArray();
                    #endregion

                    #region set a primary key column

                    //DataColumn[] columns = new DataColumn[1];
                    //columns[0] = CancelDailies.Columns["PK_RSSCHED"];
                    //CancelDailies.PrimaryKey = columns;

                    #endregion set a primary key column

                    #region Give New Values to some fields in the datatable

                    //  DataTable CancelDailies = SetValueForNull(ds.Tables["CancelDailies"]);

                    ////////DataRow[] rows = CancelDailies.Select();

                    ////////for (int i = 0; i < rows.Length; i++)
                    ////////{
                    ////////    rows[i]["PK_RSSTAT"] = 5;
                    ////////    rows[i]["FK_LAPACTN"] = 1;
                    ////////    rows[i]["D_LAPACTN"] = TodaysDate;
                    ////////    rows[i]["D_UPDATE"] = TodaysDate;
                    ////////    rows[i]["OP_UPDATE"] = "APIBATCH";
                    ////////    rows[i]["T_UPDATE"] = string.Format("{0:h:mm tt}", DateTime.Now);
                    ////////}

                    #endregion

                    // DataTable CancelDailiesFinalList = SetValueForNull(ds.Tables["CancelDailies"]);



                    //CageCard = GetNullFilledDataTableForXML(CageCard);




                    #region Update Record by Record and Log Every Record Update Status
                    //foreach (DataRow row in CancelDailies.Rows)
                    //{
                    //    DataTable Item = new DataTable();
                    //    Item = CancelDailies.Clone();
                    //    DataRow[] rowsToCopy;
                    //    rowsToCopy = CancelDailies.Select("PK_RSSCHED='" + row["PK_RSSCHED"] + "'");
                    //    int pk_rssched = Convert.ToInt32(row["PK_RSSCHED"]);
                    //    foreach (DataRow temp in rowsToCopy)
                    //    {
                    //        Item.ImportRow(temp);
                    //    }
                    //    Services.BulkUpdateData(Item, Services.ConvertStringArrayToStringJoin(columnNames), "RSSCHED", ApiID, pk_rssched);

                    //}

                    #endregion
                    // DataTable CageCards = SetValueForNull(CageCard);

                    // DataSet dss = new DataSet();

                    // dss.Tables.Add(CageCard);

                    #endregion

                    #region Update all records at once : Automatically Cancel Dailies


                    string CancelDailies = "Update RSSCHED set PK_RSSTAT = 5, FK_LAPACTN=1, D_LAPACTN='" + TodaysDate.ToString("d/MMM/yyyy") + "', D_UPDATE='" + TodaysDate.ToString("d/MMM/yyyy") + "', T_UPDATE='" + string.Format("{0:h:mm tt}", DateTime.Now) + "'  where D_SCHEDULE = '" + YesterdayDate.ToString("d/MMM/yyyy") + "' AND (PK_RSSTAT = 1 OR PK_RSSTAT = 2) AND TP_PATTERN = 'Daily'";


                    using (OracleCommand cmd = new OracleCommand(CancelDailies, connection))
                    {
                        cmd.Connection.Open();

                        OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        #region Creating Log Record After finishing all the processes Successfully
                        DataLogInsert.InsertLog(LogDetails, ApiID, result);
                        #endregion

                        try
                        {

                            cmd.ExecuteNonQuery();
                            trans.Commit();

                        }
                        catch (OracleException E)
                        {
                            trans.Rollback();
                            connection.Close();
                            result = "No";
                            log.Info("Automatically Cancel Dailies Records Update has Failed For API Number " + ApiID + ", Error : " + E.Message);

                            PropertiesModel.LogDetailsMessage = "Automatically Cancel Dailies Records Update has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                            PropertiesModel.LogDetailsStatus = "No";

                            #region Update the end Time and End Date of the Log Record Created
                            finished();
                            #endregion
                            throw;
                        }
                        finally
                        {
                            #region Update the end Time and End Date of the Log Record Created

                            PropertiesModel.LogDetailsMessage = "Automatically Cancel Dailies Records Method Finished Successfully";
                            PropertiesModel.LogDetailsStatus = "Yes";

                            finished();
                            #endregion
                        }
                        cmd.Connection.Close();
                    }

                    #endregion

                    #region Update all records at once : Automatically Move to Today Non - Dailies w / Leeway


                    string AutomaticallyMovetoToday = "Update RSSCHED set D_SCHEDULE = '" + TodaysDate.ToString("d/MMM/yyyy") + "', FK_LAPACTN=2, D_LAPACTN='" + TodaysDate.ToString("d/MMM/yyyy") + "', D_UPDATE='" + TodaysDate.ToString("d/MMM/yyyy") + "', T_UPDATE='" + string.Format("{0:h:mm tt}", DateTime.Now) + "'  where D_SCHEDULE = '" + YesterdayDate.ToString("d/MMM/yyyy") + "' AND (PK_RSSTAT = 1 OR PK_RSSTAT = 2) AND TP_PATTERN != 'Daily' AND (D_PLANNED + AF_LEEWAY > '" + YesterdayDate.ToString("d/MMM/yyyy") + "')";


                    using (OracleCommand cmd = new OracleCommand(AutomaticallyMovetoToday, connection))
                    {
                        cmd.Connection.Open();

                        OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        #region Creating Log Record After finishing all the processes Successfully
                        DataLogInsert.InsertLog(LogDetails, ApiID, result);
                        #endregion

                        try
                        {

                            cmd.ExecuteNonQuery();
                            trans.Commit();

                        }
                        catch (OracleException E)
                        {
                            trans.Rollback();
                            connection.Close();
                            result = "No";
                            log.Info("Automatically Move to Today Non - Dailies w / Leeway Records Update has Failed For API Number " + ApiID + ", Error : " + E.Message);

                            PropertiesModel.LogDetailsMessage = "Automatically Move to Today Non - Dailies w / Leeway Records Update has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                            PropertiesModel.LogDetailsStatus = "No";

                            #region Update the end Time and End Date of the Log Record Created
                            finished();
                            #endregion

                            throw;
                        }
                        finally
                        {
                            #region Update the end Time and End Date of the Log Record Created

                            PropertiesModel.LogDetailsMessage = "Automatically Move to Today Non - Dailies w / Leeway Records Method Finished Successfully";
                            PropertiesModel.LogDetailsStatus = "Yes";

                            finished();
                            #endregion
                        }
                        cmd.Connection.Close();
                    }

                    #endregion

                    #region Update all records at once : Mark as Requires Approval to Move to Today Non - Dailies w / o Leeway

                    string MarkasRequiresApproval = "Update RSSCHED set PK_RSSTAT = 5, FK_LAPACTN=3, D_LAPACTN='" + TodaysDate.ToString("d/MMM/yyyy") + "', D_UPDATE='" + TodaysDate.ToString("d/MMM/yyyy") + "', T_UPDATE='" + string.Format("{0:h:mm tt}", DateTime.Now) + "'  where D_SCHEDULE = '" + YesterdayDate.ToString("d/MMM/yyyy") + "' AND (PK_RSSTAT = 1 OR PK_RSSTAT = 2) AND TP_PATTERN != 'Daily' AND (D_PLANNED + AF_LEEWAY <= '" + YesterdayDate.ToString("d/MMM/yyyy") + "')";


                    using (OracleCommand cmd = new OracleCommand(MarkasRequiresApproval, connection))
                    {
                        cmd.Connection.Open();

                        OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        #region Creating Log Record After finishing all the processes Successfully
                        DataLogInsert.InsertLog(LogDetails, ApiID, result);
                        #endregion

                        try
                        {
                            cmd.ExecuteNonQuery();
                            trans.Commit();

                        }
                        catch (OracleException E)
                        {
                            trans.Rollback();
                            connection.Close();
                            result = "No";
                            log.Info("Mark as Requires Approval to Move to Today Non - Dailies w / o Leeway Records Update has Failed For API Number " + ApiID + ", Error : " + E.Message);

                            PropertiesModel.LogDetailsMessage = "Mark as Requires Approval to Move to Today Non - Dailies w / o Leeway Records Update has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                            PropertiesModel.LogDetailsStatus = "No";

                            #region Update the end Time and End Date of the Log Record Created
                            finished();
                            #endregion
                            throw;
                        }
                        finally
                        {
                            #region Update the end Time and End Date of the Log Record Created

                            PropertiesModel.LogDetailsMessage = "Mark as Requires Approval to Move to Today Non - Dailies w / o Leeway Records Update Method Finished Successfully For API Number " + ApiID + "";
                            PropertiesModel.LogDetailsStatus = "Yes";

                            finished();
                            #endregion
                        }
                        cmd.Connection.Close();
                    }

                    #endregion

                }

            }
            catch (Exception ex)
            {

                result = "No";
                ErrorMsg = ex.Message;
                Console.WriteLine(ex.Message);
                log.Info("API Processes Failed, Error: " + ex.Message + " , API Number : " + ApiID + "");

                PropertiesModel.LogDetailsMessage = "API Process Failed, Error: " + ex.Message + "";
                PropertiesModel.LogDetailsStatus = "No";

                #region Update the end Time and End Date of the Log Record Created
                finished();
                #endregion
            }
            finally
            {
                #region Update the end Time and End Date of the Log Record Created

                PropertiesModel.IsMainAPiLog = true;

                PropertiesModel.LogDetailsMessage = "API Process Finished Successfully";
                PropertiesModel.LogDetailsStatus = "Yes";

                finished();
                #endregion
            }
            // throw new NotImplementedException();
        }
        internal void apiCustom2019(int ApiID)
        {
            string ErrorMsg = "";
            try
            {
                Console.WriteLine("Hello from RS Remove Canceled or Closed Scenarios API Class !, ApiID: " + ApiID);
                result = string.Empty;
                string LogDetails = string.Empty;
                PropertiesModel.IsMainAPiLog = false;
                PropertiesModel.PK_API = ApiID;


                using (OracleConnection connection = new OracleConnection())
                {
                    OracleDataAdapter da = new OracleDataAdapter();
                    // connection.ConnectionString = ConnectionProperties.connectionString;
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand commandValue = connection.CreateCommand();
                    connection.Open();
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    #region Creating Log Record After finishing all the processes Successfully
                    DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    PropertiesModel.MainPK_APILog = PropertiesModel.LastPK_APILOGInserted;
                    #endregion
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    string CurrentDate = DateTime.Now.ToString("d/MMM/yyyy");
                    DateTime TodaysDate = DateTime.Parse(CurrentDate);

                    string CurrentDateValue = DateTime.Now.ToString("d/MMM/yyyy");
                    DateTime YesterdayDate = DateTime.Parse(CurrentDateValue).AddDays(-1);


                    #region : Automatically Remove Closed Scenarios

                    #region Creating Log Record After finishing all the processes Successfully
                    DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    #endregion

                    try
                    {

                        #region Remove Closed or Canceled Scenarios IDs from SCSCHED,SCOT,SCATND, SCRACKS, SCPTO, SCBRW tables first to prevent having Constraint Violation, then we remove from SCMAIN

                        var daysUntilDeletion = 7;

                        using (var commandRollingDays = connection.CreateCommand())
                        {
                            commandRollingDays.CommandText = "SELECT * FROM SYSPREFS  where PK_SYSPREF = 12";


                            // connection.Open();
                            using (var reader = commandRollingDays.ExecuteReader())
                            {
                                var RollingDays = reader.GetOrdinal("PROPERTY_V");

                                while (reader.Read())
                                {
                                    if (RollingDays != null)
                                    {
                                        daysUntilDeletion = Convert.ToInt32(reader.GetValue(RollingDays));
                                    }

                                }
                            }
                        }


                        var select = Services.GetDataTable("SELECT PK_SCMAIN FROM SCMAIN where PK_RSSTAT != 100 AND (D_SC_STAT + " + daysUntilDeletion + " < '" + TodaysDate.ToString("dd/MMM/yyyy") + "' OR D_SC_STAT IS NULL)");//where (PK_RSSTAT = 101 OR PK_RSSTAT = 103)  AND (D_SC_STAT + " + daysUntilDeletion + " > '" + TodaysDate.ToString("dd/MMM/yyyy") + "')


                        //ArrayList arr = new ArrayList();

                        ////put all the items in each row into a new item in the arraylist.
                        ////since the dataTable is 1 item per row, it cant be done this way
                        //foreach (DataRow dr in select.Rows)
                        //{
                        //    arr.Add(dr);
                        //}

                        //List<DataRow> list = new List<DataRow>(select.Select());

                        //ArrayList converted = new ArrayList(select.Rows.Count);

                        //foreach (DataRow row in select.Rows)

                        //{

                        //    converted.Add(row);

                        //}


                        //var stringArr = select.AsEnumerable().ToArray();
                        //string query = "SELECT * FROM SCSCHED WHERE PK_SCMAIN IN (" + converted + ")";



                        if (select.Rows.Count > 0)
                        {
                            foreach (DataRow row in select.Rows)
                            {
                                var ID = Convert.ToInt32(row["PK_SCMAIN"].ToString());

                                #region REMOVE FROM SCSCHED, SCOT, SCATND, SCPTO


                                //string RemoveFromScschedAll = "DELETE SCSCHED, SCOT, SCATND, SCRACKS, SCPTO, SCBRW  FROM SCSCHED JOIN SCOT ON SCOT.PK_SCMAIN = SCSCHED.PK_SCMAIN JOIN SCATND ON SCATND.PK_SCMAIN = SCSCHED.PK_SCMAIN JOIN SCRACKS ON SCRACKS.PK_SCMAIN = SCSCHED.PK_SCMAIN JOIN SCPTO ON SCPTO.PK_SCMAIN = SCSCHED.PK_SCMAIN  JOIN SCBRW ON SCBRW.PK_SCMAIN = SCSCHED.PK_SCMAIN where SCSCHED.PK_SCMAIN = " + ID + "";
                                //OracleCommand oraCmd = new OracleCommand(RemoveFromScschedAll, connection);
                                //oraCmd.ExecuteNonQuery();



                                string RemoveFromScsched = "Delete from  SCSCHED  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmd = new OracleCommand(RemoveFromScsched, connection);
                                oraCmd.ExecuteNonQuery();

                                string RemoveFromSCOT = "Delete from  SCOT  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmdSCOT = new OracleCommand(RemoveFromSCOT, connection);
                                oraCmdSCOT.ExecuteNonQuery();

                                string RemoveFromSCATND = "Delete from  SCATND  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmdSCATND = new OracleCommand(RemoveFromSCATND, connection);
                                oraCmdSCATND.ExecuteNonQuery();

                                string RemoveFromSCRACKS = "Delete from  SCRACKS  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmdSCRACKS = new OracleCommand(RemoveFromSCRACKS, connection);
                                oraCmdSCRACKS.ExecuteNonQuery();

                                string RemoveFromSCPTO = "Delete from  SCPTO  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmdSCPTO = new OracleCommand(RemoveFromSCPTO, connection);
                                oraCmdSCPTO.ExecuteNonQuery();

                                string RemoveFromSCBRW = "Delete from  SCBRW  where PK_SCMAIN = " + ID + "";
                                OracleCommand oraCmdSCBRW = new OracleCommand(RemoveFromSCBRW, connection);
                                oraCmdSCBRW.ExecuteNonQuery();


                                #endregion
                            }

                            string RemoveFromSCMAIN = "Delete from  SCMAIN  where PK_RSSTAT != 100 AND (D_SC_STAT + " + daysUntilDeletion + " < '" + TodaysDate.ToString("dd/MMM/yyyy") + "'  OR D_SC_STAT IS NULL)";
                            OracleCommand oraCmdSCMAIN = new OracleCommand(RemoveFromSCMAIN, connection);
                            oraCmdSCMAIN.ExecuteNonQuery();

                        }
                        else
                        {
                            Console.WriteLine("No Canceled or Closed Scenarios Found to be removed !!");
                        }


                        #endregion

                    }
                    catch (OracleException E)
                    {
                        // trans.Rollback();
                        connection.Close();
                        result = "No";
                        log.Info("Removing Closed or Canceled Scenarios has Failed For API Number " + ApiID + ", Error : " + E.Message);

                        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                        PropertiesModel.LogDetailsStatus = "No";

                        #region Update the end Time and End Date of the Log Record Created
                        finished();
                        #endregion
                        throw;
                    }
                    finally
                    {
                        #region Update the end Time and End Date of the Log Record Created

                        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios has Finished Successfully";
                        PropertiesModel.LogDetailsStatus = "Yes";

                        finished();
                        #endregion
                    }




                    #endregion


                    #region REMOVE FROM SCOT
                    //string RemoveFromSCOT = "Delete from  SCOT  where PK_SCMAIN = " + ID + "";

                    //using (OracleCommand cmd = new OracleCommand(RemoveFromSCOT, connection))
                    //{
                    //    //cmd.Connection.Open();

                    //    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    //    cmd.Transaction = trans;

                    //    #region Creating Log Record After finishing all the processes Successfully
                    //    // DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    //    #endregion

                    //    try
                    //    {

                    //        cmd.ExecuteNonQuery();
                    //        trans.Commit();

                    //    }
                    //    catch (OracleException E)
                    //    {
                    //        trans.Rollback();
                    //        connection.Close();
                    //        result = "No";
                    //        log.Info("Removing Closed or Canceled Scenarios IDs from SCOT has Failed For API Number " + ApiID + ", Error : " + E.Message);

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCOT has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                    //        PropertiesModel.LogDetailsStatus = "No";

                    //        #region Update the end Time and End Date of the Log Record Created
                    //        // finished();
                    //        #endregion
                    //        throw;
                    //    }
                    //    finally
                    //    {
                    //        #region Update the end Time and End Date of the Log Record Created

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCOT has Finished Successfully";
                    //        PropertiesModel.LogDetailsStatus = "Yes";

                    //        //  finished();
                    //        #endregion
                    //    }


                    //    //  cmd.Connection.Close();
                    //}
                    #endregion

                    #region REMOVE FROM SCATND
                    //string RemoveFromSCATND = "Delete from  SCATND  where PK_SCMAIN = " + ID + "";

                    //using (OracleCommand cmd = new OracleCommand(RemoveFromSCATND, connection))
                    //{
                    //    //cmd.Connection.Open();

                    //    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    //    cmd.Transaction = trans;

                    //    #region Creating Log Record After finishing all the processes Successfully
                    //    // DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    //    #endregion

                    //    try
                    //    {

                    //        cmd.ExecuteNonQuery();
                    //        trans.Commit();

                    //    }
                    //    catch (OracleException E)
                    //    {
                    //        trans.Rollback();
                    //        connection.Close();
                    //        result = "No";
                    //        log.Info("Removing Closed or Canceled Scenarios IDs from SCATND has Failed For API Number " + ApiID + ", Error : " + E.Message);

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCATND has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                    //        PropertiesModel.LogDetailsStatus = "No";

                    //        #region Update the end Time and End Date of the Log Record Created
                    //        // finished();
                    //        #endregion
                    //        throw;
                    //    }
                    //    finally
                    //    {
                    //        #region Update the end Time and End Date of the Log Record Created

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCATND has Finished Successfully";
                    //        PropertiesModel.LogDetailsStatus = "Yes";

                    //        //  finished();
                    //        #endregion
                    //    }


                    //    //  cmd.Connection.Close();
                    //}
                    #endregion

                    #region REMOVE FROM SCRACKS
                    //string RemoveFromSCRACKS = "Delete from  SCRACKS  where PK_SCMAIN = " + ID + "";

                    //using (OracleCommand cmd = new OracleCommand(RemoveFromSCRACKS, connection))
                    //{
                    //    //cmd.Connection.Open();

                    //    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    //    cmd.Transaction = trans;

                    //    #region Creating Log Record After finishing all the processes Successfully
                    //    // DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    //    #endregion

                    //    try
                    //    {

                    //        cmd.ExecuteNonQuery();
                    //        trans.Commit();

                    //    }
                    //    catch (OracleException E)
                    //    {
                    //        trans.Rollback();
                    //        connection.Close();
                    //        result = "No";
                    //        log.Info("Removing Closed or Canceled Scenarios IDs from SCRACKS has Failed For API Number " + ApiID + ", Error : " + E.Message);

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCRACKS has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                    //        PropertiesModel.LogDetailsStatus = "No";

                    //        #region Update the end Time and End Date of the Log Record Created
                    //        // finished();
                    //        #endregion
                    //        throw;
                    //    }
                    //    finally
                    //    {
                    //        #region Update the end Time and End Date of the Log Record Created

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCRACKS has Finished Successfully";
                    //        PropertiesModel.LogDetailsStatus = "Yes";

                    //        //  finished();
                    //        #endregion
                    //    }


                    //    //  cmd.Connection.Close();
                    //}
                    #endregion

                    #region REMOVE FROM SCPTO
                    //string RemoveFromSCPTO = "Delete from  SCPTO  where PK_SCMAIN = " + ID + "";

                    //using (OracleCommand cmd = new OracleCommand(RemoveFromSCPTO, connection))
                    //{
                    //    //cmd.Connection.Open();

                    //    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    //    cmd.Transaction = trans;

                    //    #region Creating Log Record After finishing all the processes Successfully
                    //    // DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    //    #endregion

                    //    try
                    //    {

                    //        cmd.ExecuteNonQuery();
                    //        trans.Commit();

                    //    }
                    //    catch (OracleException E)
                    //    {
                    //        trans.Rollback();
                    //        connection.Close();
                    //        result = "No";
                    //        log.Info("Removing Closed or Canceled Scenarios IDs from SCPTO has Failed For API Number " + ApiID + ", Error : " + E.Message);

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCPTO has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                    //        PropertiesModel.LogDetailsStatus = "No";

                    //        #region Update the end Time and End Date of the Log Record Created
                    //        // finished();
                    //        #endregion
                    //        throw;
                    //    }
                    //    finally
                    //    {
                    //        #region Update the end Time and End Date of the Log Record Created

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios IDs from SCPTO has Finished Successfully";
                    //        PropertiesModel.LogDetailsStatus = "Yes";

                    //        //  finished();
                    //        #endregion
                    //    }


                    //    //  cmd.Connection.Close();
                    //}
                    #endregion

                    #region Remove Closed or Canceled Scenarios from SCMAIN

                    //string RemoveScenarios = "Delete from  SCMAIN  where PK_RSSTAT != 100 and PK_SCMAIN = " + ID + "";

                    //using (OracleCommand cmd = new OracleCommand(RemoveScenarios, connection))
                    //{
                    //  // cmd.Connection.Open();

                    //    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    //    cmd.Transaction = trans;

                    //    #region Creating Log Record After finishing all the processes Successfully
                    //    DataLogInsert.InsertLog(LogDetails, ApiID, result);
                    //    #endregion

                    //    try
                    //    {

                    //        cmd.ExecuteNonQuery();
                    //        trans.Commit();

                    //    }
                    //    catch (OracleException E)
                    //    {
                    //        trans.Rollback();
                    //        connection.Close();
                    //        result = "No";
                    //        log.Info("Removing Closed or Canceled Scenarios has Failed For API Number " + ApiID + ", Error : " + E.Message);

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios has Failed to Finish For API Number " + ApiID + ", Error : " + E.Message + "";
                    //        PropertiesModel.LogDetailsStatus = "No";

                    //        #region Update the end Time and End Date of the Log Record Created
                    //        finished();
                    //        #endregion
                    //        throw;
                    //    }
                    //    finally
                    //    {
                    //        #region Update the end Time and End Date of the Log Record Created

                    //        PropertiesModel.LogDetailsMessage = "Removing Closed or Canceled Scenarios has Finished Successfully";
                    //        PropertiesModel.LogDetailsStatus = "Yes";

                    //        finished();
                    //        #endregion
                    //    }
                    //  //  cmd.Connection.Close();
                    //}
                    #endregion


                }

            }
            catch (Exception ex)
            {

                result = "No";
                ErrorMsg = ex.Message;
                Console.WriteLine(ex.Message);
                log.Info("API Processes Failed, Error: " + ex.Message + " , API Number : " + ApiID + "");

                PropertiesModel.LogDetailsMessage = "API Process Failed, Error: " + ex.Message + "";
                PropertiesModel.LogDetailsStatus = "No";

                #region Update the end Time and End Date of the Log Record Created
                finished();
                #endregion
            }
            finally
            {
                #region Update the end Time and End Date of the Log Record Created

                PropertiesModel.IsMainAPiLog = true;

                PropertiesModel.LogDetailsMessage = "API Process Finished Successfully";
                PropertiesModel.LogDetailsStatus = "Yes";

                finished();
                #endregion
            }
            // throw new NotImplementedException();
        }
        private DataTable SetValueForNull(DataTable dt)
        {
            int i, j;
            for (i = 0; i < dt.Columns.Count; i++)
            {
                for (j = 0; j < dt.Rows.Count; j++)
                {
                    if (dt.Columns[i].DataType.ToString() == "System.Int32" || dt.Columns[i].DataType.ToString() == "System.Single" || dt.Columns[i].DataType.ToString() == "System.Double" || dt.Columns[i].DataType.ToString() == "System.Decimal")
                    {
                        if (dt.Rows[j][i] == DBNull.Value)
                            dt.Rows[j][i] = 0;


                    }
                    else if (dt.Columns[i].DataType.ToString() == "System.String")
                    {


                        if (dt.Rows[j][i] == DBNull.Value || dt.Rows[j][i].ToString().Trim() == "")
                            dt.Rows[j][i] = "0";
                    }
                }
            }
            return dt;
        }
        private DataTable GetNullFilledDataTableForXML(DataTable dtSource)
        {
            // Create a target table with same structure as source and fields as strings
            // We can change the column datatype as long as there is no data loaded
            DataTable dtTarget = dtSource.Clone();
            foreach (DataColumn col in dtTarget.Columns)
                col.DataType = typeof(string);

            // Start importing the source into target by ItemArray copying which
            // is found to be reasonably fast for nulk operations. VS 2015 is reporting
            // 500-525 milliseconds for loading 100,000 records x 10 columns
            // after null conversion in every cell which may be usable in many
            // circumstances.
            // Machine config: i5 2nd Gen, 8 GB RAM, Windows 7 64bit, VS 2015 Update 1
            int colCountInTarget = dtTarget.Columns.Count;
            foreach (DataRow sourceRow in dtSource.Rows)
            {
                // Get a new row loaded with data from source row
                DataRow targetRow = dtTarget.NewRow();
                targetRow.ItemArray = sourceRow.ItemArray;

                // Update DBNull.Values to empty string in the new (target) row
                // We can safely assign empty string since the target table columns
                // are all of string type
                for (int ctr = 0; ctr < colCountInTarget; ctr++)
                    if (targetRow[ctr] == DBNull.Value)
                        targetRow[ctr] = String.Empty;

                // Now add the null filled row to target datatable
                dtTarget.Rows.Add(targetRow);
            }

            // Return the target datatable
            return dtTarget;
        }
    }


}
