using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    class AllSingleton : Method
    {
        public Affinity a { get; private set; }
        public List<int> indexes;
        public Matrix<double> matrix { get; private set; }
        public Group ComputeGroup()
        {
            Group g = new Group(a.F);
            g.addAllSingletons(FromLabelToId(indexes));
            return g;
        }
        public List<int> FromLabelToId(List<int> hl)
        {
            List<int> pers = new List<int>();
            foreach (int j in hl)
            {
                pers.Add(j);
            }
            return pers;
        }

        public void Initialize(Affinity a)
        {
            this.a = a;
            matrix = a.getCopyMatrix();
            indexes = a.F.getCopyIndexes();
        }
    }
}
