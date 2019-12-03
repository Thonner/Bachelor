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




using System.Drawing;
using System.IO.Ports;
using System.Diagnostics;

namespace mnist_classification
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Layers> layers = new List<Layers>();
        public int layerCount = 0;
		public Dictionary<string, int> labels = new Dictionary<string, int>();
        public MainWindow()
        {
            FixedTest();

            InitializeComponent();
        }

        internal List<Layers> Layers { get => layers; set => layers = value; }



        private static void FixedTest()
        {
            //Example of how Fixed works.

            Fix8 fix1, fix2, fix3;

            fix1 = (Fix8) 2;
            fix2 = (Fix8) (-4);
            fix3 = fix1 + fix2;
            fix3 = fix1 - fix2;


            fix1 = (Fix8)17;
            fix2 = (Fix8)4;

            fix3 = fix2 * fix1;

            fix3 = fix2 * fix3;

        }


        private void Load_cfg_Button_Click(object sender, RoutedEventArgs e)
        {
            //Load cfg file
            int netHeight = 0, netWidth = 0, netChannels = 0;

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

                    reader.DiscardBufferedData();
                    while (!reader.EndOfStream)
                    {
                        
                        tempString = reader.ReadLine();

                        if (tempString == "[maxpool]")
                        {
                            Layers.Add(new Layers());
                            layerCount++;
                            Layers[layerCount - 1].LayerType = "Max";
                            Layers[layerCount - 1].Max = new MaxLayer();

                            while (tempString != "")
                            {
                                switch (tempString.Substring(0, 3))
                                {
                                    case "siz":
                                        Layers[layerCount - 1].Max.Size = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "str":
                                        Layers[layerCount - 1].Max.Stride = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    default:
                                        break;
                                }
                                tempString = reader.ReadLine();
                            }
                            
                            Layers[layerCount - 1].Max.InputDepth = Layers[layerCount - 2].Conv.OutputDepth;
                            Layers[layerCount - 1].Max.InputHeight = Layers[layerCount - 2].Conv.OutputHeight;
                            Layers[layerCount - 1].Max.InputWidth = Layers[layerCount - 2].Conv.OutputWidth;

                            Layers[layerCount - 1].Max.OutputDepth = Layers[layerCount - 1].Max.InputDepth;
                            Layers[layerCount - 1].Max.OutputWidth = Layers[layerCount - 1].Max.InputWidth / Layers[layerCount - 1].Max.Size;
                            Layers[layerCount - 1].Max.OutputHeight = Layers[layerCount - 1].Max.InputHeight / Layers[layerCount - 1].Max.Size;


                        }

                        if (tempString == "[net]")
                        {
                            while (tempString != "")
                            {


                                switch (tempString.Substring(0, 3))
                                {
                                    case "hei":

                                        netHeight = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "wid":

                                        netWidth = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "cha":

                                        netChannels = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    default:
                                        break;
                                }
                                tempString = reader.ReadLine();
                            }
                        }
                        
                        if (tempString == "[convolutional]")
                        {
                            Layers.Add(new Layers());
                            layerCount++;
                            Layers[layerCount - 1].LayerType = "Conv";
                            Layers[layerCount - 1].Conv = new ConvLayer();

                            if(layerCount == 1)
                            {
                                Layers[layerCount - 1].Conv.InputDepth = netChannels;
                                Layers[layerCount - 1].Conv.InputHeight = netHeight;
                                Layers[layerCount - 1].Conv.InputWidth = netWidth;

                            }
                            else
                            {
                                switch (Layers[layerCount - 2].LayerType)
                                {
                                    case "Conv":
                                        Layers[layerCount - 1].Conv.InputDepth = Layers[layerCount - 2].Conv.OutputDepth;
                                        Layers[layerCount - 1].Conv.InputHeight = Layers[layerCount - 2].Conv.OutputHeight;
                                        Layers[layerCount - 1].Conv.InputWidth = Layers[layerCount - 2].Conv.OutputWidth;
                                        break;

                                    case "Max":
                                        Layers[layerCount - 1].Conv.InputDepth = Layers[layerCount - 2].Max.OutputDepth;
                                        Layers[layerCount - 1].Conv.InputHeight = Layers[layerCount - 2].Max.OutputHeight;
                                        Layers[layerCount - 1].Conv.InputWidth = Layers[layerCount - 2].Max.OutputWidth;
                                        break;

                                    default:
                                        break;
                                }


                            }


                            tempString = reader.ReadLine();
                            while(tempString != "")
                            {
                                switch (tempString.Substring(0, 3))
                                {
                                    case "fil":
                                        Layers[layerCount - 1].Conv.Filters = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        Layers[layerCount - 1].Conv.Bias = new Fixed[Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1))];
                                        break;

                                    case "siz":
                                        Layers[layerCount - 1].Conv.FilterSize = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "str":
                                        Layers[layerCount - 1].Conv.Stride = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        break;

                                    case "pad":
                                        Layers[layerCount - 1].Conv.Pad = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 1));
                                        if(Layers[layerCount - 1].Conv.Pad == 1)
                                        {
                                            Layers[layerCount - 1].Conv.Pad = (Layers[layerCount - 1].Conv.FilterSize - 1) / 2;
                                        }

                                        break;
                                        
                                    default:
                                        break;
                                }
                                tempString = reader.ReadLine();
                            }

                            Layers[layerCount - 1].Conv.FilterArray = new Filter[Layers[layerCount - 1].Conv.Filters];

                            for(int i = 0; i < Layers[layerCount - 1].Conv.Filters; i++)
                            {
                                Layers[layerCount - 1].Conv.FilterArray[i] = new Filter();
                                Layers[layerCount - 1].Conv.FilterArray[i].Height = Layers[layerCount - 1].Conv.FilterSize;
                                Layers[layerCount - 1].Conv.FilterArray[i].Width = Layers[layerCount - 1].Conv.FilterSize;
                                if(layerCount == 1)
                                {
                                    Layers[layerCount - 1].Conv.FilterArray[i].Depth = netChannels;
                                }
                                else
                                {
                                    Layers[layerCount - 1].Conv.FilterArray[i].Depth = Layers[layerCount - 1].Conv.InputDepth;
                                }
                                //Layers[layerCount - 1].Conv.FilterArray[i].Depth = ?? should be calculated from preveos layer
                                Layers[layerCount - 1].Conv.FilterArray[i].Weights = new Fixed[Layers[layerCount - 1].Conv.FilterArray[i].Height, Layers[layerCount - 1].Conv.FilterArray[i].Width, Layers[layerCount - 1].Conv.FilterArray[i].Depth];

                            }

                            Layers[layerCount - 1].Conv.OutputDepth = Layers[layerCount - 1].Conv.Filters;
                            Layers[layerCount - 1].Conv.OutputHeight = (Layers[layerCount - 1].Conv.InputHeight + (Layers[layerCount - 1].Conv.Pad * 2) - Layers[layerCount - 1].Conv.FilterSize) / (Layers[layerCount - 1].Conv.Stride) + 1;
                            Layers[layerCount - 1].Conv.OutputWidth = (Layers[layerCount - 1].Conv.InputWidth + (Layers[layerCount - 1].Conv.Pad * 2) - Layers[layerCount - 1].Conv.FilterSize) / (Layers[layerCount - 1].Conv.Stride) + 1;


                        }
                        else if (tempString == "[connected]")
                        {
                            Layers.Add(new Layers());
                            layerCount++;
                            Layers[layerCount - 1].LayerType = "FC";
                            Layers[layerCount - 1].FC = new FCLayer();

                            tempString = reader.ReadLine();

                            while (tempString != "")
                            {
                                //Load settings for FC to be written

                                switch (tempString.Substring(0, 3))
                                {
                                    case "out":
                                        Layers[layerCount - 1].FC.OutputSize = Int32.Parse(tempString.Substring(tempString.IndexOf('=') + 2));
                                        break;

                                    default:

                                        break;
                                }

                                readPos = reader.BaseStream.Position;
                                tempString = reader.ReadLine();
                            }
                            
                            switch(Layers[layerCount - 2].LayerType)
                            {
                                case "Conv":
                                    Layers[layerCount - 1].FC.InputWidth = Layers[layerCount - 2].Conv.OutputWidth;
                                    Layers[layerCount - 1].FC.InputHeight = Layers[layerCount - 2].Conv.OutputHeight;
                                    Layers[layerCount - 1].FC.InputDepth = Layers[layerCount - 2].Conv.OutputDepth;

                                    Layers[layerCount - 1].FC.Weights = Layers[layerCount - 2].Conv.OutputWidth * Layers[layerCount - 2].Conv.OutputHeight * Layers[layerCount - 2].Conv.OutputDepth * Layers[layerCount - 1].FC.OutputSize;

                                    Layers[layerCount - 1].FC.WeightsArray = new Fixed[Layers[layerCount - 1].FC.OutputSize, Layers[layerCount - 1].FC.InputWidth * Layers[layerCount - 1].FC.InputHeight * Layers[layerCount - 1].FC.InputDepth];

                                    Layers[layerCount - 1].FC.Bias = new Fixed[Layers[layerCount - 1].FC.OutputSize];
                                    break;

                                case "FC":
                                    Layers[layerCount - 1].FC.InputHeight = Layers[layerCount - 2].FC.OutputSize;
                                    Layers[layerCount - 1].FC.InputHeight = 1;
                                    Layers[layerCount - 1].FC.InputDepth = 1;

                                    Layers[layerCount - 1].FC.Weights = Layers[layerCount - 1].FC.InputHeight * Layers[layerCount - 1].FC.OutputSize;

                                    Layers[layerCount - 1].FC.WeightsArray = new Fixed[Layers[layerCount - 1].FC.OutputSize, Layers[layerCount - 1].FC.InputHeight];

                                    Layers[layerCount - 1].FC.Bias = new Fixed[Layers[layerCount - 1].FC.OutputSize];

                                    break;

                                case "Max":
                                    Layers[layerCount - 1].FC.InputHeight = Layers[layerCount - 2].Max.OutputHeight;
                                    Layers[layerCount - 1].FC.InputWidth = Layers[layerCount - 2].Max.OutputWidth;
                                    Layers[layerCount - 1].FC.InputDepth = Layers[layerCount - 2].Max.OutputDepth;

                                    Layers[layerCount - 1].FC.Weights = Layers[layerCount - 2].Max.OutputWidth * Layers[layerCount - 2].Max.OutputHeight * Layers[layerCount - 2].Max.OutputDepth * Layers[layerCount - 1].FC.OutputSize;

                                    Layers[layerCount - 1].FC.WeightsArray = new Fixed[Layers[layerCount - 1].FC.OutputSize, Layers[layerCount - 1].FC.InputWidth * Layers[layerCount - 1].FC.InputHeight * Layers[layerCount - 1].FC.InputDepth];
                                    Layers[layerCount - 1].FC.Bias = new Fixed[Layers[layerCount - 1].FC.OutputSize];

                                    break;

                                default:
                                    break;
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

                    int major, minor, revision;
                    long seen;
                    
                    layerCount = 0;
                    major = br.ReadInt32();
                    minor = br.ReadInt32();
                    revision = br.ReadInt32();
                    seen = br.ReadInt64();

                    for (int i = 0; i < Layers.Count; i++)
                    {

                        if (Layers[i].LayerType == "Conv")
                        {
                            
                            for(int j = 0; j < Layers[i].Conv.Filters; j++)
                            {
                                Layers[i].Conv.Bias[j] =(Fixed) br.ReadSingle();
                            }

                            
                            // Write code that can determine wheter or not scales an

                            for (int filterNr = 0; filterNr < Layers[i].Conv.Filters; filterNr++)
                            {
                                for (int filterZ = 0; filterZ < Layers[i].Conv.InputDepth; filterZ++)
                                {
                                    for (int filterY = 0; filterY < Layers[i].Conv.FilterSize; filterY++)
                                    {
                                        for (int filterX = 0; filterX < Layers[i].Conv.FilterSize; filterX++)
                                        {
                                            Layers[i].Conv.FilterArray[filterNr].Weights[filterX, filterY, filterZ] = (Fixed) br.ReadSingle();
                                        }
                                    }
                                }
                            }
                        }
                        else if(Layers[i].LayerType == "FC")
                        {

                            for(int bias = 0; bias < Layers[i].FC.OutputSize; bias++)
                            {
                                Layers[i].FC.Bias[bias] = (Fixed)br.ReadSingle();
                            }
                            
                            for(int outputs = 0; outputs < Layers[i].FC.OutputSize; outputs++)
                            {
                                for(int inputs = 0; inputs < Layers[i].FC.InputDepth* Layers[i].FC.InputHeight* Layers[i].FC.InputWidth; inputs++)
                                {
                                    Layers[i].FC.WeightsArray[outputs, inputs] = (Fixed)br.ReadSingle();
                                }
                            }

                        }
                        //If Batch normalize has been set. scales means and varians should be loaded as well.
                    }
                }
                fileStream.Close();
            }
        }



        Bitmap pic;

        private void LoadPic_Button_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog fileDialog = new OpenFileDialog();


            fileDialog.Filter = "png Files (.png)|*.png|All Files (*.*)|*.*";

            fileDialog.FilterIndex = 1;


            bool? userClickedOK = fileDialog.ShowDialog();

            // Process input if the user clicked OK.
            if (userClickedOK == true)
            {
                // Open the selected file to read.
                pic = new Bitmap(fileDialog.FileName);

                //Make room for the file
                Layers[0].Conv.Input = new Fixed[Layers[0].Conv.InputWidth, Layers[0].Conv.InputHeight, Layers[0].Conv.InputDepth]; //Testing next line

            



                for (int i = 0; i < Layers[0].Conv.Input.GetLength(0); i++)
                {
                    for (int j = 0; j < Layers[0].Conv.Input.GetLength(1); j++)
                    {
                        System.Drawing.Color color = pic.GetPixel(i, j);

                        float red = 0, green = 0, blue = 0;


                        red = color.R / 255.0F;
                        green = color.G / 255.0F;
                        blue = color.B / 255.0F;
                        



                        Layers[0].Conv.Input[i, j, 0] = (Fixed)red;
                        Layers[0].Conv.Input[i, j, 1] = (Fixed)green;
                        Layers[0].Conv.Input[i, j, 2] = (Fixed)blue;



                    }
                }
            }
        }

        private void Calculate_Button_Click(object sender, RoutedEventArgs e)
        {
            Stopwatch stopwWatch = new Stopwatch();
            stopwWatch.Start();

            for (int i = 0; i < Layers.Count; i++)
            {
                switch (Layers[i].LayerType)
                {
                    case "Conv":
                        if(i != 0)
                        {
                            switch (Layers[i - 1].LayerType)
                            {
                                case "Max":
                                    layers[i].Conv.Input = Layers[i - 1].Max.Output;
                                    break;

                                default:
                                    throw new Exception("LAYER NOT RECOGNIZED");
                                    break;
                            }

                        }
                        Layers[i].Conv.CalcConv();

                        break;

                    case "Max":

                        if (Layers[i - 1].LayerType != "Conv") throw new Exception("WRONG LAYER BEFORE MAX");

                        Layers[i].Max.Input = Layers[i - 1].Conv.Output;

                        Layers[i].Max.CalcMax();

                        break;

                    case "FC":


                        switch (Layers[i-1].LayerType)
                        {
                            case "Max":
                                Layers[i].FC.Input = Layers[i - 1].Max.Output;
                                break;

                            case "FC":
                                Layers[i].FC.Input =  Layers[i - 1].FC.Output;
                                break;


                            default:
                                throw new Exception("LAYER NOT RECOGNIZED");
                                break;
                        }

                        Layers[i].FC.CalcFC();

                        break;

                       

                    default:
                        break;
                }
            }

            stopwWatch.Stop();

            Console.WriteLine("Run time: {0}", stopwWatch.ElapsedMilliseconds);

            StringBuilder builder = new StringBuilder();


            for (int i = 0; i < 10; i++)
            {
                builder.AppendFormat("{0}: {1}\n",i, Layers[Layers.Count - 1].FC.Output[0, 0, i]);
            }


            TextBoxResult.Text = builder.ToString();






        }

        private void Clear_pic_Button_Click(object sender, RoutedEventArgs e)
        {

            SaveVhdlRom();
            SaveVhdlFirstRom();
            SaveVhdlBias();
        }

        private void SaveVhdlBias()
        {
            var writer = new StreamWriter(@"C:\Users\simon\Desktop\biasRom.vhd");



            Console.WriteLine("PRINTING OUT");



            writer.WriteLine(@"library IEEE;
    use IEEE.std_logic_1164.all;
    use IEEE.numeric_std.all;

    use work.Types.all;

entity biasRom is
    port (
        clk: in  std_logic;
        rst: in  std_logic;");
            writer.WriteLine("\t\tlayer: in integer range 0 to {0};",(Layers.Count+1)/2);



            writer.WriteLine("\t\tfilter: in integer range 0 to {0};", Math.Max( Layers[0].Conv.Bias.Length - 1, Layers[2].Conv.Bias.Length - 1));
            writer.WriteLine(@"
        output: out signed(7 downto 0)
    );
end entity;

architecture rtl of biasRom is
    
begin
    
    
    process(all)
    begin
        if rising_edge(clk) then
            case layer is");

            for (int layer = 0; layer < (Layers.Count + 1) / 2; layer++) {

                if ((Layers.Count + 1) / 2 != layer+1 ) { 
                    writer.WriteLine("\t\t\t\twhen {0} =>", layer);


                    writer.WriteLine("\t\t\t\t\tcase filter is");

                    int filters = Layers[layer * 2].Conv.Filters;

                    for (int i = 0; i < filters; i++)
                    {
                        if (i != filters - 1)
                        {
                            writer.WriteLine("\t\t\t\t\t\twhen {0} =>", i);
                        }
                        else
                        {
                            writer.WriteLine("\t\t\t\t\t\twhen others =>");
                        }

                        string temp = Convert.ToString((Layers[layer * 2].Conv.Bias[i].value), 2).PadLeft(8, '0');


                        temp = temp.Remove(0, -8 + temp.Length);


                        writer.WriteLine("\t\t\t\t\t\t\toutput <= \"{0}\";", temp);

                    }

                    writer.WriteLine("\t\t\t\t\t\tend case;");
                }
                else
                {
                    writer.WriteLine("\t\t\t\twhen others =>");

                    writer.WriteLine("\t\t\t\t\tcase filter is");

                    int filters = Layers[layer * 2].FC.OutputSize;

                    for (int i = 0; i < filters; i++)
                    {
                        if (i != filters - 1)
                        {
                            writer.WriteLine("\t\t\t\t\t\twhen {0} =>", i);
                        }
                        else
                        {
                            writer.WriteLine("\t\t\t\t\t\twhen others =>");
                        }

                        string temp = Convert.ToString((Layers[layer * 2].FC.Bias[i].value), 2).PadLeft(8, '0');


                        temp = temp.Remove(0, -8 + temp.Length);


                        writer.WriteLine("\t\t\t\t\t\t\toutput <= \"{0}\";", temp);

                    }

                    writer.WriteLine("\t\t\t\t\t\tend case;");
                }


            }
            writer.WriteLine(@"            end case;
        end if;
    end process;
end architecture;");
            writer.Close();
        }
        

        public void SaveVhdlFirstRom()
        {
            var writer = new StreamWriter(@"C:\Users\simon\Desktop\FirstRom.vhd");



            Console.WriteLine("PRINTING OUT");



            writer.WriteLine(@"library IEEE;
    use IEEE.std_logic_1164.all;
    use IEEE.numeric_std.all;

    use work.Types.all;

entity FirstRom is
    port (
        clk: in  std_logic;");
            writer.WriteLine("\t\taddressX: in integer range 0 to {0}; ", Layers[0].Conv.InputWidth - 1);
            writer.WriteLine("\t\taddressY: in integer range 0 to {0};", Layers[0].Conv.InputHeight - 1);
            writer.WriteLine("\t\taddressZ: in integer range 0 to {0};", Layers[0].Conv.InputDepth - 1);
            writer.WriteLine(@"
        output: out unsigned(15 downto 0)
    );
end entity;

architecture rtl of FirstRom is
    
begin
    
    
    process(all)
    begin
        if rising_edge(clk) then 
");

            int x = Layers[0].Conv.InputWidth;
            int y = Layers[0].Conv.InputHeight;
            int depth = Layers[0].Conv.InputDepth;



                writer.WriteLine("\t\t\t\t\tcase addressX is");


                for (int j = 0; j < x; j++)
                {
                    if (j != x - 1)
                    {
                        writer.WriteLine("\t\t\t\t\t\twhen {0} =>", j);
                    }
                    else
                    {
                        writer.WriteLine("\t\t\t\t\t\twhen others =>");
                    }

                    writer.WriteLine("\t\t\t\t\t\t\tcase addressY is");


                    for (int k = 0; k < y; k++)
                    {
                        if (k != y - 1)
                        {
                            writer.WriteLine("\t\t\t\t\t\t\t\twhen {0} =>", k);
                        }
                        else
                        {
                            writer.WriteLine("\t\t\t\t\t\t\t\twhen others =>");
                        }

                        writer.WriteLine("\t\t\t\t\t\t\t\t\tcase addressZ is");


                        for (int l = 0; l < depth; l++)
                        {

                            if (l != depth - 1)
                            {
                                writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen {0} =>", l);
                            }
                            else
                            {
                                writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen others =>");
                            }

                            string temp = Convert.ToString((Layers[0].Conv.Input[j,k,l].value), 2).PadLeft(16, '0');

                        
                            writer.WriteLine("\t\t\t\t\t\t\t\t\t\t\toutput <= \"{0}\";", temp);


                        }

                        writer.WriteLine("\t\t\t\t\t\t\t\t\tend case;");

                    }
                    writer.WriteLine("\t\t\t\t\t\t\tend case;");
                }
                writer.WriteLine("\t\t\t\t\tend case;");


            writer.WriteLine(@"
        end if;
    end process;
end architecture;");
            writer.Close();
        }

        public void SaveVhdlRom()
        {
            var writer = new StreamWriter(@"C:\Users\simon\Desktop\weightsRom.vhd");



            Console.WriteLine("PRINTING OUT");



            writer.WriteLine(@"library IEEE;
    use IEEE.std_logic_1164.all;
    use IEEE.numeric_std.all;


entity weightsRom is
    generic (");
            writer.WriteLine("\t\taddressX: integer range 0 to {0}; ", Layers[0].Conv.FilterSize - 1);
            writer.WriteLine("\t\taddressY: integer range 0 to {0}", Layers[0].Conv.FilterSize - 1);
            writer.WriteLine(@"
    );
    port (
        clk: in  std_logic;
        rst: in  std_logic;");
            writer.WriteLine("\t\tlayer: in integer range 0 to {0};", (Layers.Count)/2);
            writer.WriteLine("\t\tfilter: in integer range 0 to {0};", Math.Max(Layers[0].Conv.Filters - 1, Layers[2].Conv.Filters - 1));

            writer.WriteLine("\t\taddressZ: in integer range 0 to {0};", Math.Max(Math.Max( Layers[0].Conv.InputDepth - 1, Layers[2].Conv.InputDepth - 1), Layers[Layers.Count-1].FC.InputDepth*4-1));
            writer.WriteLine(@"
        output: out signed(7 downto 0)
    );
end entity;

architecture rtl of weightsRom is
    --variable output1 : integer;
begin
    
        --output <= to_signed(output1,output'length);
    process(all)
    begin

        if rising_edge(clk) then
            
            case layer is");

            for (int layer = 0; layer <= (Layers.Count - 1) / 2; layer++)
            {

                if ((Layers.Count + 1) / 2 != layer + 1)
                {
                    writer.WriteLine("\t\t\t\twhen {0} =>", layer);



                    writer.WriteLine("\t\t\t\tcase filter is");

                    int filters = Layers[layer * 2].Conv.Filters;
                    int size = Layers[layer * 2].Conv.FilterSize;
                    int depth = Layers[layer * 2].Conv.InputDepth;

                    for (int i = 0; i < filters; i++)
                    {
                        if (i != filters - 1)
                        {
                            writer.WriteLine("\t\t\t\twhen {0} =>", i);
                        }
                        else
                        {
                            writer.WriteLine("\t\t\t\twhen others =>");
                        }


                        writer.WriteLine("\t\t\t\t\tcase addressX is");


                        for (int j = 0; j < size; j++)
                        {
                            if (j != size - 1)
                            {
                                writer.WriteLine("\t\t\t\t\t\twhen {0} =>", j);
                            }
                            else
                            {
                                writer.WriteLine("\t\t\t\t\t\twhen others =>");
                            }

                            writer.WriteLine("\t\t\t\t\t\t\tcase addressY is");


                            for (int k = 0; k < size; k++)
                            {
                                if (k != size - 1)
                                {
                                    writer.WriteLine("\t\t\t\t\t\t\t\twhen {0} =>", k);
                                }
                                else
                                {
                                    writer.WriteLine("\t\t\t\t\t\t\t\twhen others =>");
                                }

                                writer.WriteLine("\t\t\t\t\t\t\t\t\tcase addressZ is");


                                for (int l = 0; l < depth; l++)
                                {
                                    writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen {0} =>", l);


                                    string temp = Convert.ToString((Layers[layer * 2].Conv.FilterArray[i].Weights[j, k, l].value), 2).PadLeft(8, '0');

                                    temp = temp.Remove(0, -8 + temp.Length);

                                    //string temp = Convert.ToString((Layers[layer * 2].Conv.FilterArray[i].Weights[j, k, l].value));
                                    writer.WriteLine("\t\t\t\t\t\t\t\t\t\t\toutput <= \"{0}\";", temp);
                                    if (l != depth - 1)
                                    {

                                    }
                                    else
                                    {
                                        writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen others =>");
                                        writer.WriteLine("\t\t\t\t\t\t\t\t\t\t\toutput <= \"--------\";", temp);
                                    }




                                }

                                writer.WriteLine("\t\t\t\t\t\t\t\t\tend case;");

                            }
                            writer.WriteLine("\t\t\t\t\t\t\tend case;");
                        }
                        writer.WriteLine("\t\t\t\t\tend case;");

                    }
                    writer.WriteLine("\t\t\t\t\t\tend case;");
                }
                else
                {
                    writer.WriteLine("\t\t\t\twhen others =>");



                    //Save the weights into an easier data structure.




                    writer.WriteLine("\t\t\t\tcase filter is");

                    int filters = Layers[layer * 2].FC.OutputSize;
                    int size = Layers[0].Conv.FilterSize;
                    int depth = Layers[layer * 2].FC.InputDepth;


                    for (int i = 0; i < filters; i++)
                    {
                        Fixed[,,] values = new Fixed[7, 7, 64];

                        int counter = 0;

                        for (int z = 0; z < 64; z++)
                        {
                            for (int y = 0; y < 7; y++)
                            {
                                for (int x = 0; x < 7; x++)
                                {
                                    values[x, y, z] = Layers[layers.Count - 1].FC.WeightsArray[i, counter];
                                    counter++;
                                }
                            }
                        }



                        Fixed[,,] values2 = new Fixed[5, 5, 64 * 4];



                        for (int z = 0; z < 64 * 4; z++)
                        {
                            for (int y = 0; y < 5; y++)
                            {
                                for (int x = 0; x < 5; x++)
                                {

                                    int offsetX = 0;
                                    int offsetY = 0;

                                    if (x >= 2 && ((z >= 64 && z < 128) || (z >= 192)))
                                    {
                                        break;
                                    }

                                    if (y >= 2 && z >= 128)
                                    {
                                        break;
                                    }

                                    if (z >= 64)
                                    {
                                        offsetX += 5;
                                    }
                                    if (z >= 128)
                                    {
                                        offsetY += 5;
                                        offsetX -= 5;
                                    }
                                    if (z >= 192)
                                    {
                                        offsetX += 5;
                                    }

                                    if (values2[x, y, z] != null)
                                    {
                                        int gfsg = 0;
                                    }

                                    values2[x, y, z] = values[x + offsetX, y + offsetY, z % 64];

                                }
                            }
                        }




                        if (i != filters - 1)
                        {
                            writer.WriteLine("\t\t\t\twhen {0} =>", i);
                        }
                        else
                        {
                            writer.WriteLine("\t\t\t\twhen others =>");
                        }

                        writer.WriteLine("\t\t\t\t\tcase addressX is");

                        for (int j = 0; j < size; j++)
                        {


                            if (j != size - 1)
                            {
                                writer.WriteLine("\t\t\t\t\t\twhen {0} =>", j);
                            }
                            else
                            {
                                writer.WriteLine("\t\t\t\t\t\twhen others =>");
                            }

                            writer.WriteLine("\t\t\t\t\t\t\tcase addressY is");


                            for (int k = 0; k < size; k++)
                            {
                                if (k != size - 1)
                                {
                                    writer.WriteLine("\t\t\t\t\t\t\t\twhen {0} =>", k);
                                }
                                else
                                {
                                    writer.WriteLine("\t\t\t\t\t\t\t\twhen others =>");
                                }

                                writer.WriteLine("\t\t\t\t\t\t\t\t\tcase addressZ is");



                                int maxDepth = depth * 4;
                                bool skip64 = false;

                                if (k >= 2)
                                {
                                    maxDepth /= 2;
                                }
                                if (j >= 2)
                                {
                                    skip64 = true;
                                    maxDepth /= 2;
                                }

                                for (int l = 0; l < maxDepth; l++)
                                {
                                    int lNew = l;

                                    if (skip64)
                                    {
                                        if (l >= 64)
                                        {
                                            lNew += 64;
                                        }
                                    }


                                    int calcdepth = 0;

                                    calcdepth += lNew;

                                    //calcdepth += Layers[layer * 2].FC.InputWidth * j * Layers[layer * 2].FC.InputDepth;

                                    //calcdepth += Layers[layer * 2].FC.InputHeight * k * Layers[layer * 2].FC.InputDepth * size;

                                    //calcdepth += j * Layers[layer * 2].FC.InputDepth;

                                    //calcdepth += k * Layers[layer * 2].FC.InputDepth * Layers[layer * 2].FC.InputWidth;

                                    if (false)
                                    {//calcdepth >= Layers[layer * 2].FC.Weights / 10) {
                                    }
                                    else
                                    {
                                        bool lastElement = false;

                                        lastElement |= !(l != maxDepth - 1);

                                        writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen {0} =>", lNew);


                                        string temp = Convert.ToString((values2[j, k, lNew].value), 2).PadLeft(8, '0');
                                        //string temp = Convert.ToString((values2[j, k, lNew].value));

                                        temp = temp.Remove(0, -8 + temp.Length);

                                        writer.WriteLine("\t\t\t\t\t\t\t\t\t\t\toutput <= \"{0}\";",temp);

                                        if (!lastElement)
                                        {

                                        }
                                        else
                                        {
                                            writer.WriteLine("\t\t\t\t\t\t\t\t\t\twhen others =>");

                                            writer.WriteLine("\t\t\t\t\t\t\t\t\t\t\toutput <= \"--------\";");
                                        }


                                    }

                                }

                                writer.WriteLine("\t\t\t\t\t\t\t\t\tend case;");

                            }
                            writer.WriteLine("\t\t\t\t\t\t\tend case;");
                        }
                        writer.WriteLine("\t\t\t\t\tend case;");

                    }
                    writer.WriteLine("\t\t\t\t\t\tend case;");

                }


                
            }


            writer.WriteLine(@"            end case;
        end if;
    end process;
end architecture;");
            writer.Close();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listView.Items.Clear();
            string[] items = SerialPort.GetPortNames();
            foreach (string item in items)
            {
                listView.Items.Add(item);
                Console.WriteLine(item);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(listView.SelectedItem != null)
            {
                SerialPort UARTPort = new SerialPort();
                UARTPort.PortName = listView.SelectedItem.ToString();
                UARTPort.BaudRate = 115200;
                UARTPort.Parity = Parity.None;
                UARTPort.DataBits = 8;
                UARTPort.StopBits = StopBits.One;
                UARTPort.WriteTimeout = 500;
                UARTPort.Open();
                short Neuron = 0x0000;
                byte[] NeuronBytes;
                UARTPort.Write("S"); // this is the start signal for the FPGA

                for(int z = 0; z < Layers[0].Conv.InputDepth; z++)
                {
                    for(int y = 0; y < Layers[0].Conv.InputHeight; y++)
                    {
                        for(int x = 0; x < Layers[0].Conv.InputWidth; x++)
                        {
                            Neuron = Layers[0].Conv.Input[x, y, z].value;
                            NeuronBytes = BitConverter.GetBytes(Neuron);
                            UARTPort.Write(NeuronBytes, 0, 2);
                        }
                    }
                }
                UARTPort.Close();     

            }
        }

		private void ImageFolder_Clicked(object sender, RoutedEventArgs e)
		{
			float correct = 0.0F;
			float totalPics = 0.0F;
   
			OpenFileDialog fileDialog = new OpenFileDialog();


			fileDialog.Filter = "png Files (.png)|*.png|All Files (*.*)|*.*";

			fileDialog.FilterIndex = 1;

			
			bool? userClickedOK = fileDialog.ShowDialog();
			// Process input if the user clicked OK.
			if (userClickedOK == true)
			{
				string folderPath = fileDialog.FileName.Replace("\\"+fileDialog.SafeFileName, "");

				var pngs = Directory.EnumerateFiles(folderPath, "*.png");


				Layers[0].Conv.Input = new Fixed[Layers[0].Conv.InputWidth, Layers[0].Conv.InputHeight, Layers[0].Conv.InputDepth]; //Testing next line
				foreach (string thisPng in pngs)
				{
					totalPics++;
					pic = new Bitmap(thisPng);

					//Make room for the file
					
					for (int i = 0; i < Layers[0].Conv.Input.GetLength(0); i++)
					{
						for (int j = 0; j < Layers[0].Conv.Input.GetLength(1); j++)
						{
							System.Drawing.Color color = pic.GetPixel(i, j);

							float red = 0, green = 0, blue = 0;


							red = color.R / 255.0F;
							green = color.G / 255.0F;
							blue = color.B / 255.0F;




							Layers[0].Conv.Input[i, j, 0] = (Fixed)red;
							Layers[0].Conv.Input[i, j, 1] = (Fixed)green;
							Layers[0].Conv.Input[i, j, 2] = (Fixed)blue;



						}
					}

					for (int i = 0; i < Layers.Count; i++)
					{
						switch (Layers[i].LayerType)
						{
							case "Conv":
								if (i != 0)
								{
									switch (Layers[i - 1].LayerType)
									{
										case "Max":
											layers[i].Conv.Input = Layers[i - 1].Max.Output;
											break;

										default:
											throw new Exception("LAYER NOT RECOGNIZED");
											break;
									}

								}
								Layers[i].Conv.CalcConv();

								break;

							case "Max":

								if (Layers[i - 1].LayerType != "Conv") throw new Exception("WRONG LAYER BEFORE MAX");

								Layers[i].Max.Input = Layers[i - 1].Conv.Output;

								Layers[i].Max.CalcMax();

								break;

							case "FC":


								switch (Layers[i - 1].LayerType)
								{
									case "Max":
										Layers[i].FC.Input = Layers[i - 1].Max.Output;
										break;

									case "FC":
										Layers[i].FC.Input = Layers[i - 1].FC.Output;
										break;


									default:
										throw new Exception("LAYER NOT RECOGNIZED");
										break;
								}

								Layers[i].FC.CalcFC();

								break;



							default:
								break;
						}
					}


					StringBuilder builder = new StringBuilder();

					/*
					for (int i = 0; i < 10; i++)
					{
						builder.AppendFormat("{0}: {1}\n", i, Layers[Layers.Count - 1].FC.Output[0, 0, i]);
					}
					TextBoxResult.Text = builder.ToString();
					*/
					int maxIndex = 0;
					Fixed maxVal = 0;
					for (int i = 0; i < 10; i++)
					{
						if (Layers[Layers.Count - 1].FC.Output[0, 0, i] > maxVal)
						{
							maxVal = Layers[Layers.Count - 1].FC.Output[0, 0, i];
							maxIndex = i;
						}
	  ;
					}

					string searchkey = thisPng.Substring(folderPath.Length+1);

					if (labels[searchkey] == maxIndex)
					{
						correct++;

					}

				}
				
	float accuracy = correct/totalPics;
				Console.WriteLine(accuracy);
			}
		}

		private void LoadLabels_Clicked(object sender, RoutedEventArgs e)
		{
			OpenFileDialog fileDialog = new OpenFileDialog();

			fileDialog.Filter = "txt Files (.txt)|*.txt|All Files (*.*)|*.*";

			fileDialog.FilterIndex = 1;

			bool? userClickedOK = fileDialog.ShowDialog();

			string[] theLabels = File.ReadAllLines(fileDialog.FileName);

			string test = "0";
			int testint = Int32.Parse(test);


			for (int i = 0; i < theLabels.Length; i++)
			{
				labels.Add(theLabels[i].Substring(0, theLabels[i].IndexOf(":")), Int32.Parse(theLabels[i].Substring(theLabels[i].IndexOf(":") + 2)));
			}
		}
	}
}
