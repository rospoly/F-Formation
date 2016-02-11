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
        private Dictionary<int, List<Person>> grouping;

        public Group(Frame IdFrame,int GN,Dictionary<int,List<Person>> grouping) {
            this.IdFrame = IdFrame;
            this.GN = GN;
            this.grouping = grouping;
        }

        public Group(Frame IdFrame)
        {
            this.IdFrame = IdFrame;
            this.GN = 0;
            this.grouping = new Dictionary<int, List<Person>>();
        }
        public void addSubGroup(List<Person> b) {
            int temp = grouping.Keys.Count;
            grouping.Add(temp, b);

        }
        public void removeSubGroup(int n){
            grouping.Remove(n);
        }

        public void removeSubGroup(List<Person> b)
        {
            foreach (int v in grouping.Keys) {
            }
        }
    }
}
