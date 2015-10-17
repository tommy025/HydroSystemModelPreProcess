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
        private HydroObjectGraph hydroObjectGraph;

        private MainWindowState mainWindowState;

        private FrameworkElement selectedElement;

        private Vector clickOffset;

        private bool isDragging;

        public MainWindow()
        {
            InitializeComponent();

            hydroObjectGraph = new HydroObjectGraph();
        }
                
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void drawingCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var hitPosition = e.GetPosition(drawingCanvas);
            if (rbtnPointer.IsChecked == true)
            {
                if (selectedElement != null)
                    selectedElement.Tag = "";
            
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                selectedElement = hitResult.VisualHit as FrameworkElement;
                selectedElement.Tag = "Selected";

                clickOffset = hitPosition - new Point(Canvas.GetLeft(selectedElement), Canvas.GetTop(selectedElement));

                isDragging = true;
            }
            else if(rbtnPPipe.IsChecked == true)
            {
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;

                var pipe = hydroObjectGraph.AddPressurePipe(
                    hydroObjectGraph,                   
                    hydroObjectGraph.GetHydroObjectByElement(node2) as HydroVertex);

                (pipe as Line).X1 = Canvas.GetLeft(node1) + node1.ActualWidth / 2;
                (pipe as Line).X2 = Canvas.GetLeft(node2) + node2.ActualWidth / 2;
                (pipe as Line).Y1 = Canvas.GetTop(node1) + node1.ActualHeight / 2;
                (pipe as Line).Y2 = Canvas.GetTop(node2) + node2.ActualHeight / 2;
                drawingCanvas.Children.Add(pipe);
            }
            else if(rbtnCNode.IsChecked == true)
            {
                var cNode = hydroObjectGraph.AddConnectNode();          
                Canvas.SetLeft(cNode, hitPosition.X - cNode.Width / 2);
                Canvas.SetTop(cNode, hitPosition.Y - cNode.Height / 2);
                drawingCanvas.Children.Add(cNode);
            }
        }

        private void drawingCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void drawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (rbtnPointer.IsChecked == true)
            {
                if (selectedElement != null && isDragging == true)
                {
                    var drawPosition = e.GetPosition(drawingCanvas) - clickOffset;
                    Canvas.SetLeft(selectedElement, drawPosition.X);
                    Canvas.SetTop(selectedElement, drawPosition.Y);
                }                  
            }
        }

        abstract class MainWindowState
        {
            protected MainWindow container;

            public MainWindowState(MainWindow _container)
            {
                container = _container;
                container.drawingCanvas.MouseMove += OnDrawingCanvasMouseMove;
                container.drawingCanvas.MouseLeftButtonDown += OnDrawingCanvasMouseLeftButtonDown;
                container.drawingCanvas.MouseRightButtonDown += OnDrawingCanvasMouseRightButtonDown;
            }

            protected virtual void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseRightButtonDown(object sender, MouseButtonEventArgs e)
            { }
        }
    }
}
