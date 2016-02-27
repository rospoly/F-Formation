using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public class PersonComparator : IEqualityComparer<Person>
    {
        public bool Equals(Person a, Person b)
        {
            return a==b;
        }

        public int GetHashCode(Person a)
        {
            return a.GetHashCode();
        }
    }

    public class Person
    {
        private double coordX;
        private double coordY;
        private double angle;
        public int HelpLabel { get; set; }
        public int ID { get; private set; }

        public double CoordX { get { return coordX; } set { if (value > 0) coordX = value; } }
        public double CoordY { get { return coordY; } set { if (value > 0) coordY = value; } }
        public double Angle { get { return angle; } set { if (Math.Abs(value) < Math.PI) angle = value; } }

        public Person(int id, double coordX, double coordY, double angle,int label) {
            this.ID = id;
            this.CoordX = coordX;
            this.CoordY = coordY;
            this.Angle = angle;
            HelpLabel = label;
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

        public double getDistance(Person p) {
            return Math.Sqrt((CoordX - p.CoordX)*(CoordX - p.CoordX)+(CoordY - p.CoordY)*(CoordY - p.CoordY));
        }

        public int getLabel()
        {
            return HelpLabel;
        }
    }
}
