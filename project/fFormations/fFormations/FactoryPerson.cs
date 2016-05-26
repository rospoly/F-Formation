using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public static class FactoryPerson
    {
        public static Person createPerson(int id, double X, double Y, double angle, int label)
        {
            double theta = normalizeAngle(angle); //get an angle from -pi to pi

            return new Person(id, X, Y, theta, label);
        }

        private static double normalizeAngle(double theta)
        {
            while (theta > Math.PI)
                theta = theta - 2 * Math.PI;
            while (theta < -1 * Math.PI)
                theta = theta + 2 * Math.PI;
            return theta;
        }

    }
}
