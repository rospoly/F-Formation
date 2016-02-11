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
            GN++;
        }
        //ATTENZIONE 
        /// <summary>
        /// Ordine importante! Il primo parametro è il gruppo stimato,
        /// il secondo parametro il gruppo originale!
        /// OutPut una lista con: prima posizione CORRETTI, seconda posizione FALSI POSITIVI, terza posizione FALSI NEGATIVI
        /// </summary>
        /// <param name="val"></param>
        /// <param name="orig"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static List<int> Compare(Group val, Group orig,double error=1) {
            if (val.IdFrame != orig.IdFrame) return new List<int>();//Non sto confrontando gli stessi frame
            int correct = 0;
            int falsePositive = 0;
            int falseNegative = 0;
            foreach (List<Person> l1 in val.Grouping.Values) {
                foreach (List<Person> l2 in orig.Grouping.Values)
                {
                    IEnumerable<Person> temp = l1.Intersect<Person>(l2,new PersonComparator());
                    if (l1.Count == 2 && l2.Count == 2 && temp.Count<Person>()==2)
                        correct++;
                    else
                        if ((temp.Count<Person>() / Math.Max(l1.Count, l2.Count)) >= error)
                            correct++;
                }
            }
            falsePositive = val.GN- correct;
            //groups that are present in MY evaluation but not in TRUE one
            falseNegative = orig.GN - correct;
            //groups that are present in the TRUE evaluation but not in MY
            List<int> myList = new List<int>();
            myList.Add(falseNegative);
            myList.Add(falsePositive);
            myList.Add(correct);
            return myList;
        }
    }
}
