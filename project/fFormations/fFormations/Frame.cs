using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public class Frame
    {
        private int n;
        //private Dictionary<int, int> labels;
        public List<Person> Persons { get; private set; }
        private Matrix<double> distances;
        public int IdFrame { get; private set; }
        public int N { get; private set; }

        public Frame(int idFrame, int n, List<Person> persons) {
            this.N = n;
            this.IdFrame = idFrame;
            this.Persons = persons;
            /*int i = 0;
            foreach (Person elem in Persons) {
                labels.Add(elem.ID, i);
                i++;
            }
            distances = Matrix<double>.Build.Dense(N, N);
            */
            //computeDistances();
        }

        public Frame(int idFrame)
        {
            this.N = 0;
            this.IdFrame = idFrame;
            this.Persons = new List<Person>();
            //labels = new Dictionary<int, int>();
        }

        /*public void AddPerson(Person p) {
            Persons.Add(p);
            N = Persons.Count;
        }

        public void removePerson(Person p)
        {
            Persons.Remove(p);
            N = Persons.Count;
            computeDistances();
        }*/

       /* public void computeDistances() {
            foreach (Person p in Persons){
                foreach (Person j in Persons){

                }
            }
        }*/

        public double getDistance(Person p, Person j)
        {
            if (Persons.Contains(p) & Persons.Contains(j))
                return (p.getDistance(j));
            else
                return -1;
        }

        //returns null if the id does not exists in the frame
        public Person getPersonById(int id)
        {
            return Persons.Find(x => x.ID == id);
        }

        public static bool operator ==(Frame a, Frame b)
        {
            return a.IdFrame == b.IdFrame;
        }
        public static bool operator !=(Frame a, Frame b)
        {
            return a.IdFrame != b.IdFrame;
        }

        public override bool Equals(object obj)
        {
            Frame temp = obj as Frame;
            return temp == null ? false : temp == this;
        }

        public override int GetHashCode()
        {
            return IdFrame;
        }

       /* public int getLabel(int a) {
            return (labels.ContainsKey(a)) ? labels[a] : -1;            
        }*/
    }
}
