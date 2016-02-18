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
        protected Matrix<double> AdjacencyMatrix { get; set; }
        protected Frame F { get; private set; }

        public Affinity(Frame f)
        {
            this.F = f;
            //AdjacencyMatrix = Matrix<double>.Build.Dense(f.N, f.N);
            computeAffinity();
        }
        public abstract void computeAffinity();
        public int getDimensionAM() {
            return F.N;
        }
        public Matrix<double> getCopyMatrix() {
            Matrix<double> copy = AdjacencyMatrix.Clone();
            return copy;
        }
        public static void main() {

        }
    }
}
