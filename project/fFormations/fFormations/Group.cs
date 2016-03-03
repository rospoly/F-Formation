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
        public Frame IdFrame { get; private set;}//Frame Id associated to a group
        public int GN { get; private set; }//number of subgroups
        public Dictionary<int, List<Person>> Grouping { get; private set; }//

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

        public void addSubGroup(List<int> b)
        {
            List<Person> ls = new List<Person>();
            foreach (int p in b)
                ls.Add(IdFrame.getPersonById(p));
            int temp = Grouping.Keys.Count;
            Grouping.Add(temp, ls);
            GN++;
        }

        public void addSingleton(int b)
        {
            int temp = Grouping.Keys.Count;
            List<Person> lp = new List<Person>();
            lp.Add(IdFrame.getPersonById(b));
            Grouping.Add(temp, lp);
        }

        public void addAllSingletons(List<int> l)
        {
            foreach(int n in l)
            {
                addSingleton(n);
            }
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

            foreach (List<Person> l2 in orig.Grouping.Values) //foreach gt group
            {
                foreach (List<Person> l1 in val.Grouping.Values) //foreach detected group
                {
                    IEnumerable<Person> temp = l1.Intersect<Person>(l2,new PersonComparator());
                    if (l1.Count == 2 && l2.Count == 2 && temp.Count<Person>()==2) //if groups have 2 members they must match perfectly
                        correct++;
                    else
                        if ((temp.Count<Person>() / Math.Max(l1.Count, l2.Count)) >= error)
                            correct++;
                }
            }

            falsePositive = val.Grouping.Values.Count - correct;
            //groups that are present in MY evaluation but not in TRUE one
            falseNegative = orig.Grouping.Values.Count - correct; 
            //groups that are present in the TRUE evaluation but not in MY 

            List<int> myList = new List<int>();
            myList.Add(falseNegative);
            myList.Add(falsePositive);
            myList.Add(correct);
            return myList;
        }

        public override string ToString()
        {
            string s = "ID: " + IdFrame.IdFrame + ", Groups: ";
            foreach (List<Person> g in Grouping.Values)
            {
                s = s + "{ ";
                foreach (Person p in g)
                {
                    s = s + p?.ID + " ";
                }
                s = s + "}; ";
            }
            return s;
        }
    }
}
