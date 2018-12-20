using System;
using Oracle.ManagedDataAccess.Client;
using log4net;
using AIM_Interface.Common;
using System.Reflection;
using AIM_Interface.Models;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Data;

namespace AIM_Interface
{
    class apiMain
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            //Console.Write("Press any key to Run this Command");
            //Console.ReadKey();

            //Console.Clear();
            // ILog log = log4net.LogManager.GetLogger("NHibernateLogging");
            // var connectionString = DbConnection.ConnectionString.GetConnection();
            // var connectionStringMetadata = DbConnection.ConnectionString.GetMetadataConnection();
            PropertiesModel PublicProperties = new PropertiesModel();
#pragma warning disable CS0168 // The variable 'num' is declared but never used
            int num;
#pragma warning restore CS0168 // The variable 'num' is declared but never used

            #region Prompt Users to Enter API Number

            using (OracleConnection connection = new OracleConnection())
            {
                connection.ConnectionString = ConnectionProperties.connectionString;
                OracleCommand command = connection.CreateCommand();
                OracleCommand commandValue = connection.CreateCommand();
                connection.Open();
                // log.Info(string.Format("State: {0}", connection.State));

                string sqlApis = "SELECT * FROM API where  API_ACTIVE=-1 and API_HIDE = 0";
                command.CommandText = sqlApis;

                Console.WriteLine("");
                Console.WriteLine("Please chose an API Number:");
                Console.WriteLine("");
                //Console.WriteLine(args);

                OracleDataReader readerApis = command.ExecuteReader();
                while (readerApis.Read())
                {
                    int ID = (int)readerApis["PK_API"];
                    string myField = (string)readerApis["DSC_API"];

                    Console.WriteLine("ID: " + ID + " | Name: " + myField);

                    // log.Info(myField);
                }
            }
            #endregion Prompt Users to Enter Pk_SITE Value

