using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public static class FactoryPerson
    {
        public static Person createPerson(int id, double X, double Y, double angle, int label)
        {
                return new Person(id, X, Y, angle, label);
        }
    }
}
