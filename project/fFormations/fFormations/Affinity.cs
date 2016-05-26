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
        protected Matrix<double> AdjacencyMatrix { get; set; }
        public Frame F { get; private set; }
        public double scalarFactor { get; private set; }
    

        public Affinity(Frame f)
        {
            this.F = f;
            AdjacencyMatrix = Matrix<double>.Build.Dense(f.N, f.N);
            //computeAffinity();
        }

        //aggiunto costruttore vuoto, il frame glieli passo uno a uno da iteration manager (Mara)

        /// <summary>
        /// </summary>
        /// <param name="scalarFactor"> Il parametro definisce la normalizzazione delle distanze. Suggerito 200</param>
        public Affinity(double scalarFactor)
        {
            this.scalarFactor = scalarFactor;
        }
        
        //Firma di come deve essere definito il metodo di computazione per ogni cella della matrice
        /// <summary>
        /// Ogni classe che estende Affinity deve definire come computare la matrice di adiacenza
        /// </summary>
        /// <param name="i"></param>
        /// <param name="j"></param>
        /// <returns></returns>
        public abstract double HowToCompute(int i, int j);

        //Calcolo della matrice di adiacenza, ogni classe deve definire HowToCompute
        public void computeAffinity()
        {
            if (F==null)
                throw new ArgumentNullException("Attenzione non hai impostato il frame!");
            //if (F == null) return; //se non ho settato F dal costruttore devo chiamare l'overload del metodo
            AdjacencyMatrix = Matrix<double>.Build.Dense(F.N, F.N, HowToCompute);
        }
    
        public virtual void InitOperation(Frame f) {}

        //calcola la matrice affinità dato un frame (Mara)
        public void computeAffinity(Frame f)
        {
            this.F = f;
            InitOperation(f);
            AdjacencyMatrix = Matrix<double>.Build.Dense(F.N, F.N, HowToCompute);
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
