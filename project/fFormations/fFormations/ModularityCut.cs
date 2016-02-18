using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace fFormations
{
    public class ModularityCut : Method
    {
        private DataManager dataManager;
        private Affinity affinity; //affinity matrix
        private int N; //elements
        private double m; //normalization term
        private bool KLflag = false;
        private Matrix<double> B; //modularity matrix
        private Matrix<double> A; //affinity matrix

        //constructor, requires a data manager??????
        // penso di no, è il gestoreIterazione l'unico ad iteragire con il data manager
        public ModularityCut(bool KL=false)
        {
            KLflag = KL; //if true, we apply kernighan-lin refinement
        }

        public void Initialize(Affinity a)
        {
            affinity = a;
            A = affinity.getCopyMatrix();
            N = affinity.getDimensionAM();

            computeNormTerm();
            computeModularityMatrix();
        }

        public List<Group> ComputeGroup()
        {
            Split first = firstSplit();
            if (first.isSplitable(Split.Parts.Left)) ;
            
            throw new NotImplementedException();
        }

        private Split firstSplit()
        {
            Matrix<double> e = getFirstEigenvector(B);
            return new Split(getPartition(e), B);
        }

        private void computeModularityMatrix()
        {
            //create empty B
            B = Matrix<double>.Build.Dense(N, N);

            for (int i = 0; i < N; i++)
                for(int j = 0; j < N; j++)
                {
                    B[i, j] = A[i, j] - (expectedDegree(i) * expectedDegree(j)) / 2 * m;
                }
        }

        //gives expected degree of element i wrt affinity matrix
        private double expectedDegree(int i)
        {
            double exp = 0;
            for (int j = 0; j < N; j++)
                exp += A[i, j];
            return exp;
        }

        //compute normalization term m
        private double computeNormTerm()
        {
            m = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    m += A[i, j];
            return m * 0.5;
        }

        //returns network modularity Q when the cut is the partition
        private double getNetworkModularity(Matrix<double> partition)
        {
            double p = (partition.Transpose() * B * partition)[0, 0];
            return p / (4 * m);
        }

        //returns 1 and -1 vector
        private Matrix<double> getPartition(Matrix<double> eigenVector)
        {
            Matrix<double> partition = Matrix<double>.Build.Dense(eigenVector.RowCount, 1);
            for (int i = 0; i < eigenVector.RowCount; i++)
                if (eigenVector[i, 0] > 0)
                    partition[i, 0] = 1;
                else
                    partition[i, 0] = -1;

            return partition;
        }

        //returns the first eigenvector of the matrix
        private Matrix<double> getFirstEigenvector(Matrix<double> matrix)
        {
            Evd<double> eigen = matrix.Evd();
            Matrix<double> vectors = eigen.EigenVectors;
            return vectors.SubMatrix(0, vectors.RowCount, 0, 1); //returns only 1st vector
        }


        class Split
        {
            private Matrix<double> s; //indicator vector
            private Matrix<double> B; //modularity matrix
            private List<int> right = new List<int>();
            private List<int> left = new List<int>();
            public enum Parts { Left, Right };

            public Split(Matrix<double> indicatorVector, Matrix<double> modMatrix)
            {
                s = indicatorVector;
                B = modMatrix;

                for (int i = 0; i < s.RowCount; i++)
                    if (s[i, 0] == 1) right.Add(i);
                    else
                        left.Add(i);
            }

            //checks if left or right part is splitable
            public bool isSplitable(Parts group)
            {
                return false;
            }

        }

    }
}
