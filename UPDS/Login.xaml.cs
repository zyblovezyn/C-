using System;
using System.Windows;
using Service;

namespace UPDS {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Login : Window {
        public Login () {
            InitializeComponent();
            this.Loaded += ( obj, env ) => {
                init();
            };
        }

        private TestService testService = new TestService();
        private void init () {
            registerClickEvent();
        }

        private void registerClickEvent () {
            this.btn_Login.Click += Btn_Login_Click;
            this.btn_Cancel.Click += Btn_Cancel_Click;
        }

        private void Btn_Cancel_Click ( object sender, RoutedEventArgs e ) {
            this.Close();
        }

        private void Btn_Login_Click ( object sender, RoutedEventArgs e ) {
            bool right = true;
            if ( right ) {
                // 进入主界面
            } else {

            }
        }

        public string test () {
            string str = testService.TestFunction();
            //MessageBox.Show(str);
            return str;
        }
    }
}
