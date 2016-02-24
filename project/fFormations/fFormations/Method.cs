using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace fFormations
{
    public interface Method
    {
        void Initialize(Affinity a);
        Group ComputeGroup();
    }
}
