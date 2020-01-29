using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;
using System.IO;
using System.Windows;
using ConsoleApplication1.untils;
using System.Text.RegularExpressions;
namespace ConsoleApplication1 {
    public class Program {
        #region field
        static List<string> parametersList = new List<string>();
        //static List<Dictionary<string, string>> parameters = new List<Dictionary<string, string>>();
        static Dictionary<string, string> sqlParameters = new Dictionary<string, string>();

        static StringBuilder sqlStrBuilder = new StringBuilder();
        static List<string> listSql = new List<string>();
        static string complileStr = "[compile]";
        static string describeStr = "[describe]";
        static string executeStr = "[execute]";
        static string getStr = "[get";
        static string oraerrStr = "[oraerr]";
        static string bindStr = "[bind]";
        static string endline = "[end of fetch]";
        static bool sqlflag = false;
        static bool parametersFlag = false;
        static int startnum = 1;
        static List<string> listOracleKeyWord = new List<string>();
        #endregion end field

        public static String executeFunction () {
            string str = MyBatisHelper.QueryForObject<string>("FindPageId", null);
            return str;
        }

        [STAThread]
        static void Main ( string [] args ) {
            #region open log
            var logConfig = new FileInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log4net.config");
            XmlConfigurator.ConfigureAndWatch(logConfig);
            #endregion

            Console.WriteLine(executeFunction());

            List<NameAndPath> list = ConfigAppSettings.GetNameAndPaths();
            do {
                try {
                    int start = 1;
                    foreach ( var item in list ) {
                        Console.WriteLine(start++ + " " + item.Name);
                    }
                    Console.Write("please enter a number : ");
                    int num = ReadInt();
                    if(num<=0||num>list.Count){
                        System.Console.WriteLine("please enter a right number.");
                        continue;
                    }
                    listOracleKeyWord = getOracleKeyWord();
                    WriteSql(list [num - 1]);
                } catch ( Exception ex ) {
                    throw ex;
                }
            } while ( true );
        }

        static void WriteSql ( NameAndPath nameAndPath ) {
            string [] lines = File.ReadAllLines(nameAndPath.Path + "\\centura.log");

            List<List<string>> listlines = getAllStatement(lines);
            StringBuilder clipData = new StringBuilder();
            int flag = 1;
            foreach ( var list in listlines ) {
                flag++;
                getSqlStatement(list);
                List<Dictionary<string,string>> parameters=getSqlParameters(list);
                WriteFormatSql(listSql, parameters, clipData);
                listSql.Clear();
                sqlParameters.Clear();
            }
            startnum = 1;
            Clipboard.Clear();
            Clipboard.SetText(clipData.ToString());
        }

        private static void WriteFormatSql ( List<string> listsql, List<Dictionary<string, string>> parameters, StringBuilder clipData ) {
            try {
                foreach ( var sql in listsql ) {
                    List<int> removeIndex=new List<int>();
                    if ( sql.Contains(":1") ) {
                        if ( parameters.Count > 0 ) {
                            StringBuilder resSql = new StringBuilder();
                            List<string> resStrs = sql.Replace(",", " , ").Replace(":", " :").Replace(")", " )").Split(' ').ToList();
                            for (int i = 0; i < parameters.Count; i++)
                            {
                                foreach (var keyValue in parameters[i])
                                {
                                    for ( int j = 0; j < resStrs.Count; j++ ) {
                                        if ( resStrs [j].Equals(keyValue.Key) ) {
                                            resStrs [j] = keyValue.Value;
                                            removeIndex.Add(i);
                                            continue;
                                        }
                                    }
                                }
                            }
                            foreach ( var s in resStrs ) {
                                resSql.Append(" " + s + " ");
                            }
                            FormatStatement(clipData, resSql.ToString());
                        }
                    } else {
                        FormatStatement(clipData, sql);
                    }
                    for (int i = 0; i < removeIndex.Count; i++)
                    {
                        parameters.RemoveAt(0);
                    }
                }
            } catch ( Exception ex ) {
                throw ex;
            }
        }

        private static void FormatStatement ( StringBuilder clipData, string sql ) {
            string str = "------ " + startnum++ + "  ------";
            string formatSql = MergeSpace(sql) + ";";
            Console.WriteLine(str);
            Console.WriteLine(formatSql);
            clipData.AppendLine(str);
            clipData.AppendLine(formatSql);
        }

        private static int ReadInt () {
            int number = 0;
            try {
                //将根据提示输入的数字字符串转换成int型 
                //Console.ReadLine(),这个函数，是以回车判断字符串结束的
                //
                number = Convert.ToInt32(Console.ReadLine());//与下面的效果一样
            } catch {
                Console.WriteLine("输入有误，重新输入！");
            }

            return number;

        }

        #region 字符串中多个连续空格转为一个空格
        /// <summary>
        /// 字符串中多个连续空格转为一个空格
        /// </summary>
        /// <param name="str">待处理的字符串</param>
        /// <returns>合并空格后的字符串</returns>
        public static string MergeSpace ( string str ) {
            if ( str != string.Empty &&
                str != null &&
                str.Length > 0
                ) {
                str = new System.Text.RegularExpressions.Regex("[\\s]+").Replace(str, " ");
            }
            return str;
        }


