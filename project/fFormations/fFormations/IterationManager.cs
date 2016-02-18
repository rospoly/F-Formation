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
            a.computeAffinity();
            m.Initialize(a);
            computed= m.ComputeGroup();
        }

        public CollectorResult comparison() {
            Console.WriteLine("GroupList computed has size: "+computed.Count);
            CollectorResult rs = new CollectorResult();
            foreach (Group g in computed) {
                List<int> temp=Group.Compare(DM.getGTById(g.IdFrame.IdFrame), g);
                rs.addResult(new Result(temp, g.IdFrame.IdFrame));
            }
            //lista di tutti i risultati
            rs.ToString();
            //funzione di riduzione su tutti i risultati (ex.average)
            //aggiunta alla fine di rs
            //ritorno rs
            rs.computation();
            return rs;
        }
    }
}
