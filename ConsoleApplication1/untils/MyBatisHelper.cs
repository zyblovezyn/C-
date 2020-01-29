using IBatisNet.Common.Utilities;
using IBatisNet.DataMapper;
using IBatisNet.DataMapper.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApplication1.untils {

    public sealed class MyBatisHelper {
        private static volatile ISqlMapper mapper = null;

        private static string GetConfigPath () {
            //string baseDir = AppDomain.CurrentDomain.BaseDirectory; 
            string baseDir = Path.GetFullPath(@"../../"); ;

            return baseDir + ConfigurationManager.AppSettings ["DBConfig"];  
        }

        public static void Configure ( object obj ) {
            mapper = ( ISqlMapper ) obj;
        }

        public static void InitMapper () {
            string configPath = GetConfigPath();
            ConfigureHandler hanlder = new ConfigureHandler(Configure);

            DomSqlMapBuilder builder = new DomSqlMapBuilder();
            mapper = builder.ConfigureAndWatch(configPath, hanlder);
        }

        public static ISqlMapper Instance {
            get {
                try {
                    if ( mapper == null ) {
                        lock ( typeof(SqlMapper) ) {
                            if ( mapper == null ) {
                                InitMapper();
                            }
                        }
                    }
                    return mapper;
                } catch ( Exception ex ) {
                    throw ex;
                }
            }
        }

        public static T QueryForObject<T> ( string statementName, object parameterObject ) {
            T result = default(T);
            try {
                result = Instance.QueryForObject<T>(statementName, parameterObject);
                return result;
            } catch ( Exception ex ) {
                throw ex;
            }
        }

        public static T QueryForList<T> ( string statementName, object parameterObject ) {
            T result = default(T);
            try {
                result = Instance.QueryForObject<T>(statementName, parameterObject);
                return result;
            } catch ( Exception ex ) {
                throw ex;
            }
        }
    }
}
