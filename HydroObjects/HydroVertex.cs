﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public abstract class HydroVertex : HydroObject
    {
        public HydroVertex()
        { }

        public HydroVertex(DateTime _creationTime, string Name = "") : base(_creationTime)
        { }
    } 
}
