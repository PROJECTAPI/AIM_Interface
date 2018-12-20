using System;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using System.Configuration;
using log4net;
using AIM_Interface.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.IO;
using System.Data.SQLite;
using System.ComponentModel;
using Microsoft.VisualBasic.FileIO;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.InteropServices;
using System.Data.OleDb;
using ExcelDataReader;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Globalization;
using System.Threading;
using AIM_Interface.Common;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Client;

namespace AIM_Interface.Common
{

    public static class Services
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static Action finished = () =>
         {
             DataLogUpdate.UpdateLog();
             Console.WriteLine("Finished Inserting Data Records");

         };
        public static Action finishedInsertingLog = () =>
        {
            DataLogUpdate.UpdateLog();
            Console.WriteLine("Finished Inserting Log Records");

        };
        public static string BulkUpdateData(DataTable data, string Columns, string tableName, int ApiID, int RecordID)
        {
            string result = "";

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;

                string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    connection.Open();

                    OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                    myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);

                    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;

                    try
                    {
                        OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                        custCB.ConflictOption = ConflictOption.OverwriteChanges;
                        custCB.SetAllValues = true;
                        int i = 0;
                        foreach (DataRow dr in data.Rows)
                        {
                            if (dr.RowState == DataRowState.Unchanged)
                                dr.SetModified();
                            i = i + 1;
                        }
                        myDataAdapter.Update(data);
                        data.AcceptChanges();
                        myDataAdapter.Dispose();
                        trans.Commit();

                        result = "Success, Data Records Updated in table " + tableName + " : " + i + ", Record ID : "+ RecordID + "";
                        Console.WriteLine(result);
                        //  int rowsAffected = cmd.ExecuteNonQuery();
                        log.Info(result);

                        #region Creating Log Record After finishing all the processes
                        DataLogInsert.InsertLogDetails(result, PropertiesModel.LastPK_APILOGInserted, "Success");
                        #endregion

                        return result;

                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        trans.Rollback();
                        connection.Close();
                        result = "Data Record Update has Failed into table " + tableName + " : " + E.Message + ", API Number: " + ApiID + ",PK_APILOG ID : "+ PropertiesModel.LastPK_APILOGInserted + ", Record ID : " + RecordID + "";
                        log.Info("Data Record Update has Failed into table " + tableName + " : " + E.Message + ", API Number: " + ApiID + ",PK_APILOG ID : " + PropertiesModel.LastPK_APILOGInserted + ", Record ID : " + RecordID + "");
                        #region Creating Log Record After Failure in Inserting all Records
                        DataLogInsert.InsertLogDetails("Error, Record Failed while Updating table " + tableName + " : " + E.Message + "", PropertiesModel.LastPK_APILOGInserted, "Warning");
                        #endregion
                        return result;
                        //return false;
                    }
                    finally
                    {
                        //finished();
                    }
                }
            }
        }
        public static string BulkInsertData(DataTable ds, string Columns, string tableName, int ApiID)//, List<int> RecordsID
        {
            string result = "";
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            bool PrimaryKeyExist = false;

            #region get number of records inside the Datatable (used to get the number of NextSequences Needed)
            int NumOfRecords = ds.Rows.Count;

            List<int> NextSeqRangeArray = Services.GetNextSeqRange(tableName, NumOfRecords);

            #endregion



            string[] RecordsInsertedIDs = NextSeqRangeArray.Select(i => i.ToString()).ToArray();

            #region check if dataset has the PRIMARY KEY Column that exist in metadata view QV_TABLE_PRIMARY_KEYS
            try
            {
                using (SQLiteConnection SQLconnect = new SQLiteConnection())
                {


                    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                    SQLconnect.Open();
                    Console.WriteLine("Metdata Connection State In BulkInsert method: {0}", SQLconnect.State);

                    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and FLD_NM <>'';", SQLconnect);

                    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                    SQLiteDataReader r = cmd.ExecuteReader();

                    if (r.HasRows)
                    {
                        while (r.Read())
                        {
                            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                            DataColumnCollection columns = ds.Columns;
                            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                            //{
                            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                            {
                                PrimaryKeyExist = true;

                                if (r.GetInt16(r.GetOrdinal("SURROGATE")) == 1)
                                {
                                    int index = 0;

                                    foreach (DataRow row in ds.Rows)
                                    {
                                        string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];

                                        if (string.IsNullOrEmpty(value))
                                        {
                                            #region Get Next PK_APILOG SEquence ID
                                            int indexChild = 0;

                                            foreach (var NextSeq in NextSeqRangeArray)
                                            {
                                                if (index == indexChild)
                                                {
                                                    row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                                                    break;
                                                }
                                                indexChild++;
                                            }
                                            #endregion
                                            index++;



                                        }

                                    }

                                }
                            }
                            else
                            {
                                PrimaryKeyExist = false;
                                result = "No";
                                log.Info("Insert Failed for table "+ tableName + ", Error: Primary keys Are Missing for Table " + tableName + " , API Number : " + ApiID + "");
                                #region Creating Log Record After finishing all the processes
                                DataLogInsert.InsertLog("API Processes Failed Inserting into table " + tableName + ", Error: Primary keys Are Missing for Table " + tableName + "", ApiID, result);
                                #endregion
                                break;
                            }

                            // }
                        }

                    }
                    else
                    {
                        //////////// No Primary Keys Fields Found for this table in the metadata
                        PrimaryKeyExist = false;
                        result = "No";
                        log.Info("Insert Failed for table " + tableName + ", Error: Primary keys Are Missing for Table " + tableName + " , API Number : " + ApiID + "");
                        #region Creating Log Record After finishing all the processes
                        DataLogInsert.InsertLog("API Processes Failed Inserting into table " + tableName + ", Error: Primary keys Are Missing for Table " + tableName + "", ApiID, result);
                        #endregion
                    }
                    SQLconnect.Close();
                }
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine(ex.Message);
            }
            #endregion

            if (PrimaryKeyExist)
            {

                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = ConnectionProperties.connectionString;

                    string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
                    using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                    {
                        connection.Open();

                        OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                        myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);

                        OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                        cmd.Transaction = trans;

                        try
                        {
                            myDataAdapter.UpdateBatchSize = 0;
                            OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                            //DataTable dt = ds.Tables[0].Copy();
                            DataTable dtTemp = ds.Clone();

                            int times = 0;
                            int k = 0;
                            for (int count = 0; count < ds.Rows.Count; times++)
                            {
                                for (int i = 0; i < 400 && 400 * times + i < ds.Rows.Count; i++, count++)
                                {
                                    //if (dtTemp.RowState != DataRowState.Unchanged)
                                    //{

                                    //}
                                    dtTemp.Rows.Add(ds.Rows[count].ItemArray);
                                    k = k + 1;
                                }

                                myDataAdapter.Update(dtTemp);
                                dtTemp.Rows.Clear();
                            }



                            ds.Dispose();
                            dtTemp.Dispose();
                            myDataAdapter.Dispose();
                            trans.Commit();

                            result = "Success, " + ConvertStringArrayToStringJoin(RecordsInsertedIDs) + " Records has been Inserted into table " + tableName + ", Number of Records : " + k + " , API Number : " + ApiID + "";

                            Console.WriteLine(result);
                            log.Info(result);
                            connection.Close();

                            #region Creating Log Record After Inserting all Records
                            DataLogInsert.InsertLog(result, ApiID, "Yes");
                            #endregion

                            return result;

                        }
                        catch (System.Data.OracleClient.OracleException E)
                        {
                            trans.Rollback();
                            connection.Close();
                            result = "Error, Data Records Insert has Failed into table " + tableName + " : " + E.Message + " , API Number : " + ApiID + "";
                            log.Info("Error, Data Records Insert has Failed into table " + tableName + " : " + E.Message + " , API Number : " + ApiID + "");
                            #region Creating Log Record After Failure in Inserting all Records
                            DataLogInsert.InsertLog("Error, Records Failed while Inserting into table " + tableName + " : " + E.Message + "", ApiID, "No");
                            #endregion
                            return result;
                        }
                        finally
                        {
                            finished();
                        }

                    }
                }

            }

            return result;

        }
        public static string BulkUpdateDataLog(DataTable data, string Columns, string tableName)
        {
            string result = "";

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;

                string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    connection.Open();

                    OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                    myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);

                    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;

                    try
                    {
                        OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                        custCB.ConflictOption = ConflictOption.OverwriteChanges;
                        custCB.SetAllValues = true;
                        int i = 0;
                        foreach (DataRow dr in data.Rows)
                        {
                            if (dr.RowState == DataRowState.Unchanged)
                                dr.SetModified();
                            i = i + 1;
                        }
                        myDataAdapter.Update(data);
                        data.AcceptChanges();
                        myDataAdapter.Dispose();
                        trans.Commit();

                        result = "Success,Log Records Updated : " + i + "";
                        Console.WriteLine(result);
                        //  int rowsAffected = cmd.ExecuteNonQuery();
                        log.Info(result);
                        return result;

                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        trans.Rollback();
                        connection.Close();
                        result = "Log Records Update has Failed";
                        log.Info("Log Records Update has Failed : " + E.Message);
                        return result;
                        //return false;
                    }
                }
            }
        }
        public static string BulkInsertDataLog(DataTable ds, string Columns, string tableName)
        {
            string result = "";

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;

                string SQLString = string.Format("select {0} from {1} where rownum=0", Columns, tableName);
                using (OracleCommand cmd = new OracleCommand(SQLString, connection))
                {
                    connection.Open();

                    OracleDataAdapter myDataAdapter = new OracleDataAdapter();
                    myDataAdapter.SelectCommand = new OracleCommand(SQLString, connection);

                    OracleTransaction trans = connection.BeginTransaction(IsolationLevel.ReadCommitted);
                    cmd.Transaction = trans;

                    try
                    {
                        myDataAdapter.UpdateBatchSize = 0;
                        OracleCommandBuilder custCB = new OracleCommandBuilder(myDataAdapter);
                        //DataTable dt = ds.Tables[0].Copy();
                        DataTable dtTemp = ds.Clone();

                        int times = 0;
                        int k = 0;
                        for (int count = 0; count < ds.Rows.Count; times++)
                        {
                            for (int i = 0; i < 400 && 400 * times + i < ds.Rows.Count; i++, count++)
                            {
                                //if (dtTemp.RowState != DataRowState.Unchanged)
                                //{

                                //}
                                dtTemp.Rows.Add(ds.Rows[count].ItemArray);
                                k = k + 1;
                            }

                            myDataAdapter.Update(dtTemp);
                            dtTemp.Rows.Clear();
                        }



                        ds.Dispose();
                        dtTemp.Dispose();
                        myDataAdapter.Dispose();
                        trans.Commit();

                        result = "Success, Log Records Inserted : " + k + "";

                        Console.WriteLine(result);
                        log.Info(result);
                        connection.Close();


                        return result;




                    }
                    catch (System.Data.OracleClient.OracleException E)
                    {
                        trans.Rollback();
                        connection.Close();
                        result = "Log Records Insert has Failed";
                        log.Info("Log Records Insert has Failed : " + E.Message);
                        return result;
                    }
                    finally
                    {
                        finishedInsertingLog();
                    }

                }
            }

        }
        public static DataTable GetDataTable(string query)
        {
            String ConnString = ConnectionProperties.connectionString;
            OracleDataAdapter adapter = new OracleDataAdapter();
            DataTable myDataTable = new DataTable();
            using (OracleConnection conn = new OracleConnection(ConnString))
            {
                adapter.SelectCommand = new OracleCommand(query, conn);
                adapter.Fill(myDataTable);
            }


            return myDataTable;
        }
        public static DataTable SelectedColumns(DataTable RecordDT_, string VER_NOTES, string DSC_API, string PK_API, string D_CREATE, string T_CREATE, string PK_APIVER)
        {
            DataTable TempTable = RecordDT_;

            System.Data.DataView view = new System.Data.DataView(TempTable);
            System.Data.DataTable selected = view.ToTable("Selected", false, VER_NOTES, DSC_API);
            return selected;
        }
        public static DataTable ReadCsvFileWithHeaderRow(string CLientCSVFile)
        {

            DataTable dtCsv = new DataTable();
            string Fulltext;

            string FileSaveWithPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../" + CLientCSVFile + ""));
            using (StreamReader sr = new StreamReader(FileSaveWithPath))
            {
                while (!sr.EndOfStream)
                {
                    Fulltext = sr.ReadToEnd().ToString(); //read full file text
                    string[] rows = Fulltext.Split('\n'); //split full file text into rows
                    for (int i = 0; i < rows.Count() - 1; i++)
                    {
                        string[] rowValues = rows[i].Split(','); //split each row with comma to get individual values
                        {
                            if (i == 0)
                            {
                                for (int j = 0; j < rowValues.Count(); j++)
                                {
                                    dtCsv.Columns.Add(rowValues[j].Replace("\r", "")); //add headers
                                }
                            }
                            else
                            {
                                DataRow dr = dtCsv.NewRow();
                                for (int k = 0; k < rowValues.Count(); k++)
                                {
                                    dr[k] = rowValues[k].Replace("\r", "").ToString();
                                }
                                dtCsv.Rows.Add(dr); //add other rows
                            }
                        }
                    }
                }
            }
            return dtCsv;
        }
        public static DataTable ReadCsvFileNoHeaderRow(string CLientCSVFile)
        {
            //bool header = false;
            // string[] alldata = "";
            DataTable dt = new DataTable();
            List<string> result = new List<string>();

            string[] FileSaveWithPath1 = File.ReadAllLines(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../" + CLientCSVFile + "")));
            var totalNumofcols = FileSaveWithPath1[0].Split(',').Length;

            for (int i = 0; i < totalNumofcols; i++)
            {
                DataColumn dc = new DataColumn("col" + i.ToString(), typeof(string));
                dt.Columns.Add(dc);
            }

            foreach (var nextline in FileSaveWithPath1)
            {
                // if line empty continue itration
                if (string.IsNullOrEmpty(nextline))
                    continue;
                #region On way
                // declare bool var outside loop and find header
                //if (!header)
                //{
                //    var allcolumns = nextline.Split(',');
                //    //your all columns or header infor will be here
                //    //TODO: what you want
                //    header = true;
                //}
                //else
                //{
                //your data line
                var alldatacolumns = nextline.Split(',');
                //TODO: what you want,

                dt.Rows.Add(alldatacolumns);
                //  }
                #endregion

                #region other way

                //you make sure your first line is header then try this
                // var alldata = nextline.Split(',');


                #endregion


            }

            //string[] st = new string[] { "1", "2", "3" };
            //   DataTable dt = new DataTable();

            //   ToDataTable(result);
            return dt;


        }
        public static string ConvertStringArrayToStringJoin(string[] array)
        {
            // Use string Join to concatenate the string elements.
            string result = string.Join(",", array);
            return result;
        }
        public static int GetNextSeq(string TableName)
        {
            int NextValue = 0;

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                connection.Open();

                #region Get Next  SEquence ID

                var strQuery = "SELECT " + TableName + "_SEQ.NEXTVAL FROM DUAL";//APILOG_SEQ
                OracleCommand objOracleCommand = new OracleCommand();
                objOracleCommand = new OracleCommand(strQuery, connection);
                NextValue = Convert.ToInt32(objOracleCommand.ExecuteScalar());
                //////////////// PropertiesModel.LastPK_APILOGInserted = NextValue;
                #endregion
            }
            return NextValue;
        }
        public static List<int> GetNextSeqRange(string TableName, int NumOfRecords)
        {
            int NextValueGrabbed = 0;
            List<int> NextValueSeqArray = new List<int>();


            // You can convert it back to an array if you would like to
            int[] terms = NextValueSeqArray.ToArray();

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                connection.Open();

                #region Get Next  SEquence ID

                var strQuery = "SELECT " + TableName + "_SEQ.NEXTVAL FROM DUAL";//APILOG_SEQ
                OracleCommand objOracleCommand = new OracleCommand();
                objOracleCommand = new OracleCommand(strQuery, connection);
                NextValueGrabbed = Convert.ToInt32(objOracleCommand.ExecuteScalar());

                for (int NextValue = 0; NextValue < NumOfRecords; NextValue++)
                {
                    NextValueSeqArray.Add(NextValueGrabbed + NextValue);
                }
                //////////////// PropertiesModel.LastPK_APILOGInserted = NextValue;
                #endregion
            }
            return NextValueSeqArray;
        }
        public static DataTable ConvertExcelToDatatable(string excelFilePath)
        {
            string ExcelFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../" + excelFilePath + ""));

            string connectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ExcelFile + ";Extended Properties=\"Excel 12.0 Xml; HDR=NO; IMEX=1\"";
            // if you don't want to show the header row (first row) use 'HDR=NO' in the string
            OleDbConnection excelConnection = new OleDbConnection(connectionString);
            excelConnection.Open();
            ////////// Get the data table containg the schema guid.
            ////////DataTable dtWorksheetTables = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
            //////////  if (dtWorksheetTables == null || dtWorksheetTables.Rows.Count == 0) return false;
            ////////// string worksheetName = GetWorksheetName(dtWorksheetTables);
            ////////string strExcelSQL = "SELECT * FROM [Sheet1$]";
            ////////OleDbCommand oleDbCommand = new OleDbCommand(strExcelSQL, excelConnection);
            ////////OleDbDataAdapter dataAdapter = new OleDbDataAdapter(oleDbCommand);
            ////////DataTable excelDataTable = new DataTable();
            ////////dataAdapter.Fill(excelDataTable);
            //////////Dispose
            ////////dataAdapter.Dispose();
            ////////oleDbCommand.Dispose();
            ////////excelConnection.Close();
            ////////excelConnection.Dispose();
            ////////GC.Collect();

            var prevCulture = Thread.CurrentThread.CurrentCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            DataTable DT = new DataTable();
            try
            {


                XSSFWorkbook wb;
                XSSFSheet sh;
                String Sheet_name;

                using (var fs = new FileStream(ExcelFile, FileMode.Open, FileAccess.Read))
                {
                    wb = new XSSFWorkbook(fs);

                    Sheet_name = wb.GetSheetAt(0).SheetName;  //get first sheet name
                }

                DT.Rows.Clear();
                DT.Columns.Clear();

                // get sheet
                sh = (XSSFSheet)wb.GetSheet(Sheet_name);

                int i = 0;
                while (sh.GetRow(i) != null)
                {
                    // add neccessary columns
                    if (DT.Columns.Count < sh.GetRow(i).Cells.Count)
                    {
                        for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                        {
                            DT.Columns.Add("", typeof(string));
                        }
                    }

                    // add row
                    DT.Rows.Add();

                    // write row value
                    for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                    {
                        var cell = sh.GetRow(i).GetCell(j);

                        if (cell != null)
                        {
                            // TODO: you can add more cell types capatibility, e. g. formula
                            switch (cell.CellType)
                            {
                                case NPOI.SS.UserModel.CellType.Numeric:
                                    // DT.Rows[i][j] = sh.GetRow(i).GetCell(j).NumericCellValue;

                                    DateTime dateValue = new DateTime(cell.DateCellValue.Year);

                                    string DateNow = cell.DateCellValue.ToString("M/d/yyyy");
                                    DateTime DateNowValue = DateTime.Parse(DateNow);

                                    Console.WriteLine(dateValue);

                                    if (DateNow == "12/31/1899")
                                    {
                                        DT.Rows[i][j] = DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue.ToString("hh:mm:ss") : cell.NumericCellValue.ToString();
                                    }
                                    else
                                    {
                                        DT.Rows[i][j] = DateUtil.IsCellDateFormatted(cell) ? cell.DateCellValue.ToString("M/dd/yyyy") : cell.NumericCellValue.ToString();
                                    }

                                    //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;

                                    break;
                                case NPOI.SS.UserModel.CellType.Unknown:
                                    DT.Rows[i][j] = sh.GetRow(i).GetCell(j).StringCellValue;
                                    //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;

                                    break;
                                case NPOI.SS.UserModel.CellType.String:
                                    DT.Rows[i][j] = sh.GetRow(i).GetCell(j).StringCellValue;

                                    break;
                            }
                        }
                    }

                    i++;
                }
            }
            finally
            {
                Thread.CurrentThread.CurrentCulture = prevCulture;
                //  Console.WriteLine(DT);
            }

            //HSSFWorkbook hssfwb;
            //using (FileStream file = new FileStream(@"" + ExcelFile + "", FileMode.Open, FileAccess.Read))
            //{
            //    hssfwb = new HSSFWorkbook(file);
            //}

            //ISheet sheet = hssfwb.GetSheet("Sheet1$");
            //for (int row = 0; row <= sheet.LastRowNum; row++)
            //{
            //    if (sheet.GetRow(row) != null) //null is when the row only contains empty cells
            //    {
            //        Console.WriteLine(string.Format("Row {0} = {1}", row, sheet.GetRow(row).GetCell(0).StringCellValue));
            //    }
            //}

            //////////////string con_string = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + ExcelFile + ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=1\"";

            //////////////OleDbConnection con = new OleDbConnection(con_string);
            //////////////OleDbDataAdapter data = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", con);

            //////////////DataTable rawData = new DataTable();
            //////////////data.Fill(rawData);


            return DT;
        }
        public static bool ConvertExcelToCsv(string excelFilePath)
        {
            string ExcelFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../" + excelFilePath + ""));
            string CsvFile = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"../../" + excelFilePath.Substring(0, excelFilePath.IndexOf('.')) + ".csv"));
            string DestinationCsvFileName = excelFilePath.Substring(0, excelFilePath.IndexOf('.')) + ".csv";

            using (var stream = new FileStream(ExcelFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader = null;
                if (ExcelFile.EndsWith(".xls", StringComparison.Ordinal))
                {
                    reader = ExcelReaderFactory.CreateBinaryReader(stream);
                }
                else if (ExcelFile.EndsWith(".xlsx", StringComparison.Ordinal))
                {
                    reader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                }

                if (reader == null)
                    return false;

                var ds = reader.AsDataSet(new ExcelDataSetConfiguration()
                {
                    ConfigureDataTable = (tableReader) => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = false
                    }
                });

                var csvContent = string.Empty;
                int row_no = 0;
                while (row_no < ds.Tables[0].Rows.Count)
                {
                    var arr = new List<string>();
                    for (int i = 0; i < ds.Tables[0].Columns.Count; i++)
                    {


                        arr.Add(ds.Tables[0].Rows[row_no][i].ToString());
                    }
                    row_no++;
                    csvContent += string.Join(",", arr) + "\n";
                }
                StreamWriter csv = new StreamWriter(CsvFile, false);
                csv.Write(csvContent);
                csv.Close();

                #region ReDefine the Name of the csv file to be inserted after creating the csv from excel
                PropertiesModel.CSV_FILE_NAME = DestinationCsvFileName;
                #endregion

                return true;
            }
        }
        //private void GetPrimaryKeys(DataTable table)
        //{
        //    // Create the array for the columns.
        //    DataColumn[] columns;
        //    columns = table.PrimaryKey;
        //    // Get the number of elements in the array.
        //    Console.WriteLine("Column Count: " + columns.Length);
        //    for (int i = 0; i < columns.Length; i++)
        //    {
        //        Console.WriteLine(columns[i].ColumnName + columns[i].DataType);
        //    }
        //}
    }

}
