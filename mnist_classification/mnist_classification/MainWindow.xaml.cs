using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;




namespace mnist_classification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load_cfg_Button_Click(object sender, RoutedEventArgs e)
        {
            //Load cfg file
            Console.WriteLine("Pressed load cfg");



            //Add the load file under here.

            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.Filter = "Cfg Files (.cfg)|*.cfg|All Files (*.*)|*.*";

            fileDialog.FilterIndex = 1;


            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = fileDialog.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it to the console. No point.
                    Console.WriteLine( reader.ReadLine());
                }
                fileStream.Close();
            }

        }

        private void Load_weights_Button_Click(object sender, RoutedEventArgs e)
        {
            //Load Weights file
            Console.WriteLine("Pressed load weights");



            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.Filter = "Weight Files (.weights)|*.weights|All Files (*.*)|*.*";

            fileDialog.FilterIndex = 1;


            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                System.IO.Stream fileStream = fileDialog.OpenFile();

                using (System.IO.StreamReader reader = new System.IO.StreamReader(fileStream))
                {
                    // Read the first line from the file and write it to the console. No point.
                    Console.WriteLine(reader.ReadLine());
                }
                fileStream.Close();
            }
        }
    }
}
