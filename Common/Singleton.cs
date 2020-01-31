using Common.DBHelper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    // singletom
    public class Singleton
    {
        private static DBHelper.OracleHelper uniqueOracleHelper;
        private readonly static object locker = new Object();

        private Singleton()
        {

        }

        public static DBHelper.OracleHelper GetOracleHelper()
        {
            if (uniqueOracleHelper == null)
            {
                lock (locker)
                {
                    if (uniqueOracleHelper == null)
                    {
                        uniqueOracleHelper = new DBHelper.OracleHelper();
                    }
                }
            }
            return uniqueOracleHelper;
        }
    }
}
