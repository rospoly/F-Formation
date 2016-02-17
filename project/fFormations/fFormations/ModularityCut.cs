﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;

namespace fFormations
{
    class ModularityCut : Method
    {
        private DataManager dataManager;
        private Affinity affinity; //affinity matrix
        private int N; //elements
        private double m; //normalization term
        private bool KLflag = false;
        private Matrix<double> B; //modularity matrix
        private Matrix<double> A; //affinity matrix

        //constructor, requires a data manager??????
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
            throw new NotImplementedException();
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

        //returns network modularity Q when the cut is partition
        private double getNetworkModularity(Matrix<double> partition)
        {
            double p = (partition.Transpose() * B * partition)[0, 0];
            return p / (4 * m);
        }

        //returns 1 and -1 vector
        private Matrix<double> getPartition(Matrix<double> vector)
        {
            Matrix<double> partition = Matrix<double>.Build.Dense(vector.RowCount, 1);
            for (int i = 0; i < vector.RowCount; i++)
                if (vector[i, 0] > 0)
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
    }
}
