using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using HydroSystemModelPreProcess.HydroObjects;
using System.Windows.Shapes;

namespace HydroSystemModelPreProcess
{
    static class HydroResourceHelper
    {
        //This lead to UnitTest not working since pack URI requires System.Windows.Application while UnitTest didn't create one.
        private const string rdictSource = "pack://application:,,,/UI;component/generic.xaml";

        private static readonly ResourceDictionary rdict;

        static HydroResourceHelper()
        {
            rdict = new ResourceDictionary();
            rdict.Source = new Uri(rdictSource, UriKind.RelativeOrAbsolute);
        }

        public static HydroObject CreateHydroObject(Type type)
        {
            switch (type.Name)
            {
                case nameof(ConnectNode):
                    return new ConnectNode();
                case nameof(PressurePipe):
                    return new PressurePipe();
                default:
                    return null;
            }
        }

        public static FrameworkElement CreateVisualElement(Type type)
        {
            FrameworkElement element = null;
            switch(type.Name)
            {
                case nameof(ConnectNode):
                    element = rdict["ConnectNodeIcon"] as Rectangle;
                    break;
                case nameof(PressurePipe):
                    element = rdict["PressurePipeLine"] as Line;
                    break;
                default:
                    return null;
            }

            element.Name = "_" + DateTime.Now.Ticks.ToString();
            return element;
        }

        public static FrameworkElement GetProcedurePropertySettingControl(Type type)
        {
            switch (type.Name)
            {
                case nameof(HeadLossCalculation):
                    return rdict["HeadLossProcedurePropertyControl"] as FrameworkElement;
                default:
                    return null;
            }
        }

        public static FrameworkElement GetHydroObjectPropertySettingControl(Type type)
        {
            switch (type.Name)
            {
                case nameof(ConnectNode):
                    return null;
                case nameof(PressurePipe):
                    return rdict["PressurePipePropertyControl"] as FrameworkElement;
                default:
                    return null;
            }            
        }
    }
}