            // This will open the connection and query the database
            try
            {
                List<string> ListOfParameters = new List<string>();

                for (int i = 0; i < args.Length; i++)
                {
                    var c = args[i];
                    // Console.WriteLine("Sent parameters :" + c);

                    int PassedValue = Convert.ToInt32(c);
                    PublicProperties.ParameterApiID = PassedValue;
                    ListOfParameters.Add(c);
                }


                // SQLiteDataReader readerValuee = fmd.ExecuteReader();


                //if (readerValuee.HasRows)
                //{
                //    while (readerValuee.Read())
                //    {
                //        int ID = (int)readerValuee["PK_EMAIL"];
                //        string myField = (string)readerValuee["EMAIL_MESSAGE"].ToString();
                //        Console.WriteLine(Convert.ToString(readerValuee["EMAIL_MESSAGE"]));
                //    }
                //}

                // AllData.Add(QueryEmailLayout);

                //foreach (var Item in AllData)
                //{
                //    Console.WriteLine(Item);
                //}



                //  while (true)
                //  {
                String command1;
                using (OracleConnection connection = new OracleConnection())
                {
                    connection.ConnectionString = ConnectionProperties.connectionString;
                    OracleCommand command = connection.CreateCommand();
                    OracleCommand commandValue = connection.CreateCommand();
                    connection.Open();
                    Console.WriteLine("Connection State: {0}", connection.State);

                    #region Manual enter API Id
                    while (true)
                    {
                        // String command1;
                        command1 = Console.ReadLine();
                        int EnteredNUmber = Convert.ToInt32(command1);

                        string sqlApisValue = "SELECT * FROM API where PK_API = " + EnteredNUmber + " and  API_ACTIVE=-1 and API_HIDE = 0 ";
                        commandValue.CommandText = sqlApisValue;

                        OracleDataReader readerValue = commandValue.ExecuteReader();

                        if (readerValue.HasRows)
                        {
                            while (readerValue.Read())
                            {
                                int ID = (int)readerValue["PK_API"];
                                string myField = (string)readerValue["DSC_API"];
                                // ValidApiNum = true;
                                runAPi((string)readerValue["PK_API"].ToString().Trim(), ID);
                                break;
                                // log.Info(myField);
                            }
                            break;
                        }
                        else
                        {
                            Console.WriteLine("API Id Not Found : " + command1);
                        }
                    }

                    #endregion End Manual Entrance

                    #region send parameter API ID from exe
                    foreach (var test in ListOfParameters)
                    {
                        //  Console.WriteLine("List parameters :" + test);

                        int PassedValue = Convert.ToInt32(test);
                        PublicProperties.ParameterApiID = PassedValue;

                        // Console.WriteLine(connectionString);



                        //String command1;
                        // Boolean ValidApiNum = false;


                        if (PublicProperties.ParameterApiID > 0)
                        {
                            string sqlApisValue = "SELECT * FROM API where PK_API = " + PublicProperties.ParameterApiID + " and API_ACTIVE=-1 and API_HIDE = 0";
                            commandValue.CommandText = sqlApisValue;
                        }
                        //else
                        //{
                        //    command1 = Console.ReadLine();
                        //    int EnteredNUmber = Convert.ToInt32(command1);

                        //    string sqlApisValue = "SELECT * FROM API where PK_API = " + EnteredNUmber + "";
                        //    commandValue.CommandText = sqlApisValue;

                        //}

                        OracleDataReader readerValue = commandValue.ExecuteReader();

                        if (readerValue.HasRows)
                        {
                            while (readerValue.Read())
                            {
                                int ID = (int)readerValue["PK_API"];
                                string myField = (string)readerValue["DSC_API"];
                                runAPi((string)readerValue["PK_API"].ToString().Trim(), (int)PublicProperties.ParameterApiID);
                                //log.Info(string.Format("State: {0}", connection.State));
                                //log.Info(DateTime.Now.ToString("M/dd/yyyy"));
                            }
                        }
                        else
                        {
                            Console.WriteLine("API Id Not Found : " + Console.ReadLine());
                        }



                    }
                    #endregion send parameter API ID from exe


                    connection.Close();
                    Console.WriteLine("Connection State: {0}", connection.State);
                }
                #region Enable it to keep console open and enter manually API ID
              Console.ReadLine();
                #endregion
            }
            catch (Exception ex)
            {
                //  Console.WriteLine(ex.Message);
                // DataLogInsert.InsertLog(ex.Message);
                log.Error("Error!", new Exception(ex.Message));
                //  log.Warn("Warning");
                Console.ReadLine();
            }

        }
        static void runAPi(string DFLT_PATH, int ApiID)
        {
            try
            {

                allmethods p = new allmethods();
                Type t = p.GetType();
                MethodInfo mi = t.GetMethod("apiCustom" + ApiID, BindingFlags.NonPublic | BindingFlags.Instance);
                string result = mi.Invoke(p, new object[] { ApiID }).ToString();

                // log.Info("Success! in Executing the API Method named '" + DFLT_PATH + "' in record PK_API: " + ApiID + ":" + result);
                // Console.WriteLine("Result = " + result);
                Console.ReadLine();

                // Console.Clear();


                ////////Process process = Process.Start(@"" + DFLT_PATH + "");
                ////////int id = process.Id;
                ////////Process tempProc = Process.GetProcessById(id);

                ////////tempProc.WaitForExit();


                //Console.ReadLine();
                // Start the process with the info we specified.
                // Call WaitForExit and then the using statement will close.
                //using (Process exeProcess = Process.Start(startInfo))
                //{
                //    exeProcess.WaitForExit();
                //}
            }
#pragma warning disable CS0168 // The variable 'ex' is declared but never used
            catch (Exception ex)
#pragma warning restore CS0168 // The variable 'ex' is declared but never used
            {
               // Console.WriteLine(ex.Message);
                // DataLogInsert.InsertLog(ex.Message);
                // log.Error("Error! in Executing the API Method named '"+ DFLT_PATH + "' in record PK_API: " + ApiID+ "", new Exception(ex.Message));
                // Console.ReadLine();
                //  log.Warn("Warning");
                // Log error.
            }
        }


    }

}
