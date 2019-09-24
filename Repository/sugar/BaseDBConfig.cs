using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

namespace Repository.sugar
{
    public   class BaseDBConfig
    {
        //public static string ConnectionString { get; set; } 

        //public static string ConnectionString = "Server=.;Database=Note;User ID=sa;Password=sasa;";//获取连接字符串 


        public static string ConnectionString = Appsettings.app(new string[] { "AppSettings", "SqlServerConnection" });//获取连接字符串
    }
}
