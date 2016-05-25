using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Collections;

namespace fFormations
{
    public abstract class Affinity
    {
        //protected List<int> Indexes { get; set; }
        protected Matrix<double> AdjacencyMatrix { get; set; }
        public Frame F { get; private set; }
        public double scalarFactor=20;

        public Affinity(Frame f)
        {
            this.F = f;
            AdjacencyMatrix = Matrix<double>.Build.Dense(f.N, f.N);
            //computeAffinity();
        }

        //aggiunto costruttore vuoto, il frame glieli passo uno a uno da iteration manager (Mara)
        public Affinity() { }

        //Metodi di default offerti dalla nostra classe Affinity//
        public bool ConditionRegularAffinity(double valij)
        {
            return (((valij <= Math.PI / 2.0) && (valij >= -Math.PI / 2.0)) ||
                    ((valij >= 3* Math.PI / 2.0) && (valij <= 2*Math.PI)));
            //return (((valij <= Math.PI / 2.0) && (valij >= -Math.PI / 2.0)) ||
            //((valij >= 3*Math.PI / 2.0) && (valij <= -3 * Math.PI / 2.0)));
            //((Math.Abs(valij)>= (1.5*Math.PI)) && ((Math.Abs(valij % (2.0 * Math.PI)) >= (Math.PI / 2.0))));
            //return (valij + Math.PI / 2 >= 0) && (ProxOrient.AngleDifference(valij,Math.PI/2)<=0);
        }

        public double ComputationRegularAffinity(int i, int j)
        {
            return Math.Exp(-F.distances[i, j] / (2.0*scalarFactor*scalarFactor));
        }
        //////////////////////////////////////////////////////////

        //Firma di come deve essere definito il metodo di computazione per ogni cella della matrice
        public abstract double HowToCompute(int i, int j);
        /////
        //Calcolo della matrice di adiacenza, ogni classe deve definire HowToCompute
        //Vengono forniti da affinity dei metodi Regular come supporto
        public void computeAffinity()
        {
            if (F == null) return; //se non ho settato F dal costruttore devo chiamare l'overload del metodo
            AdjacencyMatrix = Matrix<double>.Build.Dense(F.N, F.N, HowToCompute);
            //Indexes = new List<int>();
            //for (int i = 0;i<AdjacencyMatrix.ColumnCount; i++)
                //Indexes.Add(F.getPersonByHelpLabel(i).ID);
        }

        public virtual void InitOperation(Frame f) { }

        //calcola la matrice affinità dato un frame (Mara)
        public void computeAffinity(Frame f)
        {
            this.F = f;
            InitOperation(f);
            AdjacencyMatrix = Matrix<double>.Build.Dense(F.N, F.N, HowToCompute);
            //Indexes = new List<int>();
            //for (int i = 0; i < AdjacencyMatrix.ColumnCount; i++)
                //Indexes.Add(F.getPersonByHelpLabel(i).ID);
        }

        public int getDimensionAM() {
            return F.N;
        }

        public Matrix<double> getCopyMatrix() {
            Matrix<double> copy = AdjacencyMatrix.Clone();
            return copy;
        }
    }
}
