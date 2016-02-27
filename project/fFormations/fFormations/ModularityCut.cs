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
        protected Affinity affinity; //affinity matrix
        
        private int N; //elements
        private double m; //normalization term
        private bool KLflag = false;
        private Matrix<double> B; //modularity matrix for entire network
        private Matrix<double> A; //affinity matrix

        private Split firstSplit;
      //  private List<Split> groups;

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

            List<int> elementsID = new List<int>(); //contains all helplabels
            for (int i = 0; i < N; i++)
                elementsID.Add(a.F.Persons[i].HelpLabel);

            computeNormTerm();
            computeModularityMatrix();

            firstSplit = new Split(elementsID, this); //the first split contains all elements
        }

        public Group ComputeGroup()
        {
            List<Split> groups = new List<Split>();
            Group result = new Group(affinity.F);

            Queue<Split> q = new Queue<Split>();
            q.Enqueue(firstSplit);
            groups.Add(firstSplit);

            while (q.Count != 0)
            {
                Tuple<List<int>, List<int>> res;
                Split current = q.Dequeue();
                if(current.divide(out res))
                {
                    groups.Remove(current); //remove current split --> usa equals
                    if (res.Item1 != null)
                    {
                        Split sub1 = new Split(res.Item1, this);
                        groups.Add(sub1);
                        q.Enqueue(sub1);
                    }
                    if (res.Item2 != null)
                    {
                        Split sub2 = new Split(res.Item2, this);
                        groups.Add(sub2);
                        q.Enqueue(sub2);
                    }
                }

            }

            foreach (Split s in groups)
            {
                List<Person> p = new List<Person>();
                foreach (int n in s.members)
                {
                    p.Add(affinity.F.getPersonByHelpLabel(n));
                }
                result.addSubGroup(p);
            }

            return result;
        }

        

        //compute values for matrix B
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



        class Split: IEquatable<Split>
        {
            private ModularityCut modCut; //parent modularity cut object
            public List<int> members = new List<int>();
            public Matrix<double> B; //mod matrix of the current subgraph
            private int n;

            public Split(List<int> members, ModularityCut mc)
            {
                this.members = members;
                n = members.Count; //elements in THIS split
                computeGroupModularityMatrix(mc.B);
                modCut = mc;
            }

            //computes B(g) for the current split
            private void computeGroupModularityMatrix(Matrix<double> networkB)
            {
                B = Matrix<double>.Build.Dense(n, n);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (i != j) B[i, j] = networkB[i, j];
                        else
                        {
                            double sum = 0;
                            foreach (int p in members)
                                sum = sum + networkB[i, p];
                            B[i, j] = networkB[i, j] - sum;
                        }
            }

            public bool divide(out Tuple<List<int>, List<int>> result)
            {
                Matrix<double> e = getFirstEigenvector(B); //get first eigenvector of modularity matrix of the split
                Matrix<double> partition = getPartition(e);

                if (!isSplitable(partition)) //if the current split is no further divisible (deltaQ not negative)
                {
                    result = null;
                    return false;
                }

                List<int> group1 = new List<int>();
                List<int> group2 = new List<int>();

                for (int i = 0; i < partition.RowCount; i++)
                {
                    if (partition[i, 0] > 1) group1.Add(members[i]);
                    else group2.Add(members[i]);
                }

                result = new Tuple<List<int>, List<int>>(group1, group2);
                return true;
            }

            //checks if the proposed split is acceptable
            public bool isSplitable(Matrix<double> partitionVector)
            {
                //construct matrix S of 0/1
                int nSubGroups = 2;
                Matrix<double> S = Matrix<double>.Build.Dense(n, nSubGroups); // we have 2 sub-communities
                for (int i = 0; i < n; i++)
                    if (partitionVector[i, 0] == 1) S[i, 0] = 1;
                    else S[i, 1] = 1;

                double deltaQ = (1 / (2 * modCut.m)) * (S.Transpose() * B * S).Trace();

                if (deltaQ < 0) return true; //modularity decreases, we can split
                else return false;
            }

            //returns the first eigenvector of the matrix
            private Matrix<double> getFirstEigenvector(Matrix<double> matrix)
            {
                Evd<double> eigen = matrix.Evd();
                Matrix<double> vectors = eigen.EigenVectors;
                return vectors.SubMatrix(0, vectors.RowCount, 0, 1); //returns only 1st vector
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

            public bool Equals(Split other)
            {
                foreach (int a in members)
                {
                    if (!other.members.Contains(a))
                        return false;
                }

                return true;
            }
        }

    }
}
