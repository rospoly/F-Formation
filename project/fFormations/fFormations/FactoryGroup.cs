using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public static class FactoryGroup
    {
        public static Group createGroup(Frame IdFrame)
        {
            return new Group(IdFrame);
        }

        public static Group createGroup(Frame IdFrame, int GN, Dictionary<int, List<Person>> grouping)
        {
            return new Group(IdFrame, GN, grouping);
        }
    }
}
