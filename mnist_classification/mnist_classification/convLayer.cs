﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{
    class ConvLayer
    {
        public int InputWidth { get; set; }

        public int InputHeight { get; set; }

        public int InputDepth { get; set; }

        public int OutputWidth { get; set; }

        public int OutputHeight { get; set; }

        public int OutputDepth { get; set; }

        public int Filters { get; set; }

        public int FilterSize { get; set; }

        public int Stride { get; set; }

        public int Pad { get; set; }

        public Filter[] FilterArray { get; set; }

        public Fix8[] Bias { get; set; }


        //Input and output is given as width times height times depth.
        public Fix8[,,] Output { get; set; }

        public Fix8[,,] Input { get; set; }






        public void CalcConv()
        {

            Output = new Fix8[OutputWidth, OutputHeight, OutputDepth];

			//Part of the array, used for convolution.
			Fix8[,,] part = new Fix8[FilterSize, FilterSize, InputDepth];


            for (int i = 0; i < InputWidth; i += Stride)
            {
                for (int j = 0; j < InputHeight; j += Stride)
                {
                    //Fill the part of the array that is to be calculated.
                    FillPart(part, i - Pad, j - Pad);


                    int depth = 0;
                    //Multiply each filter onto the array.
                    foreach (Filter filter in FilterArray)
                    {

                        //Max for ReLu function.
                        Output[i, j, depth] =  Max(MultFilter(filter, part) + Bias[depth],0);


                        depth++;
                    }
                }
            }
        }

        private Fix8 Max(Fix8 a, Fix8 b)
        {
            if (a > b) return a;
            return b;
        }

        private Fix8 MultFilter(Filter filter, Fix8[,,] part)
        {
			Fix8 sum = 0;

            for(int i = 0; i< filter.Depth; i++)
            {
                for(int j = 0; j < filter.Height; j++)
                {
                    for(int k = 0; k< filter.Width; k++)
                    {
                        sum += part[k, j, i] *  filter.Weights[k, j, i];
                    }
                }
            }
            return sum;
        }

        private void FillPart(Fix8[,,] part, int i, int j)
        {
            //Fill temp array with values.
            for (int n = 0; n < FilterSize; n++)
            {
                for(int m = 0; m <FilterSize; m++)
                {
                    for (int k = 0; k < InputDepth; k++)
                    {
						Fix8 temp = 0;

                        if (n + i >= InputWidth) {
                            temp = 0;
                        }
                        else if (m + j >= InputHeight)
                        {
                            temp = 0;
                        }
                        else if (n + i < 0 || m + j < 0) { temp = 0; }
                        else
                        {
                            temp = Input[n + i, m + j, k];
                        }


                        part[n, m, k] = temp;


                    }
                }
            }
        }
    }

}
