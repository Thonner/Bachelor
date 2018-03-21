using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{

    class Fixed : System.Object
    {
        public short value;

        const int right = 10; //10
        const int left = 6; //6

        const int multiplier = 1024; //2^right;

        private Fixed(short value)
        {
            this.value = value;
        }


        public Fixed(double value)
        {
            int temp = (short)(value * multiplier);


            if (temp < Int16.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int16.MinValue;
            }
            else if (temp > Int16.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int16.MaxValue;
            }


            this.value = (short)temp;
        }

        public Fixed(int value)
        {
            int temp = (short)(value * multiplier);


            if (temp < Int16.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int16.MinValue;
            }
            else if (temp > Int16.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int16.MaxValue;
            }

            this.value = (short)temp;
        }



        public static Fixed operator + (Fixed f1, Fixed f2)
        {
            if (f1 == null)
            {
                return f2;
            }

            int temp = (f1.value + f2.value);

            //If both are negative, result should be negative
            if(f1.value < 0 && f2.value < 0)
            {
                if(temp < Int16.MinValue )
                {
                    //Underflow occured, set to min value.
                    temp = Int16.MinValue;
                }
                //If both are positive, result should be positive.
            }else if(f1.value >= 0 && f2.value >= 0)
            {
                if(temp > Int16.MaxValue)
                {
                    //Overflow occured, set to max value.
                    temp = Int16.MaxValue;
                }
            }
            //If one is negative and the other is positive, it will not overflow.

            return new Fixed((short)temp);
        }

        public static Fixed operator -(Fixed f1, Fixed f2)
        {

            //We invert the right side and can use our adder.
            Fixed temp = new Fixed((short)(0 - f2.value));


            return f1 + temp;
        }

        public static Fixed operator *(Fixed f1, Fixed f2)
        {
            int temp = f1.value * f2.value;

            temp = temp >> right;


            if (temp < Int16.MinValue)
            {
                //Underflow occured, set to min value.
                temp = Int16.MinValue;
            } else if (temp > Int16.MaxValue)
            {
                //Overflow occured, set to max value.
                temp = Int16.MaxValue;
            }


            return new Fixed((short)temp);
        }

        public static explicit operator Fixed(double v)
        {
            return new Fixed(v);
        }

        public static implicit operator Fixed(int v)
        {
            return new Fixed(v);
        }

        public static implicit operator Double(Fixed v)
        {
            return v.Double();
        }

        public static implicit operator float(Fixed v)
        {
            return (float)v.Double();
        }

        public double Double()
        {
            double temp = value;

            temp /= multiplier;

            return temp;
        }

        public override string ToString()
        {
            return this.Double().ToString();
        }

    }
}
