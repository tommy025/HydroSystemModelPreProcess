using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydroSystemModelPreProcess
{
    public interface ICustomizedClone<out T>
    {
        string Note
        { get; }

        T CustomizedClone();
    }
}
