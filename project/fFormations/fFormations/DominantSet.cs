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
        /// DeltaZero is the limit at witch we consider the value zero.
        /// </summary>
        public double DeltaZero;
        /// <summary>
        /// DeltaValue is the limit at witch the function f reach a local maximun
        /// </summary>
        public double DeltaValue;
        //public Frame CopyFrame { get; private set; }
        //public double DeltaValue;
        //Vector rispetta la regola sul label
        public DominantSet(double DeltaZero, double DeltaValue)
        {
            this.DeltaZero = DeltaZero;
            this.DeltaValue = DeltaValue;
        }

        public virtual void Initialize(Affinity a)
        {
            this.a = a;
            matrix = a.getCopyMatrix();
            indexes = a.F.getCopyIndexes();
            ResetVector();
        }
        public List<int> FromLabelToId(List<int> hl)
        {
            List<int> pers = new List<int>();
            foreach (int j in hl)
            {
                pers.Add(indexes[j]);
            }
            return pers;
        }

        public Group ComputeGroup()
        {
            ResetVector();
            Group g = new Group(a.F);
            Utils.printMatrix(matrix);
            while (indexes.Count > 1)
            {
                while (RecursiveResearchMax() > DeltaValue) {};
                if (CheckValidFunction())
                {
                    List<int> label = FindDominantGroup();
                    List<int> ids = FromLabelToId(label);

                    Utils.printMatrix(vector);
                    Utils.printMatrix(matrix);
                    
                    if (StoppingCriterium(ids))
                    {
                        indexes.Clear();
                    }
                    else
                    {
                        g.addSubGroup(ids);
                        RemoveDominantGroup(label, ids);
                    }
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
            vector = Matrix<double>.Build.Dense(matrix.ColumnCount, 1, 1.0 / matrix.ColumnCount);
        }

        /// <summary>
        /// Compute standard quadratic formula, Lagrangiana del grafo.
        /// </summary>
        /// <returns>
        /// Return the computation.
        /// </returns>
        public double ComputeFunction()
        {
            return (((vector.Transpose()).Multiply(matrix)).Multiply(vector))[0, 0];
        }

        /// <summary>
        /// Check if the current configuration of manager vector is inside de simplex and then valid solution.
        /// </summary>
        /// <returns></returns>
        public virtual bool CheckValidFunction()
        {
            //controllare
            return ((vector.ColumnSums()[0] >= 1 - DeltaValue) && (vector.ForAll(x => x >= 0)));
        }

        /// <summary>
        /// Compute di replicator dynamics. Used to achieve a local local maximun. 
        /// It compute the replicator function on each element of the vector in order to achieve the maximun.
        /// </summary>
        public double RecursiveResearchMax()
        {
            double val = ComputeFunction();
            Matrix<double> temp = matrix.Multiply(vector);
            if (val > 0) { 
                vector.MapIndexedInplace((index, colZero, value) => value * (temp[index, colZero] / val));
                return ComputeFunction() - val;
            }
            return 0;
        }

        /// <summary>
        /// When to stop the reaserch of domint set and consider all nodes like singleton.
        /// It receive the last dominant group computed.
        /// </summary>
        /// <returns></returns>
        public abstract bool StoppingCriterium(List<int> l=null);

        /// <summary>
        /// What to do when a stationary point is found!
        /// </summary>
        /// <returns></returns>

        public virtual List<int> FindDominantGroup()
        {
            List<int> lp = new List<int>();
            int i = 0;
            for (i = 0; i < vector.RowCount; i++)
            {
                if (Math.Abs(vector[i, 0]) > DeltaZero)
                {
                    lp.Add(i);
                }
            }
            return lp;
        }

        public virtual void RemoveDominantGroup(List<int> lp, List<int> values)
        {
            indexes.RemoveAll(x => values.Contains(x));
            List<int> temp = Enumerable.Range(0, matrix.ColumnCount).Except(lp).ToList();
            if (temp.Count != 0)
            {
                matrix = WeightedAffinity.convertListToMatrix(temp, matrix);
                ResetVector();
            }
        }
    }

    class LocalDominantSet : DominantSet
    {

        /// <summary>
        /// DeltaValue is the range at witch we consider equal two entity.
        /// </summary>
        //double DeltaValue;
        //Frame CopyFrame;
        public LocalDominantSet(double DeltaZero, double DeltaValue) : base(DeltaZero, DeltaValue){}
        /// <summary>
        /// false: go on with the research of DominantSet. Else consider the rest all singleton;
        /// </summary>
        /// <returns></returns>
        public override bool StoppingCriterium(List<int> l=null)
        {
            return Math.Abs(ComputeFunction()) < DeltaZero;
        }
    }

    class GlobalDominantSet : DominantSet
    {
        Matrix<double> reservedMatrix;
        List<int> reservedIndexes;
        public GlobalDominantSet(double DeltaZero, double DeltaValue) : base(DeltaZero, DeltaValue) { }

        public override void Initialize(Affinity a)
        {
            base.Initialize(a);
            reservedMatrix = matrix.Clone();
            reservedIndexes = new List<int>(indexes);
        }

        /// <summary>
        /// false: go on with the research of DominantSet. Else consider the rest all singleton;
        /// </summary>
        /// <returns></returns>
        public override bool StoppingCriterium(List<int> l)
        {
            if (l.Count == reservedMatrix.ColumnCount)
                 return false;

            List<int> lp = new List<int>();
            foreach (int id in l)
            {
                lp.Add(reservedIndexes.IndexOf(id));
            }
            
            for (int i = 0; i < reservedMatrix.ColumnCount; i++)
            {
                if (!lp.Contains(i))
                {
                    lp.Add(i);
                    Matrix<double> m = WeightedAffinity.convertListToMatrix(lp, reservedMatrix);
                    
                    if (WeightedAffinity.Weight(m, lp.Count - 1) > 0)
                    {
                        return true;
                    }
                    lp.Remove(i);
                }
            }
            return false;  
        }
    }
    public class WeightedAffinity
    {
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
            return ((1.0 / res.ColumnCount) * (res.Row(index).Sum()));
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
            return (aij - WeightedDegree(res, index));
        }


        public static double Weight(Matrix<double> v, int index)
        {
            if (v.ColumnCount == 1)
                return 1;

            Matrix<double> temp = v.RemoveColumn(index).RemoveRow(index);
            double sum = 0;
            for (int c = 0; c < temp.ColumnCount; c++)
            {
                int tmp = c;
                if (c >= index)
                    tmp = c + 1;
                sum = sum + (RelativeAffinity(temp, c, v[tmp, index]) * Weight(temp, c));
            }
            return sum;
        }

        public static double Weight2(Matrix<double> v, int index)
        {
            if (v.ColumnCount == 1)
                return 1;

            double sum2 = 0;
            Matrix<double> temp2 = v.RemoveColumn(index).RemoveRow(index);
            for (int c = 0; c < temp2.ColumnCount; c++)
            {
                int tmp = c;
                if (c >= index)
                    tmp = c + 1;
                int h = 0;
                while (h == index)
                    h = h + 1;
                sum2 = sum2 + (v[tmp, index] - v[tmp, h]) * Weight(temp2, c);
            }
            return sum2;
        }
    }
}