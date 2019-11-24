using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class MaxLayer
    {

        public int InputWidth { get; set; }

        public int InputHeight { get; set; }

        public int InputDepth { get; set; }

        public int OutputWidth { get; set; }

        public int OutputHeight { get; set; }

        public int OutputDepth { get; set; }
        
        public int Size { get; set; }
        public int Stride { get; set; }



        //Input and output is given as width times height times depth.
        public Fix8[,,] Output { get; set; }

        public Fix8[,,] Input { get; set; }



        public void CalcMax()
        {

            Output = new Fix8[OutputWidth,OutputHeight,OutputDepth];

			Fix8[,] part = new Fix8[Size, Size];

            for (int k = 0; k < InputDepth; k++)
            {
                for (int i = 0; i < InputWidth; i+=Stride)
                {
                    for (int j = 0; j < InputHeight; j+=Stride)
                    {
                        FillPart(part, i, j, k);

                        Output[i/Stride, j/Stride, k] = Max(part);
                    }
                }
            }
        }

        private Fix8 Max(Fix8[,] part)
        {
			Fix8 max = 0;



            for(int i = 0; i < part.GetLength(0); i++)
            {
                for(int j = 0; j < part.GetLength(1); j++)
                {
                    if(part[i,j] > max)
                    {
                        max = part[i, j];
                    }
                }
            }



            return max;
        }

        private void FillPart(Fix8[,] part, int i, int j, int k)
        {
            //Fill temp array with values.
            for (int n = 0; n < Size; n++)
            {
                for (int m = 0; m < Size; m++)
                {
                    part[n, m] = Input[n + i, m + j, k];
                }
            }
        }





    }
}
