using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public abstract class DominantSet:Method
    {
        public Affinity a { get; private set; }
        public Matrix<double> matrix { get; private set; }//matrice di affinità
        public Vector<double> vector { get; private set; }//vettori per la ricerca dei raggruppamenti
        //public double DeltaValue;
        //Vector rispetta la regola sul label
        public DominantSet() {

        }
        
        public void Initialize(Affinity a)
        {
            this.a = a;
            matrix = a.getCopyMatrix();
            ResetVector();
        }
    
        public Group ComputeGroup()
        {
            Group g = new Group(a.F);
            return g;
        }

        /// <summary>
        /// Reset the manager vector used for dominant set research. This vector is based on affinity matrix currently used.
        /// Every changes made on this matrix are mirrored on manager vector.
        /// </summary>
        public void ResetVector() {
            vector = Vector<double>.Build.Dense(matrix.ColumnCount, 1.0 / (double)matrix.ColumnCount);
        }

        /// <summary>
        /// Compute standard quadratic formula
        /// </summary>
        /// <returns>
        /// Return the computation.
        /// </returns>
        public double ComputeFunction() {
            return (vector.ToColumnMatrix().Multiply(matrix).Multiply(vector)).Single();
        }

        /// <summary>
        /// Check if the current configuration of manager vector is inside de simplex and then valid solution.
        /// </summary>
        /// <returns></returns>
        public bool CheckValidFunction() {
            //controllare
            return ((vector.Sum() == 1) && (vector.ForAll(x => x >= 0))); 
        }

        public double RecursiveResearchMax()
        {
            Vector<double> temp= vector.ToRowMatrix().Multiply(matrix.Multiply(vector) / ComputeFunction());
            return temp.Subtract(vector).AbsoluteMaximum();
            /*if ((temp.Subtract(vector)).ForAll(x=> Math.Abs(x) <= DeltaEqual) && CheckValidFunction())
            {
                //ho trovato un punto stazionario
                List<Person> lp = new List<Person>();
                int i = 0;
                for (i = 0; i < temp.Count; i++) {
                    if (!(Math.Abs(vector[i]) <= DeltaZero))
                    {
                        //siamo in presenza di un i buono
                        lp.Add(a.F.getPersonByHelpLabel(i));
                        //matrix.RemoveColumn(i);
                        //matrix.RemoveRow(i);
                    }
                }
                //ResetVector();
            }*/
        }

        //public abstract List<Person> rebuiltVectorCriterion();
        public abstract bool StoppingCriterium();

        /// <summary>
        /// What to do when a stationary point is found!
        /// </summary>
        /// <returns></returns>
        public abstract List<Person> StationaryPointCriterion();
    }

    class LocalDominantSet : DominantSet
    {
        /// <summary>
        /// DeltaZero is the range at witch we consider the value zero.
        /// </summary>
        double DeltaZero;
        /// <summary>
        /// DeltaValue is the range at witch we consider equal two entity.
        /// </summary>
        double DeltaValue;
        LocalDominantSet(double DeltaValue,double DeltaZero)
        {
            this.DeltaValue = DeltaValue;
            this.DeltaZero = DeltaZero;
        }
        public override List<Person> StationaryPointCriterion()
        {
            List<Person> lp = null;
            if (RecursiveResearchMax()<=DeltaValue)
            {
                //ho trovato un punto stazionario
                lp = new List<Person>();
                int i = 0;
                for (i = 0; i < vector.Count; i++)
                {
                    if (!(Math.Abs(vector[i]) <= DeltaZero))
                    {
                        //siamo in presenza di un i buono
                        lp.Add(a.F.getPersonByHelpLabel(i));
                        //matrix.RemoveColumn(i);
                        //matrix.RemoveRow(i);
                        int var = 0;
                        for (var = 0; var < matrix.ColumnCount; var++)
                        {
                            matrix[i, var] = Int16.MinValue;
                            matrix[var,i] = Int16.MinValue;
                        }
                    }
                }
                ResetVector();
            }
            return lp;
        }
        /// <summary>
        /// True: go on with the research of DominantSet. Else consider the rest all singleton;
        /// </summary>
        /// <returns></returns>
        public override bool StoppingCriterium()
        {
            return (ComputeFunction() < DeltaZero);
        }
    }
    class GlobalDominantSet : DominantSet
    {
        public double DeltaValue;
        public GlobalDominantSet(double DeltaValue) {
            this.DeltaValue = DeltaValue;
        }
        
        public override List<Person> StationaryPointCriterion()
        {
            //List<Person> lp = null;
            if (RecursiveResearchMax()<DeltaValue)
            {
                //ho trovato un punto stazionario
                List<int> lp = new List<int>();
                int i = 0;
                for (i = 0; i < vector.Count; i++)
                {
                    if (!(Math.Abs(vector[i]) <= DeltaZero))
                    {
                        //siamo in presenza di un i buono
                        lp.Add(i);
                    }
                }
                WeightedAffinity wa = new WeightedAffinity(matrix);

                for (i = 0; i < vector.Count; i++)
                {
                    if (!lp.Contains(i))
                    {
                        temp.Add(i);
                        Matrix<double> m=wa.convertListToMatrix(lp);
                        if (wa.Weight(m,lp.Count-1))
                        //prova a inserire dentro e calcolare il risultato
                        //se positivo allora ci sta bene!!
                    }
                }
            }
            return lp;
        }

        public override double StationaryPointCriterion()
        {
            return DeltaValue;
        }

        public class WeightedAffinity
        {
            Matrix<double> affinity;
            public WeightedAffinity(Matrix<double> affinity)
            {
                this.affinity = affinity.Clone();
            }
            public Matrix<double> convertListToMatrix(List<int> l)
            {
                Matrix<double> res = Matrix<double>.Build.Dense(l.Count, l.Count);
                for(int i = 0; i<res.ColumnCount; i++)
                {
                    for (int j = 0; j < res.ColumnCount; j++)
                    {
                        res[i, j] = affinity[l[i], l[j]];
                    }
                }
                return res;
            }
            //Lista di indice e index è indice nella lista
            public double WeightedDegree(Matrix<double> res,int index)
            {
                return (1/res.ColumnCount)*(res.Row(index).Sum());
            }
            /// <summary>
            /// La differenza fra l'affinità di i e S, e l'affinità tra i e J.
            /// </summary>
            /// <param name="v">
            /// Il vettore cui appartiene i
            /// </param>
            /// <param name="j">
            /// Il peso del collegamento tra i e j
            /// </param>
            /// <returns></returns>
            public double RelativeAffinity(Matrix<double> res, int index, double aij)
            {
                return aij - WeightedDegree(res,index);
            }


            public double Weight(Matrix<double> v,int index)
            {
                if (v.ColumnCount == 1)
                    return 1;
                else
                {
                    Matrix<double> temp = v.RemoveColumn(index).RemoveRow(index);
                    double sum = 0;
                    for (int c = 0; c < temp.ColumnCount; c++) {
                        sum = sum + RelativeAffinity(temp, index,v[c,index]) * Weight(temp, c);
                    }
                    return sum;
                }
            }
        }
    }
}