using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class IterationManager
    {
        public DataManager DM;
        public List<Group> computed;
        
        public IterationManager(DataManager DM) {
            this.DM = DM;
            computed = new List<Group>();
        }

        public void computeMethod(Method m, Affinity a)
        {
            a.computeAffinity();
            m.Initialize(a);
            computed.Add(m.ComputeGroup());
        }

        public void computeMethod(Method m, List<Affinity> a)
        {
            foreach (Affinity affinity in a) {
                affinity.computeAffinity();
                m.Initialize(affinity);
                computed.Add(m.ComputeGroup());
            }
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
