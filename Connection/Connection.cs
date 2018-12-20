using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.OracleClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DbConnection
{
    public static class ConnectionString
    {
        public static string GetConnection()
        {
            return ConfigurationManager.ConnectionStrings["MyConn"].ConnectionString;
        }
        public static string GetMetadataConnection()
        {
            return ConfigurationManager.ConnectionStrings["Metadata"].ConnectionString;
        }


    }

}
