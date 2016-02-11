using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public class Group
    {
        public Frame IdFrame { get; private set;}
        public int GN { get; private set; }//grouping number -> numero sottogruppi
        public Dictionary<int, List<Person>> Grouping { get; private set; }

        public Group(Frame IdFrame,int GN,Dictionary<int,List<Person>> grouping) {
            this.IdFrame = IdFrame;
            this.GN = GN;
            this.Grouping = grouping;
        }

        public Group(Frame IdFrame)
        {
            this.IdFrame = IdFrame;
            this.GN = 0;
            this.Grouping = new Dictionary<int, List<Person>>();
        }

        public void addSubGroup(List<Person> b) {
            int temp = Grouping.Keys.Count;
            Grouping.Add(temp, b);
        }

        public int Compare(Group a, double error=0) {
            if (IdFrame != a.IdFrame) return -1;//Non sto confrontando gli stessi frame
            foreach (List<Person> l1 in Grouping.Values) {
                foreach (List<Person> l2 in a.Grouping.Values)
                {
                    IEnumerable<Person> temp = l1.Intersect<Person>(l2,new PersonComparator());
                    if (l1.Count == 2 && l2.Count == 2)
                    {
                       //( if (temp.Count<Person>()==)
                    }
                    else
                    {

        }
        }
            }
            return 0;
        }
    }
}
