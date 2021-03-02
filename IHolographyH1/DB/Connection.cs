using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace IHolographyH1.DB
{
    static class Connection
    {
        public static string ConnStr{ get; private set; }

        //public static string GetConnection()
        //{
        //    return new ConfigurationManager.ConnectionStrings.ToString();
            
        //    //return new SqlConnection(ConnStr);
        //}
    }
}
