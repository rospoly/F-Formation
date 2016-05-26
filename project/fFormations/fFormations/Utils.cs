using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class Utils
    {
        public static void printMatrix(Matrix<double> matrix)
        {
            int spaces = 7;
            for (int j = 0; j < matrix.RowCount; j++)
            {
                for (int i = 0; i < matrix.ColumnCount; i++)
                {
                    double val = Math.Round(matrix[j, i], 2);
                    Console.Write(val + new String(' ', spaces-val.ToString().Count()));
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static double AngleDifference(double a, double b)
        {
            double d = a - b;
            return AngleNorm(d);
        }

        public static double AngleNorm(double d)
        {
            while (d < -Math.PI)
                d = d + 2 * Math.PI;
            while (d > Math.PI)
                d = d - 2 * Math.PI;
            return d;
        }

        public static double changeSign(double d)
        {
            if (d > 0)
                while (d > 0)
                    d = d - 2 * Math.PI;
            else
                while (d < 0)
                    d = d + 2 * Math.PI;
            return d;
        }
    }
}
