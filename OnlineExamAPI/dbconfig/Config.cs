using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineExamAPI.dbconfig
{
    public class Config
    {
        private static string settingJson = "appsettings.json";
        public static AppSettings AppSettings()
        {
            var builder = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory()) 
                 .AddJsonFile(settingJson) 
                 .AddEnvironmentVariables(); 


            var config = builder.Build();
            var appConfig = new AppSettings();
            config.GetSection("AppSettings").Bind(appConfig);
            return appConfig;
        }

        public static string ConnectionString()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory())
                  .AddJsonFile(settingJson) 
                  .AddEnvironmentVariables(); 

            var config = builder.Build();
            var appConfig = new ConnectionStrings();
            config.GetSection("ConnectionStrings").Bind(appConfig);
            return appConfig.Connection;
        }
        public static string SqlServerConnectionString()
        {
            var builder = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetCurrentDirectory()) 
                  .AddJsonFile(settingJson);
            var config = builder.Build();
            var appConfig = new ConnectionStrings();
            config.GetSection("ConnectionStrings").Bind(appConfig);
            return appConfig.SqlServerConnection;
        }

    }
}
