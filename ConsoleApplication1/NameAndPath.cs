using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
   public class NameAndPath
    {
        private string name;
        private string path;

        public string Path { get => path; set => path = value; }
        public string Name { get => name; set => name = value; }
        public NameAndPath()
        {
                
        }
        public NameAndPath(string name,string path)
        {
            this.name = name;
            this.path = path;
        }
    }
}
