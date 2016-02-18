using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class IterationManager
    {
        private DataManager DM;
        private Method m;
        private Affinity a;
        private List<Group> computed;
        public IterationManager(DataManager DM, Method m, Affinity a) {
            this.DM = DM;
            this.m = m;
            this.a = a;
        }

        public void computeMethod()
        {
            //ottieni data da DM
            a.computeAffinity();
            m.Initialize(a);
            computed= m.ComputeGroup();
        }

        public Tuple<double,double,double> comparison() {
            foreach (Group g in computed) {
                List<int> temp=Group.Compare(DM.getGTById(g.IdFrame.IdFrame), g);
                //elaborate temp
            }
        }
    }
}
