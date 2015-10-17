﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess.HydroObjects
{
    public class ConnectNode : HydroVertex
    {
        public override FrameworkElement VertexVisualElement
        {
            get
            {
                return (Rectangle)rdict["ConnectNodeIcon"];
            }
        }
    }
}
