using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public class Person
    {
        //private int ID;
        private double coordX;
        private double coordY;
        private double angle;

        public int ID { get; private set; }

        public double CoordX { get; private set}

        public Person(int id, double coordX, double coordY, double angle) {
            this.ID = id;
            this.coordX = coordX;
            this.coordY = coordY;
            this.angle = angle;
        }

        public static bool operator ==(Person a, Person b)
        {
            return a.ID == b.ID;
        }
        public static bool operator !=(Person a, Person b)
        {
            return a.ID != b.ID;
        }

        public override bool Equals(object obj) {
            Person temp = obj as Person;
            return temp == null ? false : temp == this;
        }
    }
}
