using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;


namespace fFormations
{
    class ModularityCut : Method
    {
        private DataManager dataManager;
        private Affinity affinity; //affinity matrix
        private Matrix<double> B; //modularity matrix
        private Matrix<double> A; //affinity matrix

        //constructor, requires a data manager??????
        public ModularityCut(DataManager dm)
        {
            dataManager = dm;
        }

        public void Initialize(Affinity a)
        {
            affinity = a;
            A = affinity.getCopyMatrix();
            computeModularityMatrix();
        }

        public List<Group> ComputeGroup()
        {
            throw new NotImplementedException();
        }

        private void computeModularityMatrix()
        {
            //create empty B
            B = Matrix<double>.Build.Dense(affinity.N, affinity.N);

            for (int i = 0; i < affinity.N; i++)
                for(int j = 0; j < affinity.N; j++)
                {

                }
        }

        //gives expected degree of element i wrt affinity matrix
        private double expectedDegree(int i)
        {
            double exp = 0;
            for (int j = 0; j < affinity.N; j++)
                exp += A[i, j];
            return exp;
        }
    }
}
