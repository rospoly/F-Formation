using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class DominantSet:Method
    {
        Affinity a;
        Matrix<double> matrix;//matrice di affinità
        Vector<double> vector;//vettori per la ricerca dei raggruppamenti
        DominantSet() {

        }
        
        public void Initialize(Affinity a)
        {
            this.a = a;
            matrix = a.getCopyMatrix();
            ResetVector();
        }
    
        public List<Group> ComputeGroup()
        {

        }

        public void ResetVector() {
            vector = Vector<double>.Build.Dense(matrix.ColumnCount, 1.0 / (double)matrix.ColumnCount);
        }

        public double ComputeFunction() {
            return (vector.ToColumnMatrix().Multiply(matrix).Multiply(vector)).Single();
        }

        public bool CheckValidFunction() {
            //controllare
            return ((vector.Sum() == 1) && (vector.ForAll(x => x >= 0))); 
        }

        public void RecursiveResearchMax()
        {
            vector = vector.ToRowMatrix().Multiply(matrix.Multiply(vector) / ComputeFunction());
        }
         
    }
    class Context
    {

    }
    class Local : Context
    {

    }
    class Global : Context
    {

    }
}
