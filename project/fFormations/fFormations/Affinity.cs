using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;

namespace fFormations
{
    public abstract class Affinity
    {
        private Matrix<double> Matrix;
        public int N { get; private set;}
        private Frame F;
        private bool Empty;
        public Affinity(Frame f)
        {
            this.F = f;
            N = f.N;
            Matrix = Matrix<double>.Build.Dense(N, N); 
            Empty = true;
        }
        public abstract void computeAffinity();
        public int getDimension()
        {
            return N;
        }

        public Matrix<double> getCopyMatrix() {
            Matrix<double> copy = Matrix.Clone();
            return copy;
        }
        public bool isEmpty() {
            return Empty;
        }
    }
}
