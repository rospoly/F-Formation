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
        //public Proximity(Frame f) : base(f) {}
        public Proximity(double scalarFactor) : base(scalarFactor) { } //aggiunto costruttore vuoto
        public override double HowToCompute(int i, int j)
        {
            return ComputationRegularAffinity(i, j);
        }

        public double ComputationRegularAffinity(int i, int j)
        {
            return Math.Exp(-F.distances[i, j] / (2.0 * scalarFactor * scalarFactor));
        }
    }
    class ProxOrient : Proximity
    {
        Vector vector1;
        Vector vector2;
        Vector axisXVector;
        double angleij;
        double angleji;
        double windowAngle;

        //costruttore vuoto
        public ProxOrient(double scalarFactor,double windowAngle) : base(scalarFactor) {
            vector1 = new Vector();
            vector2 = new Vector();
            angleij = 0;
            angleji = 0;
            this.windowAngle = windowAngle;
            axisXVector = new Vector(1, 0);
        }

        public void InitVectors(int i, int j) {
            //inizializzo i vettori
            vector1.X = F.getPersonByHelpLabel(i).CoordX;
            vector1.Y = F.getPersonByHelpLabel(i).CoordY;
            vector2.X = F.getPersonByHelpLabel(j).CoordX;
            vector2.Y = F.getPersonByHelpLabel(j).CoordY;
            //angolo fra i due vettori, dovrebbero essere uguali a meno di un segno forse.
            //angolo misurato in gradi quindi converto in radianti

            Vector vij = vector2 - vector1;
            Vector vji = vector1 - vector2;
            angleij= Vector.AngleBetween(axisXVector, vij) * Math.PI / 180.0;
            angleji= Vector.AngleBetween(axisXVector, vji) * Math.PI / 180.0;

        }
        /// <summary>
        /// </summary>
        /// <param name="anglei"> Angolo del vettore che identifica la persona i-esima o una sua posizione</param>
        /// <param name="angleij"> Angolo del vettore tra la persona i-esima e j-esima rispetto all'asse x</param>
        /// <param name="windowAngle"> Finestra di tolleranza fra il vettore ideale che congiunge le due persone, e dove realmente siano orientate </param>
        /// <returns> Ritorna true se le due entità soddisfano le condizioni, false altrimenti</returns>
        public bool ConditionRegularAffinity(double anglei, double angleij, double windowAngle)
        {
            if ((anglei >= 0 && angleij >= 0) || (anglei <= 0 && angleij <= 0))
            {
                double valij = Utils.AngleDifference(anglei, angleij);
                return (valij <= windowAngle) && (valij >= -windowAngle);
            }

            if (angleij >= 0 && anglei <= 0)
            {
                return (Utils.changeSign(angleij + windowAngle) > anglei) || (angleij - windowAngle < anglei);
            }

            if (angleij <= 0 && anglei >= 0)
            {
                return (angleij + windowAngle > anglei) || (Utils.changeSign(angleij - windowAngle) < anglei);
            }

            throw new ArgumentException("Il metodo ha raggiunto una condizione irraggiungibile!!");
        }

        public override double HowToCompute(int i, int j) {
            if (i == j)
                return 1;
            InitVectors(i, j);

            if (ConditionRegularAffinity(GetMeasure(i), angleij,windowAngle) && ConditionRegularAffinity(GetMeasure(j), angleji,windowAngle))
            {
                return ComputationRegularAffinity(i, j);
            }
            return 0;
        }

        public virtual double GetMeasure(int i)
        {
            return F.getPersonByHelpLabel(i).Angle;
        }
       
    }

    class SMEFO :ProxOrient
    {
        public Matrix<double> tempProxMatrix;
        public SMEFO(double scalarFactor,double windowAngle) : base(scalarFactor,windowAngle){}

        public override void InitOperation(Frame f)
        {
            Affinity tempProximity = new Proximity(scalarFactor);
            tempProximity.computeAffinity(f);
            tempProxMatrix = tempProximity.getCopyMatrix();
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
            Vector person = new Vector(i.CoordX, i.CoordY);
            Vector focusCenter = FocusCenters(i);
            Vector vectorPF = focusCenter - person;
            double angle = Vector.AngleBetween(new Vector(1, 0), vectorPF) * Math.PI / 180.0;
            return angle;
            //return Math.Acos(angle);
            //return Math.Acos(Vector.AngleBetween(new Vector(i.CoordX,i.CoordY),FocusCenters(i)) * Math.PI / 180.0);
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
                sumx = sumx + j.CoordX * tempProxMatrix[i.HelpLabel, j.HelpLabel];
                sumy = sumy + j.CoordY * tempProxMatrix[i.HelpLabel, j.HelpLabel];
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
            return tempProxMatrix.Column(i.HelpLabel).Sum();
        }
    }
}
