using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.LinearAlgebra;
using System.Windows;

namespace fFormations
{
    class Proximity : Affinity
    {
        public Proximity(Frame f) : base(f) {}
        public Proximity() : base() { } //aggiunto costruttore vuoto

        public override double HowToCompute(int i, int j)
        {
            return ComputationRegularAffinity(i, j);
        }
    }
    class ProxOrient : Affinity
    {
        Vector vector1 = new Vector();
        Vector vector2 = new Vector();
        double angleij;
        double angleji;
        double valij;
        double valji;

        public ProxOrient(Frame f) : base(f) {
             vector1 = new Vector();
             vector2 = new Vector();
             angleij = 0;
             angleji = 0;
             valij = 0;
             valji = 0;
        }

        //costruttore vuoto
        public ProxOrient() : base() { }

        public void InitVectors(int i, int j) {
            //inizializzo i vettori
            vector1.X = F.getPersonByHelpLabel(i).CoordX;
            vector1.Y = F.getPersonByHelpLabel(i).CoordY;
            vector2.X = F.getPersonByHelpLabel(j).CoordX;
            vector2.Y = F.getPersonByHelpLabel(j).CoordY;
            //angolo fra i due vettori, dovrebbero essere uguali a meno di un segno forse.
            //angolo misurato in gradi quindi converto in radianti
            angleij = Vector.AngleBetween(vector1, vector2) * Math.PI / 180.0;
            angleji = Vector.AngleBetween(vector2, vector1) * Math.PI / 180.0;//dovrebbe cambiare solo il segno
        }

        public override double HowToCompute(int i, int j) {
            InitVectors(i, j);
            //In questo caso mi serve l'angolo della prima persona e quello della seconda
            valij = GetMeasure(i) - angleij;
            valji = GetMeasure(j) - angleji;
            //Secondo me la ComputationRegularAffinity(i,j)=ComputationRegularAffinity(j,i)
            //Se entrambe le condizioni sono verificate, devo calcolare il valore
            //Se una delle due condizioni è falsa prendo 0, siccome la computazione
            //coinvolge un'esponenziale allora 0 è sicuramente minore.
            if (ConditionRegularAffinity(valij) && ConditionRegularAffinity(valji))
            {
                if (ComputationRegularAffinity(i, j) != ComputationRegularAffinity(j, i))
                    Console.WriteLine();
                return ComputationRegularAffinity(i, j);
            }
            return 0;
        }

        public virtual double GetMeasure(int i)
        {
            //Nel caso di Prox e Orien confronto l'angolo della persona con label i
            return F.getPersonByHelpLabel(i).Angle;
        }
    }

    class SMEFO :ProxOrient
    {
        public Vector<double> pf;//smefo values 
        public Vector<double> centers;//centers of focus

        public SMEFO(Frame f) : base(f)
        {
             pf = Vector<double>.Build.Dense(f.N); 
             centers = Vector<double>.Build.Dense(f.N);
        }

        public override double GetMeasure(int i)
        {
            //Qui uso lo SMEFO della persona con label i
            return computeSMEFO(F.getPersonByHelpLabel(i));
        }
        /// <summary>
        /// Arcocoseno dell' angolo tra il vettore della posizione della persona i e il suo centro di focus
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double computeSMEFO(Person i) {
            return Math.Acos(Vector.AngleBetween(new Vector(i.CoordX,i.CoordY),FocusCenters(i)));
        }

        /// <summary>
        /// Centro di focus della persona i
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public Vector FocusCenters(Person i) {
            double sumx = 0;
            double sumy = 0;
            foreach (Person j in F.Persons) {
                sumx = sumx + j.CoordX * AdjacencyMatrix[i.HelpLabel, j.HelpLabel];
                sumy = sumy + j.CoordY * AdjacencyMatrix[i.HelpLabel, j.HelpLabel];
            }
            double degree = totalDegreeOf(i);
            return new Vector(sumx / degree, sumy / degree);
        }

        /// <summary>
        /// Calcola il grado della persona i nei confronti di tutti gli altri
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public double totalDegreeOf(Person i)
        {
            return AdjacencyMatrix.Column(i.HelpLabel).Sum();
        }
    }
}
