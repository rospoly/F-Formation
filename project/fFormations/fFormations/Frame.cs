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

        public List<Person> Persons { get; private set; }
        public Matrix<double> distances { get; private set; }
        public int IdFrame { get; private set; }
        public int N { get; private set; }

        public Frame(int idFrame, List<Person> persons) {
            this.N = persons.Count;
            this.IdFrame = idFrame;
            this.Persons = persons;
            distances = Matrix<double>.Build.Dense(N, N);
            computeDistances();
        }

        public Frame(int idFrame)
        {
            this.N = 0;
            this.IdFrame = idFrame;
            this.Persons = new List<Person>();
        }

        public void SetPersons(List<Person> l) {
            Persons = l;
            computeDistances();
        }

        public void computeDistances() {
            foreach (Person p in Persons){
                foreach (Person j in Persons){
                    distances[p.HelpLabel, j.HelpLabel] = p.getDistance(j);
                }
            }
        }

        public void ReassignHelpLabel()
        {
            int i = 0;
            foreach (Person p in Persons)
            {
                p.HelpLabel = i;
                i++;
            }
        }

        public void RemovePerson(Person p) {
            Persons.Remove(p);
            this.N = Persons.Count;
            distances = Matrix<double>.Build.Dense(N, N);
            computeDistances();
            ReassignHelpLabel();
        }

        public Frame getCopyFrame()
        {
            return FactoryFrame.createFrame(IdFrame, new List<Person>(Persons));
        }

        //public double getDistances(Person p, Person j) {
        //return ((Persons.Contains(p) && Persons.Contains(j)) ? p.getDistance(j) : -1);
        //}

        //returns null if the id does not exists in the frame
        public Person getPersonById(int id)
        {
            return Persons.Find(x => x.ID == id);
        }

        public Person getPersonByHelpLabel(int id)
        {
            return Persons.Find(x => x.HelpLabel == id);
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
    }
}
