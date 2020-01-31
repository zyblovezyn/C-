using System.Windows;
using Service;

namespace UPDS {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow () {
            InitializeComponent();
            this.Loaded +=(obj,env)=>{
                test();
            };
        }

        private TestService testService = new TestService();
        public string test () {
            string str = testService.TestFunction();
            MessageBox.Show(str);
            return str;
        }
    }
}
