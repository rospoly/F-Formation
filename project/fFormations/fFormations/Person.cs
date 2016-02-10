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

        public double CoordX { get { return coordX; } set { if (value > 0) coordX = value; } }
        public double CoordY { get { return coordY; } set { if (value > 0) coordY = value; } }

        public double Angle { get { return angle; } set { if (Math.Abs(value) < Math.PI) angle = value; } }

        public Person(int id, double coordX, double coordY, double angle) {
            this.ID = id;
            this.CoordX = coordX;
            this.CoordY = coordY;
            this.Angle = angle;
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
 
        public override int GetHashCode()
        {
            return ID;
        }
    }
}
