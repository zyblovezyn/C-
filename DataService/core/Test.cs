using Common.DBHelper;
using DataService.inter;

namespace DataService.core {
    public class Test : ITest {
       public string TestFunction () {
            string str = OracleHelper.Select<string>("select name from xmmessage where code = '2001'");
            return str;
        }
    }
}
