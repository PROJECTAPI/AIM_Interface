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
using System.Text.RegularExpressions;
using System.Data.SqlClient;
//using Oracle.DataAccess.Client;
//using Oracle.DataAccess.Client;

namespace AIM_Interface.Common
{

    public static class clsDevelopers
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string SAVE_CSV_FILE_WITHOUT_HEADER_ROW(string CsvFileName, int ApiID)
        {
            string result = "";
            string tableName = "";

            #region insert data into table (currently to APILOG and APIVER table for testing) data is grabbed from CSV File

            DataTable CSVDataNoHeader = new DataTable();

            #region if the file to be inserted is not csv is an Excel xlsx or xls then convert it automatically to csv and go do the insert
            if (CsvFileName.EndsWith(".xls", StringComparison.Ordinal) || CsvFileName.EndsWith(".xlsx", StringComparison.Ordinal))
            {
                //Services.ConvertExcelToCsv(CsvFileName);
                // CsvFileName = PropertiesModel.CSV_FILE_NAME;
                CSVDataNoHeader = Services.ConvertExcelToDatatable(CsvFileName);

            }
            else
            {
                #region convert csv file with HEADERS NAMES to datatable
                CSVDataNoHeader = Services.ReadCsvFileNoHeaderRow(CsvFileName);
                #endregion

            }



            #endregion



            //#region get number of records inside the csv (used to get the number of NextSequences Needed)
            //int NumOfRecords = CSVDataNoHeader.Rows.Count;
            //#endregion

            #region Converts Datatable to DataView which allows us to pick the columns we want to insert
            DataView view = new DataView(CSVDataNoHeader);
            #endregion

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                connection.Open();

                #region INSERT DATA FROM CSV INSIDE APILOG FOR TESTING PURPOSES

                tableName = "APILOG";

                //  List<int> NextSeqRangeArray = Services.GetNextSeqRange(tableName, NumOfRecords);

                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APILOG_Table = view.ToTable(false, "col0", "col1", "col2", "col3", "col5", "col6", "col8", "col4");


                APILOG_Table.Columns["col0"].ColumnName = "D_API_B";
                APILOG_Table.Columns["col1"].ColumnName = "T_API_B";
                APILOG_Table.Columns["col2"].ColumnName = "API_SUCCES";
                APILOG_Table.Columns["col3"].ColumnName = "TRANS_LOG";
                APILOG_Table.Columns["col5"].ColumnName = "PK_API";
                APILOG_Table.Columns["col6"].ColumnName = "D_CREATE";
                APILOG_Table.Columns["col8"].ColumnName = "T_CREATE";
                APILOG_Table.Columns["col4"].ColumnName = "PK_APILOG";


                APILOG_Table.AcceptChanges();

                #endregion

                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CSV File method: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APILOG_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {
                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST

                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];

                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APILOG SEquence ID
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArray)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;



                //                    }

