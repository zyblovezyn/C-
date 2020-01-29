using IBatisNet.DataMapper;
using log4net.Config;
using System;
using System.Windows;

namespace Wpf_UPDS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.init();
        }

        private void init()
        {
            addClickEventToBtn();
        }

        private void addClickEventToBtn()
        {
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "log4net.config"));
            executeFunction();
            this.btn_login.Click += Btn_login_Click;
            this.btn_exit.Click += Btn_exit_Click;
        }

        private void Btn_exit_Click(object sender, RoutedEventArgs e)
        {
             this.Close();
        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        public static ISqlMapper EntityMapper
        {
            get
            {
                try
                {
                    ISqlMapper mapper = Mapper.Instance();
                    mapper.DataSource.ConnectionString = "Data Source=(local);Initial Catalog=web;Integrated Security=True";
                    return mapper;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static int executeFunction()
        {

            ISqlMapper mapper = EntityMapper;

            int str = mapper.QueryForObject<int>("FindPageId", "Footer");
                             
            return str;

        }
 
    }
}