        #endregion


        private static List<Dictionary<string, string>> GetParameters ( List<string> list ) {
            List<Dictionary<string,string>> parameters;
            try {
                parameters=new List<Dictionary<string, string>>();
                foreach ( var str in list ) {
                    Dictionary<string,string> dic=new Dictionary<string, string>();
                    string [] temp = str.Trim().Split(' ');
                    if ( temp.Length <= 0 || temp.Length != 2 )
                        continue;
                    temp [0] = ":" + temp [0].Replace('[', ' ').Replace(']', ' ').Trim();
                    string res = temp [1].Trim();
                    if ( res.Contains("-") && res.Contains(".") ) {
                        res = "cast(to_timestamp('" + res + "'," + "'yyyy-mm-dd hh24:mi:ss.ff') as date)";
                        temp [1] = res;
                    } else {
                        temp [1] = "'" + temp [1].Trim() + "'";
                    }
                    dic.Add(temp [0], temp [1]);
                    parameters.Add(dic);
                }
                return parameters;
            } catch ( Exception ex ) {
                throw ex;
            }
        }


        private static List<List<string>> getAllStatement ( string [] lines ) {
            List<List<string>> listAll = new List<List<string>>();
            List<string> list = new List<string>();
            bool flag = false;

            foreach ( string line in lines ) {
                int index = line.IndexOf(">");
                string lineStr = MergeSpace(line.Substring(index + 1, line.Length - ( index + 1 )).Trim()).ToLower();
                if ( lineStr.StartsWith(complileStr) ) {
                    flag = true;
                }
                if ( lineStr.StartsWith(endline) ) {
                    flag = false;
                    list.Add(lineStr);
                    listAll.Add(new List<string>(list));
                    list.Clear();
                }
                if ( flag ) {
                    list.Add(lineStr);
                }
            }
            return listAll;
        }


        private static void getSqlStatement ( List<string> listlines ) {
            foreach ( string lineStr1 in listlines ) {
                if ( string.IsNullOrWhiteSpace(lineStr1) )
                    continue;
                string lineStr = lineStr1.Trim();
                if ( lineStr.StartsWith(complileStr) ) {
                    sqlflag = true;
                }
                if ( lineStr.StartsWith(describeStr) || lineStr.StartsWith(executeStr) || lineStr.StartsWith(getStr) || lineStr.StartsWith(oraerrStr) ) {
                    sqlflag = false;
                    if ( sqlStrBuilder.Length <= 0 )
                        continue;
                    listSql.Add(sqlStrBuilder.ToString());
                    sqlStrBuilder.Clear();
                }
                if ( sqlflag ) {
                    if ( lineStr.StartsWith(complileStr) ) {
                        sqlStrBuilder.Append(lineStr.Replace(complileStr, ""));
                    } else {
                        if ( StrEndMatchOracleKeyWord(sqlStrBuilder, listOracleKeyWord) ) {
                            sqlStrBuilder.Append(" " + lineStr + " ");
                            continue;
                        }
                        if ( StrStartMatchOracleKeyWord(lineStr, listOracleKeyWord) ) {
                            sqlStrBuilder.Append(" " + lineStr);
                            continue;
                        }
                        sqlStrBuilder = new StringBuilder(sqlStrBuilder.ToString().Trim());
                        sqlStrBuilder.Append(lineStr.Trim());
                    }
                }
            }

        }


        private static bool StrStartMatchOracleKeyWord ( string originalStr, List<string> listKeyWord ) {
            foreach ( var item in listKeyWord ) {
                if ( originalStr.StartsWith(item + " ") || originalStr == item ) {
                    return true;
                }
            }
            return false;
        }
        private static bool StrEndMatchOracleKeyWord ( StringBuilder originalStr, List<string> listKeyWord ) {
            foreach ( var item in listKeyWord ) {
                if ( originalStr.ToString().EndsWith(item + " ") || originalStr.ToString().EndsWith(" " + item) ) {
                    return true;
                }
            }
            return false;
        }
        private static List<Dictionary<string, string>> getSqlParameters ( List<string> listlines ) {
            try {
                parametersList.Clear();
                foreach ( string lineStr in listlines ) {
                    if ( Regex.IsMatch(lineStr, @"\[\d\]") ) {
                        parametersList.Add(lineStr);
                    }
                }
                return GetParameters(parametersList);
            } catch ( Exception ex ) {
                throw ex;
            }
        }

        private static List<string> getOracleKeyWord () {
            if ( listOracleKeyWord.Count > 0 ) {
                return listOracleKeyWord;
            }
            listOracleKeyWord.Add("(");
            listOracleKeyWord.Add(")");
            listOracleKeyWord.Add("+");
            listOracleKeyWord.Add("=");
            listOracleKeyWord.Add("sysdba");
            listOracleKeyWord.Add("sysdate");
            listOracleKeyWord.Add("from");
            listOracleKeyWord.Add("where");
            listOracleKeyWord.Add("and");
            listOracleKeyWord.Add("between");
            listOracleKeyWord.Add("select");
            listOracleKeyWord.Add("or");
            listOracleKeyWord.Add("order");
            listOracleKeyWord.Add("group");
            listOracleKeyWord.Add("as");
            return listOracleKeyWord;
        }
    }
}