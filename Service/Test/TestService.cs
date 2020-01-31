using System;
using DataService.core;

namespace Service {
    public class TestService {
        public TestService () {

        }
        private readonly Test test=new Test();


        public string TestFunction () {
            return test.TestFunction();
        }
    }
}
