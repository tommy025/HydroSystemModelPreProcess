using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroEdge : HydroObject
    {
        public HydroEdge()
        { }

        public HydroEdge(HydroEdge other) : base(other)
        { }
    }
}
