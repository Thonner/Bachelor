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
using System.IO;




namespace mnist_classification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Layers> layers = new List<Layers>();
        public int layerCount = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        internal List<Layers> Layers { get => layers; set => layers = value; }

        private void Load_cfg_Button_Click(object sender, RoutedEventArgs e)
        {
            //Load cfg file
            Console.WriteLine("Pressed load cfg");



            //Add the load file under here.

            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.Filter = "cfg Files (.cfg)|*.cfg|All Files (*.*)|*.*";

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
                    

                    string tempString;
                    long readPos = 0;
                    

                    while (!reader.EndOfStream)
                    {
                        reader.BaseStream.Position = readPos;
                        tempString = reader.ReadLine();
                        readPos = reader.BaseStream.Position;
                        if (tempString == "[convolutional]")
                        {
                            Layers.Add(new Layers());
                            layerCount++;
                            Layers[layerCount - 1].LayerType = "Conv";
                            Layers[layerCount - 1].Conv = new ConvLayer();


                            tempString = reader.ReadLine();
                            while(tempString.First() != '[')
                            {
                                switch (tempString.Substring(0, 3))
                                {
                                    case "fil":
                                        Layers[layerCount - 1].Conv.Filters = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        Layers[layerCount - 1].Conv.bias = new double[Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1))];
                                        break;

                                    case "siz":
                                        Layers[layerCount - 1].Conv.FilterSize = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "str":
                                        Layers[layerCount - 1].Conv.Stride = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "pad":
                                        Layers[layerCount - 1].Conv.Pad = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;
                                        
                                    default:
                                        break;
                                }
                                readPos = reader.BaseStream.Position;
                                tempString = reader.ReadLine();
                            }

                            Layers[layerCount - 1].Conv.FilterArray = new Filter[Layers[layerCount - 1].Conv.Filters];

                            for(int i = 0; i < Layers[layerCount - 1].Conv.Filters; i++)
                            {
                                Layers[layerCount - 1].Conv.FilterArray[i].Height = Layers[layerCount - 1].Conv.FilterSize;
                                Layers[layerCount - 1].Conv.FilterArray[i].Width = Layers[layerCount - 1].Conv.FilterSize;
                                //Layers[layerCount - 1].Conv.FilterArray[i].Depth = ?? should be calculated from preveos layer
                                //Layers[layerCount - 1].Conv.FilterArray[i].Weights = new float[Layers[layerCount - 1].Conv.FilterArray[i].Depth, Layers[layerCount - 1].Conv.FilterArray[i].Width, Layers[layerCount - 1].Conv.FilterArray[i].Height];

                            }


                        }
                        else if (tempString == "[connected]")
                        {
                            Layers.Add(new Layers());
                            layerCount++;
                            Layers[layerCount - 1].LayerType = "FC";
                            Layers[layerCount - 1].FC = new FCLayer();

                            tempString = reader.ReadLine();

                            while (tempString.First() != '[')
                            {
                                //Load settings for FC to be written

                                readPos = reader.BaseStream.Position;
                                tempString = reader.ReadLine();
                            }
                        }
                    }


                }
                fileStream.Close();
            }

        }

        private void Load_weights_Button_Click(object sender, RoutedEventArgs e)
        {
            //Load Weights file
            string path;

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
                    Console.WriteLine(reader.ReadLine()); // Left it here will not be used.

                    //I will experiment here and move it when i works - Anthon
                    path = fileDialog.FileName;
                    BinaryReader br = new BinaryReader(File.OpenRead(path));

                    int major, minor, revision, seen;

                    //Hard coded for test:

                    /*
                    float[, , ,] filterbankconv1 = new float[16, 3, 3, 3]; //filter bank from tiny-yolo
                    float[] biasBankConv1 = new float[16];
                    float[] scales = new float[16];
                    float[] rollingMeans = new float[16];
                    float[] rollingVarians = new float[16]

                    for(int bias = 0; bias < 16; bias++){
                        biasBankConv1[bias] = br.ReadSingle();
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        scales[i] = br.ReadSingle();
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        rollingMeans[i] = br.ReadSingle();
                    }

                    for (int i = 0; i < 16; i++)
                    {
                        rollingVarians[i] = br.ReadSingle();
                    }

                    for (int filterNr = 0; filterNr < 16; filterNr++)
                    {
                        for(int filterZ = 0; filterZ < 3; filterZ++)
                        {
                            for(int filterY = 0; filterY < 3; filterY++)
                            {
                                for(int filterX = 0; filterX < 3; filterX++)
                                {
                                    filterbankconv1[filterNr, filterZ, filterY, filterX] = br.ReadSingle();
                                }
                            }
                        }
                    }
                    */




                    layerCount = 0;

                    for(int i = 0; i < Layers.Count; i++)
                    {

                        if (Layers[i].LayerType == "Conv")
                        {
                            major = br.ReadInt32();
                            minor = br.ReadInt32();
                            revision = br.ReadInt32();
                            seen = br.ReadInt32();

                            for(int j = 0; j < Layers[i].Conv.Filters; j++)
                            {
                                Layers[i].Conv.bias[j] = br.ReadSingle();
                            }
                            // Write code that can determine wheter or not scales an

                            for (int filterNr = 0; filterNr < Layers[layerCount].Conv.Filters; filterNr++)
                            {
                                for (int filterZ = 0; filterZ < Layers[layerCount].Conv.FilterSize; filterZ++)
                                {
                                    for (int filterY = 0; filterY < Layers[layerCount].Conv.FilterSize; filterY++)
                                    {
                                        for (int filterX = 0; filterX < Layers[layerCount].Conv.InputDepth; filterX++)
                                        {
                                            Layers[layerCount].Conv.FilterArray[filterNr].Weights[filterZ, filterY, filterX] = br.ReadSingle();
                                        }
                                    }
                                }
                            }



                        }
                        

                    }


                }
                fileStream.Close();
            }
        }
    }
}
