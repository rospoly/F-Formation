using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Factorization;
using System.Diagnostics;

namespace fFormations
{
    public class ModularityCut : Method
    {
        private Affinity affinity; //affinity matrix
        
        private int N; //network elements
        private double m; //normalization term
        private bool KLflag = false;
        private Matrix<double> B; //modularity matrix for entire network
        private Matrix<double> A; //affinity matrix

        private Split firstSplit;

        public ModularityCut(bool KL=false)
        {
            KLflag = KL; //if true, we apply kernighan-lin refinement
        }

        public void Initialize(Affinity a)
        {
            affinity = a;
            A = affinity.getCopyMatrix(); //affinity matrix
            N = affinity.getDimensionAM();

           // List<int> elementsID = new List<int>(); //contains all helplabels
            int[] labels = new int[N];
            for (int i = 0; i < N; i++)
                labels[i] = a.F.Persons[i].HelpLabel;
               // elementsID.Add(a.F.Persons[i].HelpLabel);

            computeNormTerm();
            computeModularityMatrix();

            firstSplit = new Split(labels, this); //the first split contains all elements
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
                Tuple<int[], int[]> res;
                Split current = q.Dequeue();
      //          Debug.WriteLine("Try to divide " + current);
                if(current.divide(out res))
                {
      //            Debug.Write("Divided: ");
                    groups.Remove(current); //remove current split --> usa equals
                    if (res.Item1 != null && res.Item1.Length > 1)
                    {
                        Split sub1 = new Split(res.Item1, this);
    //                  Debug.Write(sub1 + ", ");
                        groups.Add(sub1);
                        q.Enqueue(sub1);
                    }
                    if (res.Item2 != null && res.Item2.Length > 1)
                    {
                        Split sub2 = new Split(res.Item2, this);
                        groups.Add(sub2);
      //                Debug.WriteLine(sub2);
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
                    if (m == 0) B[i, j] = 0;
                    else B[i, j] = A[i, j] - ((expectedDegree(i) * expectedDegree(j)) / (2 * m));
                }
        }

        //gives expected degree of element i wrt affinity matrix, k_i = sum(j) a_ij
        private double expectedDegree(int i)
        {
            double exp = 0;
            for (int j = 0; j < N; j++)
                exp += A[i, j];
            return exp;
        }

        //compute normalization term m = 0.5 * somma(i,j) a_ij
        private double computeNormTerm()
        {
            m = 0;
            for (int i = 0; i < N; i++)
                for (int j = 0; j < N; j++)
                    m += A[i, j];
            return m * 0.5;
        }

        

        class Split: IEquatable<Split>
        {
            private ModularityCut modCut; //parent modularity cut object
            public int[] members;
            public Matrix<double> Bg; //mod matrix of the current subgraph
            private int n;

            public Split(int[] members, ModularityCut mc)
            {
                this.members = members;
                n = members.Length; //elements in THIS split
                modCut = mc;
                computeGroupModularityMatrix();
            }

            //computes B(g) for the current split
            private void computeGroupModularityMatrix()
            {
                Bg = Matrix<double>.Build.Dense(n, n);
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (i != j) Bg[i, j] = modCut.B[members[i], members[j]];
                        else
                        {
                            double sum = 0;
                            foreach (int p in members)
                                sum = sum + modCut.B[members[i], p];
                            Bg[i, j] = modCut.B[members[i], members[j]] - sum;
                        }
            }

            public bool divide(out Tuple<int[], int[]> result)
            {
                Matrix<double> e = getFirstEigenvector(Bg); //get first eigenvector of modularity matrix of the split
                Matrix<double> partition = getPartition(e);

                if (modCut.KLflag)
                    partition = KLRefinement(partition); //apply KL refinement

                if (!isSplitable(partition)) //if the current split is no further divisible (deltaQ not negative)
                {
                    result = null;
                    return false;
                }

                List<int> group1 = new List<int>();
                List<int> group2 = new List<int>();

                for (int i = 0; i < partition.RowCount; i++)
                {
                    if (partition[i, 0] == 1) group1.Add(members[i]);
                    else group2.Add(members[i]);
                }

                result = new Tuple<int[], int[]>(group1.ToArray(), group2.ToArray());
                return true;
            }

            //checks if the proposed split is acceptable
            public bool isSplitable(Matrix<double> partitionVector)
            {
                bool splitable = false;
                for (int i = 1; i < partitionVector.RowCount; i++)
                    if (partitionVector[0, 0] != partitionVector[i, 0])
                        splitable = true; //almeno uno è diverso dal primo, se sono tutti uguali resta false

                if (!splitable) return false;

                //construct matrix S of 0/1
                int nSubGroups = 2;
                Matrix<double> S = Matrix<double>.Build.Dense(n, nSubGroups); // we have c=2 sub-communities
                for (int i = 0; i < n; i++)
                    if (partitionVector[i, 0] == 1) S[i, 0] = 1;
                    else S[i, 1] = 1;

                double deltaQ =  (S.Transpose() * Bg * S).Trace() / (2 * modCut.m);
                //Debug.WriteLine("DeltaQ = " + deltaQ);
                if (deltaQ < 0) return true; //modularity decreases, we can split
                else return false;

                //double Q = (1 / (4 * modCut.m)) * (partitionVector.Transpose() * Bg * partitionVector)[0,0];
                //if (Q < -0.3) return true;
                //else return false;
            }

            private Matrix<double> KLRefinement(Matrix<double> partitionVector)
            {
                double initialQ = getModularity(partitionVector);
                int elem = partitionVector.RowCount;
                Matrix<double> newPartition = partitionVector.Clone();
                
                newPartition[0, 0] *= -1; //move first element
                double maxQ = getModularity(newPartition);
                newPartition[0, 0] *= -1; 
                int maxi = 0;
                for (int i = 1; i<elem; i++)
                {
                    newPartition[i, 0] *= -1; //move ith element
                    double thisQ = getModularity(newPartition);
                    if(Math.Abs(maxQ) < Math.Abs(thisQ)) // ?????????????!!!!!
                    {
                        maxQ = thisQ;
                        maxi = i;
                    }
                    newPartition[i, 0] *= -1;
                }
                newPartition[maxi, 0] *= -1;

                if (getModularity(newPartition) > getModularity(partitionVector)) return newPartition;
                else return partitionVector;
            }

            //returns modularity Q when the cut is partition
            private double getModularity(Matrix<double> partition)
            {
                double p = (partition.Transpose() * Bg * partition)[0, 0];
                return p / (4 * modCut.m);
            }

            //returns the first eigenvector of the matrix
            private Matrix<double> getFirstEigenvector(Matrix<double> matrix)
            {
                Evd<double> eigen = matrix.Evd();
                Matrix<double> vectors = eigen.EigenVectors;
             //   return vectors.SubMatrix(0, vectors.RowCount, vectors.ColumnCount-1, 1); //returns only 1st vector
                return vectors.SubMatrix(0, vectors.RowCount, 0, 1);
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

            public override string ToString()
            {
                string s = "Split: ";
                foreach (int i in members)
                    s = s + i + " ";
                return s;
            }
        }

    }
}