                //                }
                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APILOG_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APILOG_Table.AcceptChanges();
                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID
                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    #region Get Next PK_APILOG SEquence ID
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArray)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;


                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID
                //        int index = 0;

                //        foreach (DataRow row in APILOG_Table.Rows)
                //        {
                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APILOG"] = GetNextSeq("APILOG");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArray)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APILOG"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPILOG_Table = APILOG_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion


                #region Call the Bulk Insert Method to insert the datatable filled from CSV
                Services.BulkInsertData(APILOG_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPILOG_Table), tableName, ApiID); //Services.ConvertStringArrayToStringJoin(columnNames)
                #endregion

                #endregion

                #region INSERT DATA FROM CSV INSIDE APIVER FOR TESTING PURPOSES

                tableName = "APIVER";

                //  List<int> NextSeqRangeArrayAPIVER = Services.GetNextSeqRange(tableName, NumOfRecords);
                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APIVER_Table = view.ToTable(false, "col9", "col10", "col5", "col6", "col8");

                APIVER_Table.Columns["col9"].ColumnName = "VER_NOTES";
                APIVER_Table.Columns["col10"].ColumnName = "DSC_API";
                APIVER_Table.Columns["col5"].ColumnName = "PK_API";
                APIVER_Table.Columns["col6"].ColumnName = "D_CREATE";
                APIVER_Table.Columns["col8"].ColumnName = "T_CREATE";
                //   APIVER_Table.Columns["col11"].ColumnName = "PK_APIVER";

                //#region add PK_APIVER to Datatable

                //APIVER_Table.Columns.Add("PK_APIVER", typeof(int));

                //#endregion
                APIVER_Table.AcceptChanges();


                #endregion

                //  DataTable APIVER_Table = SelectedColumns(CSVData, "VER_NOTES", "DSC_API", "PK_API", "D_CREATE", "T_CREATE", "PK_APIVER");

                #region Call the Bulk Insert Method to insert the datatable filled from CSV


                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CSV File method: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APIVER_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {
                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST Unless they are EMTPY
                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];

                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APIVER SEquence ID
                //                        // row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;



                //                    }

                //                }

                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APIVER_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APIVER_Table.AcceptChanges();

                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID

                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    #region Get Next PK_APIVER SEquence ID
                //                    //  row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;
                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID

                //        int index = 0;

                //        foreach (DataRow row in APIVER_Table.Rows)
                //        {

                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APIVER"] = GetNextSeq("APIVER");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APIVER"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPIVER_Table = APIVER_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion


                Services.BulkInsertData(APIVER_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPIVER_Table), tableName, ApiID);
                #endregion

                #endregion

            }
            #endregion

            return result;
        }
        public static string SAVE_CSV_FILE_WITH_HEADER_ROW(string CsvFileName, int ApiID)
        {
            string result = "";
            string tableName = "";

            #region insert data into table (currently to APILOG table for testing) data is grabbed from CSV File

            #region if the file to be inserted is not csv is an Excel xlsx or xls then convert it automatically to csv and go do the insert
            if (CsvFileName.EndsWith(".xls", StringComparison.Ordinal) || CsvFileName.EndsWith(".xlsx", StringComparison.Ordinal))
            {
                Services.ConvertExcelToCsv(CsvFileName);
                CsvFileName = PropertiesModel.CSV_FILE_NAME;
            }
            #endregion

            #region convert csv file with HEADERS NAMES to datatable
            DataTable CSVData = Services.ReadCsvFileWithHeaderRow(CsvFileName);
            #endregion

            //#region get number of records inside the csv (used to get the number of NextSequences Needed)
            //int NumOfRecords = CSVData.Rows.Count;
            //#endregion

            #region Converts Datatable to DataView which allows us to pick the columns we want to insert
            DataView view = new DataView(CSVData);
            #endregion

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                connection.Open();

                #region INSERT DATA FROM CSV INSIDE APILOG FOR TESTING PURPOSES
                tableName = "APILOG";

                //  List<int> NextSeqRangeArray = Services.GetNextSeqRange(tableName, NumOfRecords);

                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APILOG_Table = view.ToTable(false, "D_API_B", "T_API_B", "API_SUCCES", "PK_API", "D_CREATE", "T_CREATE", "PK_APILOG");

                //APILOG_Table.Columns["PK_APILOG"].DataType = Type.GetType("System.Int32");
                //APILOG_Table.AcceptChanges();

                #endregion


                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CSV File method: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APILOG_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {

                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST
                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];
                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APILOG SEquence ID
                //                        // row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APILOG");
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArray)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;
                //                    }

                //                }

                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APILOG_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APILOG_Table.AcceptChanges();
                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID
                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    #region Get Next PK_APILOG SEquence ID
                //                    // row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APILOG");
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArray)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;


                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID
                //        int index = 0;

                //        foreach (DataRow row in APILOG_Table.Rows)
                //        {
                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APILOG"] = GetNextSeq("APILOG");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArray)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APILOG"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPILOG_Table = APILOG_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion

                #region Call the Bulk Insert Method to insert the datatable filled from CSV
                Services.BulkInsertData(APILOG_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPILOG_Table), tableName, ApiID); //Services.ConvertStringArrayToStringJoin(columnNames)
                #endregion

                #endregion

                #region INSERT DATA FROM CSV INSIDE APIVER FOR TESTING PURPOSES
                tableName = "APIVER";

                // List<int> NextSeqRangeArrayAPIVER = Services.GetNextSeqRange(tableName, NumOfRecords);

                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APIVER_Table = view.ToTable(false, "VER_NOTES", "DSC_API", "PK_API", "D_CREATE", "T_CREATE", "PK_APIVER");

                #endregion

                #region Call the Bulk Insert Method to insert the datatable filled from CSV

                //#region set a primary key column
                //DataColumn[] columns1 = new DataColumn[1];
                //columns1[0] = APIVER_Table.Columns["PK_APIVER"];
                //APIVER_Table.PrimaryKey = columns1;
                //#endregion set a primary key column

                //////////////////////////////////////////////////////////// APIVER_Table.Columns.Add("PK_APIVER", typeof(System.Int32));


                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CSV File method: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APIVER_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {
                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST
                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];
                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APIVER SEquence ID
                //                        //  row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;



                //                    }

                //                }

                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APIVER_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APIVER_Table.AcceptChanges();

                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID
                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    #region Get Next PK_APIVER SEquence ID
                //                    //  row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;


                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID
                //        int index = 0;

                //        foreach (DataRow row in APIVER_Table.Rows)
                //        {
                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APIVER"] = GetNextSeq("APIVER");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APIVER"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPIVER_Table = APIVER_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion

                Services.BulkInsertData(APIVER_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPIVER_Table), tableName, ApiID);
                #endregion

                #endregion

            }
            #endregion

            return result;
        }
        public static string SAVE_QUERYVIEW_DATA(int ApiID)
        {
            string result = "";
            string tableName = "";

            #region insert data into table (currently to APILOG table for testing) data is grabbed from CSV File

            DataTable dt = new DataTable();

            #region get Data from QUERYVIEW and Fill them into a DATATABLE
            dt = Services.GetDataTable("Select * from g3_vw_search_cards");
            #endregion


            //#region get number of records inside the QUERYVIEW (used to get the number of NextSequences Needed)
            //int NumOfRecords = dt.Rows.Count;
            //#endregion

            #region Converts Datatable to DataView which allows us to pick the columns we want to insert
            DataView view = new DataView(dt);
            #endregion

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                connection.Open();

                #region INSERT DATA FROM QUERYVIEW INSIDE APILOG FOR TESTING PURPOSES
                tableName = "APILOG";

                // List<int> NextSeqRangeArray = Services.GetNextSeqRange(tableName, NumOfRecords);

                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APILOG_Table = view.ToTable(false, "D_API_B", "T_API_B", "API_SUCCES", "PK_API", "D_CREATE", "T_CREATE", "PK_APILOG");

                //APILOG_Table.Columns["PK_APILOG"].DataType = Type.GetType("System.Int32");
                //APILOG_Table.AcceptChanges();

                #endregion


                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CQUERYVIEW DATA: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APILOG_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {

                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST
                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];
                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APILOG SEquence ID
                //                        // row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APILOG");
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArray)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;
                //                    }

                //                }

                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APILOG_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APILOG_Table.AcceptChanges();
                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID
                //                int index = 0;

                //                foreach (DataRow row in APILOG_Table.Rows)
                //                {
                //                    #region Get Next PK_APILOG SEquence ID
                //                    // row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APILOG");
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArray)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;


                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID
                //        int index = 0;

                //        foreach (DataRow row in APILOG_Table.Rows)
                //        {
                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APILOG"] = GetNextSeq("APILOG");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArray)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APILOG"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPILOG_Table = APILOG_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion

                #region Call the Bulk Insert Method to insert the datatable filled from CSV
                Services.BulkInsertData(APILOG_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPILOG_Table), tableName, ApiID); //Services.ConvertStringArrayToStringJoin(columnNames)
                #endregion

                #endregion

                #region INSERT DATA FROM QUERYVIEW INSIDE APIVER FOR TESTING PURPOSES
                tableName = "APIVER";

                //  List<int> NextSeqRangeArrayAPIVER = Services.GetNextSeqRange(tableName, NumOfRecords);

                #region choose which columns to use from the loaded csv DATATABLE

                DataTable APIVER_Table = view.ToTable(false, "VER_NOTES", "DSC_API", "PK_API", "D_CREATE", "T_CREATE", "PK_APIVER");



                #endregion

                #region Call the Bulk Insert Method to insert the datatable filled from CSV

                //#region set a primary key column
                //DataColumn[] columns1 = new DataColumn[1];
                //columns1[0] = APIVER_Table.Columns["PK_APIVER"];
                //APIVER_Table.PrimaryKey = columns1;
                //#endregion set a primary key column

                //////////////////////////////////////////////////////////// APIVER_Table.Columns.Add("PK_APIVER", typeof(System.Int32));


                #region check if primary key Exist in the DATATABLE

                //using (SQLiteConnection SQLconnect = new SQLiteConnection())
                //{


                //    SQLconnect.ConnectionString = ConnectionProperties.connectionStringMetadata;
                //    // SQLiteCommand fmd = SQLconnect.CreateCommand();
                //    SQLconnect.Open();
                //    Console.WriteLine("Metdata Connection State In Reading the CSV File method: {0}", SQLconnect.State);

                //    SQLiteCommand cmd = new SQLiteCommand(@"SELECT * FROM QV_TABLE_PRIMARY_KEYS where TBL_NM = @Where and (SURROGATE=1) and (FLD_NM <>'');", SQLconnect);

                //    cmd.Parameters.Add("@Where", DbType.String).Value = tableName;

                //    SQLiteDataReader r = cmd.ExecuteReader();

                //    if (r.HasRows)
                //    {
                //        while (r.Read())
                //        {
                //            Console.WriteLine(r.IsDBNull(r.GetOrdinal("FLD_NM")) ? String.Empty : r.GetString(r.GetOrdinal("FLD_NM")));

                //            DataColumnCollection columns = APIVER_Table.Columns;
                //            //if (!r.IsDBNull(r.GetOrdinal("FLD_NM")))
                //            //{
                //            if (columns.Contains(r.GetString(r.GetOrdinal("FLD_NM"))))
                //            {
                //                // PrimaryKeyExist = true; USE PRIMARY KEY FIELD COMING FROM THE CSV BECAUSE IT EXISTS IN THE DATATABLE LIST
                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    string value = (string)row[r.GetString(r.GetOrdinal("FLD_NM"))];
                //                    if (string.IsNullOrEmpty(value))
                //                    {
                //                        #region Get Next PK_APIVER SEquence ID
                //                        //  row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                        int indexChild = 0;

                //                        foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                        {
                //                            if (index == indexChild)
                //                            {
                //                                row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                                break;
                //                            }
                //                            indexChild++;
                //                        }
                //                        #endregion
                //                        index++;



                //                    }

                //                }

                //            }
                //            else
                //            {
                //                //PrimaryKeyExist = false; CREATE PRIMARY KEY FIELD IN THE DATATABLE BECAUSE IT DOESN"T EXIST IN THE DATATABLE BUT IT EXIST IN THE METADATA
                //                //  break;
                //                #region Create Column with PRIMARY KEY FIELD Grabbed from MEtadata
                //                APIVER_Table.Columns.Add(r.GetString(r.GetOrdinal("FLD_NM")), typeof(int));
                //                APIVER_Table.AcceptChanges();

                //                #endregion

                //                #region Give every record in CSV file a NEXTVAL ID
                //                int index = 0;

                //                foreach (DataRow row in APIVER_Table.Rows)
                //                {
                //                    #region Get Next PK_APIVER SEquence ID
                //                    //  row[r.GetString(r.GetOrdinal("FLD_NM"))] = GetNextSeq("APIVER");
                //                    int indexChild = 0;

                //                    foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //                    {
                //                        if (index == indexChild)
                //                        {
                //                            row[r.GetString(r.GetOrdinal("FLD_NM"))] = NextSeq;//GetNextSeq("APILOG");
                //                            break;
                //                        }
                //                        indexChild++;
                //                    }
                //                    #endregion
                //                    index++;


                //                }

                //                #endregion
                //            }

                //        }
                //        //}
                //    }
                //    else
                //    {
                //        #region Give every record in CSV file a NEXTVAL ID
                //        int index = 0;

                //        foreach (DataRow row in APIVER_Table.Rows)
                //        {
                //            #region Get Next PK_APILOG SEquence ID
                //            // row["PK_APIVER"] = GetNextSeq("APIVER");
                //            int indexChild = 0;

                //            foreach (var NextSeq in NextSeqRangeArrayAPIVER)
                //            {
                //                if (index == indexChild)
                //                {
                //                    row["PK_APIVER"] = NextSeq;//GetNextSeq("APILOG");
                //                    break;
                //                }
                //                indexChild++;
                //            }
                //            #endregion
                //            index++;


                //        }

                //        #endregion
                //    }
                //    SQLconnect.Close();
                //}
                #endregion


                #region Convert Datatable to An Array to get the columns names dynamically
                var columnNamesAPIVER_Table = APIVER_Table.Columns.Cast<DataColumn>()
                             .Select(x => x.ColumnName.Replace("\r", ""))
                             .ToArray();
                #endregion

                Services.BulkInsertData(APIVER_Table, Services.ConvertStringArrayToStringJoin(columnNamesAPIVER_Table), tableName, ApiID);
                #endregion

                #endregion

            }
            #endregion

            return result;
        }

    }

}
