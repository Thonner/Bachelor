using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mnist_classification
{

    class Fixed : System.Object
    {
        private ushort value;

        const int right = 10;
        const int left = 6;

        const int multiplier = 1024; //2^right;

        public Fixed(double value)
        {
            int temp = (ushort)(value * multiplier);


            this.value = (ushort)temp;
        }

        private Fixed(ushort value)
        {
            this.value = value;
        }


        public static Fixed operator + (Fixed f1, Fixed f2)
        {

            return new Fixed((ushort)(f1.value + f2.value));
        }

        public static Fixed operator -(Fixed f1, Fixed f2)
        {


            return new Fixed((ushort)(f1.value - f2.value));
        }

        public static Fixed operator *(Fixed f1, Fixed f2)
        {
            int temp = f1.value * f2.value;

            temp = temp >> right;

            return new Fixed((ushort)temp);
        }

        public static implicit operator Fixed(double v)
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
