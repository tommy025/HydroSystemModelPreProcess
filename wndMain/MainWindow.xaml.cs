using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HydroSystemModelPreProcess.HydroObjects;

namespace HydroSystemModelPreProcess
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var hydroObjectGraph = new HydroObjectGraph();
            var node = hydroObjectGraph.AddConnectNode();

            Canvas.SetTop(node, 100);
            Canvas.SetLeft(node, 100);
            drawingCanvas.Children.Add(node);
        }
    }
}
