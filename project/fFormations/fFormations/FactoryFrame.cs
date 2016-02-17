using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public static class FactoryFrame
    {
        public static Frame createEmptyFrame(int id)
        {
            return new Frame(id);
        }

        public static Frame createFrame(int id, List<Person> people)
        {
            return new Frame(id, people);
        }
    }
}
