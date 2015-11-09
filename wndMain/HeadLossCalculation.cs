using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess
{
    class HeadLossCalculation : HydroProcedure
    {
        public HeadLossCalculation(HeadLossCalculation other) : base(other)
        { }

        public HeadLossCalculation(IReadOnlyHydroObjectGraph _hydroObjectGraph) :base(_hydroObjectGraph)
        { }

        public override HydroProcedure DeepClone()
        {
            return new HeadLossCalculation(this);
        }
    }
}
