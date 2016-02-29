using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public abstract class DominantSet : Method
    {
        public Affinity a { get; private set; }
        public List<int> indexes;
        public Matrix<double> matrix { get; private set; }//matrice di affinità
        public Matrix<double> vector { get; private set; }//vettori per la ricerca dei raggruppamenti
        /// <summary>
        /// DeltaZero is the range at witch we consider the value zero.
        /// </summary>
        public double DeltaZero;
        //public Frame CopyFrame { get; private set; }
        //public double DeltaValue;
        //Vector rispetta la regola sul label
        public DominantSet(double DeltaZero)
        {

        }

        public void Initialize(Affinity a)
        {
            this.a = a;
            matrix = a.getCopyMatrix();
            indexes = a.getCopyIndexes();
            //CopyFrame = a.getCopyFrame();
            ResetVector();
        }

        public Group ComputeGroup()
        {
            ResetVector();
            Group g = new Group(a.F);
            while (indexes.Count > 0)
            {
                while (RecursiveResearchMax() > 0) { };
                if (CheckValidFunction())
                {
                    List<int> temp = FindDominantGroup();
                    if (StoppingCriterium(temp))
                    {
                        //g.addSubGroup(temp);
                        RemoveDominantGroup(temp);
                    }
                    else
                    {
                        temp = allSingletons();
                        RemoveDominantGroup(temp);
                        //g.addSubGroup(temp);
                    }
                    g.addSubGroup(temp);
                }
            }
            return g;
        }

        /// <summary>
        /// Reset the manager vector used for dominant set research. This vector is based on affinity matrix currently used.
        /// Every changes made on affinity matrix are mirrored on the vector.
        /// </summary>
        public void ResetVector()
        {
            //vettore colonna
            vector = Matrix<double>.Build.Dense(matrix.ColumnCount, 0, 1.0 / (double)matrix.ColumnCount);
        }

        /// <summary>
        /// Compute standard quadratic formula, Lagrangiana del grafo.
        /// </summary>
        /// <returns>
        /// Return the computation.
        /// </returns>
        public double ComputeFunction()
        {
            return (vector.Transpose().Multiply(matrix).Multiply(vector))[0, 0];
        }

        /// <summary>
        /// Check if the current configuration of manager vector is inside de simplex and then valid solution.
        /// </summary>
        /// <returns></returns>
        public bool CheckValidFunction()
        {
            //controllare
            return ((vector.RowSums()[0] == 1) && (vector.ForAll(x => x >= 0)));
        }

        /// <summary>
        /// Compute di replicator dynamics. Used to achieve a local local maximun. 
        /// It compute the replicator function on each element of the vector in order to achieve the maximun.
        /// </summary>
        public double RecursiveResearchMax()
        {
            Matrix<double> temp = matrix.Multiply(vector);
            double val = ComputeFunction();
            //vector.MapIndexed<double>((index, value) => vector[index] = value * temp[index] / val);
            vector.MapIndexedInplace((index, colZero, value) => value * temp[index, colZero] / val);
            return Math.Abs(ComputeFunction() - val);
        }

        /// <summary>
        /// When to stop the reaserch of domint set and consider all nodes like singleton.
        /// It receive the last dominant group computed.
        /// </summary>
        /// <returns></returns>
        public abstract bool StoppingCriterium(List<int> l);
        public List<int> allSingletons()
        {
            /*List<Person> singletons = new List<Person>();
            foreach (int i in indexes)
                singletons.Add(a.F.getPersonById(i));
            */
            return new List<int>(indexes);
        }

        /// <summary>
        /// What to do when a stationary point is found!
        /// </summary>
        /// <returns></returns>
        //public abstract List<int> FindDominantGroup();

        public virtual List<int> FindDominantGroup()
        {
            List<int> lp = new List<int>();
            int i = 0;
            for (i = 0; i < vector.RowCount; i++)
            {
                if (!(Math.Abs(vector[i, 0]) <= DeltaZero))
                {
                    //siamo in presenza di un i buono
                    int ps = indexes[i];
                    lp.Add(ps);
                }
            }
            return lp;
        }

        public virtual void RemoveDominantGroup(List<int> lp)
        {
            foreach (int j in lp)
            {
                matrix.RemoveColumn(j);
                matrix.RemoveRow(j);
                indexes.RemoveAt(j);
                vector.RemoveRow(j);
            }
            ResetVector();
        }
    }

    class LocalDominantSet : DominantSet
    {

        /// <summary>
        /// DeltaValue is the range at witch we consider equal two entity.
        /// </summary>
        //double DeltaValue;
        //Frame CopyFrame;
        public LocalDominantSet(/*double DeltaValue*/ double DeltaZero) : base(DeltaZero)
        {
            //this.DeltaValue = DeltaValue;
        }

        /*
        public override List<int> FindDominantGroup()
        {
            List<int> lp = new List<int>();
            int i = 0;
            for (i = 0; i < vector.RowCount; i++)
            {
                if (!(Math.Abs(vector[i,0]) <= DeltaZero))
                {
                    //siamo in presenza di un i buono
                    int ps = indexes[i];
                    lp.Add(ps);
                }
            }
            return lp;
        }
        */

        /// <summary>
        /// True: go on with the research of DominantSet. Else consider the rest all singleton;
        /// </summary>
        /// <returns></returns>
        public override bool StoppingCriterium(List<int> l = null)
        {
            return (ComputeFunction() < DeltaZero);
        }


    }

    class GlobalDominantSet : DominantSet
    {
        //public double DeltaValue;

        public GlobalDominantSet(/*double DeltaValue,*/ double DeltaZero) : base(DeltaZero)
        {
            //this.DeltaValue = DeltaValue;
        }

        /*public override List<int> FindDominantGroup()
        {
            List<int> lp = new List<int>();
            int i = 0;
            for (i = 0; i < vector.RowCount; i++)
            {
                if (!(Math.Abs(vector[i,0]) <= DeltaZero))
                {
                    //siamo in presenza di un i buono
                    //Person ps = a.F.getPersonById(indexes[i]);
                    lp.Add(i);
                    //matrix.RemoveColumn(i);
                    //matrix.RemoveRow(i);
                    //indexes.RemoveAt(i);
                }
            }
            //ResetVector();
            return lp;
        }*/

        /// <summary>
        /// True: go on with the research of DominantSet. Else consider the rest all singleton;
        /// </summary>
        /// <returns></returns>
        public override bool StoppingCriterium(List<int> l)
        {
            //WeightedAffinity wa = new WeightedAffinity(matrix);
            List<int> lp = new List<int>(l);
            for (int i = 0; i < matrix.ColumnCount; i++)
            {
                lp.Add(i);
                Matrix<double> m = WeightedAffinity.convertListToMatrix(lp, matrix);
                if (WeightedAffinity.Weight(m, lp.Count - 1) < 0)
                    return true;
                //prova a inserire dentro e calcolare il risultato
                //se positivo allora ci sta bene!!
            }
            return false;
        }

        public class WeightedAffinity
        {
            //Matrix<double> affinity;
            //public WeightedAffinity(Matrix<double> affinity)
            //{
            //this.affinity = affinity.Clone();
            //}
            public static Matrix<double> convertListToMatrix(List<int> l, Matrix<double> affinity)
            {
                Matrix<double> res = Matrix<double>.Build.Dense(l.Count, l.Count);
                for (int i = 0; i < res.ColumnCount; i++)
                {
                    for (int j = 0; j < res.ColumnCount; j++)
                    {
                        res[i, j] = affinity[l[i], l[j]];
                    }
                }
                return res;
            }
            //Lista di indice e index è indice nella lista
            public static double WeightedDegree(Matrix<double> res, int index)
            {
                return (1 / res.ColumnCount) * (res.Row(index).Sum());
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
            public static double RelativeAffinity(Matrix<double> res, int index, double aij)
            {
                return aij - WeightedDegree(res, index);
            }


            public static double Weight(Matrix<double> v, int index)
            {
                if (v.ColumnCount == 1)
                    return 1;
                else
                {
                    Matrix<double> temp = v.RemoveColumn(index).RemoveRow(index);
                    double sum = 0;
                    for (int c = 0; c < temp.ColumnCount; c++)
                    {
                        sum = sum + RelativeAffinity(temp, index, v[c, index]) * Weight(temp, c);
                    }
                    return sum;
                }
            }
        }
    }
}