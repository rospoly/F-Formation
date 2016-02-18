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
        public override void computeAffinity()
        {
            AdjacencyMatrix=Matrix<double>.Build.Dense(F.N, F.N, (i, j) => -Math.Exp(F.distances[i,j]/8));
        }
    }
    class ProxOrient : Affinity
    {
        public ProxOrient(Frame f) : base(f) { }
        public override void computeAffinity()
        {
            Vector vector1 = new Vector();
            Vector vector2 = new Vector();
            double angleij=0;
            double angleji =0; 
            double valij =0;
            double valji = 0;
            Func<int, int, double> convert = delegate (int i, int j)
            {
                 vector1.X = F.getPersonByHelpLabel(i).CoordX;
                 vector1.Y = F.getPersonByHelpLabel(i).CoordY;
                 vector2.X = F.getPersonByHelpLabel(j).CoordX;
                 vector2.Y = F.getPersonByHelpLabel(j).CoordY;
                 angleij = Vector.AngleBetween(vector1, vector2) * Math.PI / 180;
                 angleji = Vector.AngleBetween(vector2, vector1) * Math.PI / 180;//dovrebbe cambiare solo il segno
                 valij = (F.getPersonByHelpLabel(i).Angle - angleij);
                 valji = (F.getPersonByHelpLabel(j).Angle - angleji);
                 return ((valij <= -Math.PI / 2 && valij >= Math.PI / 2) || (valji <= -Math.PI / 2 && valji >= Math.PI / 2)) ? -Math.Exp(F.distances[i, j] / 8) : 0;
            };
            AdjacencyMatrix = Matrix<double>.Build.Dense(F.N, F.N, convert);
        }
    }

    class SMEFO :Proximity
    {
        Vector<double> pf;//smefo values 
        Vector<double> centers;//centers of focus
        public SMEFO(Frame f) : base(f)
        {
            Vector<double> pf = Vector<double>.Build.Dense(f.N);
            Vector<double> center = Vector<double>.Build.Dense(f.N);
        }

        public void computeSMEFO() {
            Vector<double> temp;
            foreach (Person p in F.Persons) {
                temp= AdjacencyMatrix.Column(p.HelpLabel);
                foreach (Person j in F.Persons)
                {
                    if (j!= p)
                    {

                    }

                }
            }
        }
    }
}
