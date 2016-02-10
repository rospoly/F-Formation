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
        public List<Person> persons { get; private set;}

        public int IdFrame { get; private set; }
        public int N { get; private set;}
        
        public Frame(int idFrame,int n,List<Person> persons) {
            this.N = n;
            this.IdFrame = idFrame;
            this.persons = persons;
        }

        public Frame(int idFrame)
        {
            this.N = 0;
            this.IdFrame = idFrame;
            this.persons = new List<Person>();
        }

        public void addPerson(Person p) {
            persons.Add(p);
            N = persons.Count;
        }

        public void removePerson(Person p)
        {
            persons.Remove(p);
            N = persons.Count;
        }
        public static bool operator ==(Frame a, Frame b)
        {
            return a.IdFrame== b.IdFrame;
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
