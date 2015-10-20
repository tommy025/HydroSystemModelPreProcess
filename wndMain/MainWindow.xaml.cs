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
        private static RoutedUICommand changeState = new RoutedUICommand("ChangeState", "ChangeState", typeof(MainWindow));

        public static RoutedUICommand ChangeState
        {
            get { return changeState; }
        }

        private HydroObjectGraph hydroObjectGraph;

        private MainWindowState mainWindowState;

        private Dictionary<FrameworkElement, HydroObject> elementDictionary;

        private FrameworkElement selectedElement;

        public MainWindow()
        {
            InitializeComponent();
            mainWindowState = new MainWindowSelecting(this);
            hydroObjectGraph = new HydroObjectGraph();
            elementDictionary = new Dictionary<FrameworkElement, HydroObject>();
        }
                
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        abstract class MainWindowState
        {
            protected MainWindow container;

            protected CommandBinding commandBinding;

            public MainWindowState(MainWindow _container)
            {
                container = _container;
                drawingCanvas.MouseMove += OnDrawingCanvasMouseMove;
                drawingCanvas.MouseLeftButtonDown += OnDrawingCanvasMouseLeftButtonDown;
                drawingCanvas.MouseLeftButtonUp += OnDrawingCanvasMouseLeftButtonUp;

                commandBinding = new CommandBinding(MainWindow.changeState);
                commandBinding.Executed += OnChangeState;
                commandBinding.CanExecute += CanChangeState;
                container.CommandBindings.Add(commandBinding);
            }

            protected FrameworkElement selectedElement
            {
                get { return container.selectedElement; }
                set { container.selectedElement = value; }
            }

            protected Canvas drawingCanvas
            {
                get{ return container.drawingCanvas; }
            }

            protected HydroObjectGraph hydroObjectGraph
            {
                get { return container.hydroObjectGraph; }
            }

            protected Dictionary<FrameworkElement, HydroObject> elementDictionary
            {
                get { return container.elementDictionary; }
            }

            protected virtual void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            { }

            protected virtual void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            { }

            protected void OnChangeState(object sender, RoutedEventArgs e)
            {        
                var newState = null as MainWindowState;
                if(sender == this)
                {
                    newState = (e as RoutedPropertyChangedEventArgs<MainWindowState>).NewValue;
                }
                else if (e.OriginalSource == container.rbtnPointer)
                {
                    newState = new MainWindowSelecting(container);
                }
                else if (e.OriginalSource == container.rbtnCNode)
                {
                    newState = new MainWindowAddingCNode(container);
                }
                else if (e.OriginalSource == container.rbtnPPipe)
                {
                    newState = new MainWindowSettingFirstPPipeNode(container);
                }

                LeaveState(newState);
                container.mainWindowState = newState;
            }

            protected virtual void LeaveState(MainWindowState newState)
            {
                drawingCanvas.MouseMove -= OnDrawingCanvasMouseMove;
                drawingCanvas.MouseLeftButtonDown -= OnDrawingCanvasMouseLeftButtonDown;
                drawingCanvas.MouseLeftButtonUp -= OnDrawingCanvasMouseLeftButtonUp;
                container.CommandBindings.Remove(commandBinding);
            }

            protected virtual void CanChangeState(object sender, CanExecuteRoutedEventArgs e)
            {
                e.CanExecute = true;
            }
        }

        class MainWindowSelecting : MainWindowState
        {
            public MainWindowSelecting(MainWindow _container) : base(_container)
            { }

            protected bool isDragging;

            protected Vector clickOffset;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if (selectedElement != null && isDragging == true)
                {
                    var drawPosition = e.GetPosition(drawingCanvas) - clickOffset;
                    Canvas.SetLeft(selectedElement, drawPosition.X);
                    Canvas.SetTop(selectedElement, drawPosition.Y);
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                if (selectedElement != null)
                    selectedElement.Tag = "";

                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                selectedElement = hitResult.VisualHit as FrameworkElement;
                selectedElement.Tag = "Selected";

                clickOffset = hitPosition - new Point(Canvas.GetLeft(selectedElement), Canvas.GetTop(selectedElement));
                isDragging = true;
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isDragging = false;
            }
        }

        class MainWindowAddingCNode : MainWindowState
        {
            public MainWindowAddingCNode(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var cNode = hydroObjectGraph.AddConnectNode();
                var element = cNode.VisualElement;
                elementDictionary.Add(element, cNode);
                Canvas.SetLeft(element, hitPosition.X - element.Width / 2);
                Canvas.SetTop(element, hitPosition.Y - element.Height / 2);
                drawingCanvas.Children.Add(element);
            }
        }

        class MainWindowSettingFirstPPipeNode : MainWindowState
        {
            public MainWindowSettingFirstPPipeNode(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;
                var hitObject = elementDictionary[hitElement];

                Line element = null;
                if(hitElement == null || !(hitObject is HydroVertex))
                {
                    var pPipe = hydroObjectGraph.AddPressurePipe(null, null);
                    element = pPipe.VisualElement as Line;
                    elementDictionary.Add(element, pPipe);
                    element.X1 = hitPosition.X;
                    element.Y1 = hitPosition.Y;
                    element.X2 = element.X1;
                    element.Y2 = element.Y1;
                    drawingCanvas.Children.Add(element);
                }
                else
                {
                    var pPipe = hydroObjectGraph.AddPressurePipe(hitObject as HydroVertex, null);
                    element = pPipe.VisualElement as Line;
                    elementDictionary.Add(element, pPipe);
                    var hitVertex = (hitObject as HydroVertex).VertexVisualElement;
                    element.X1 = Canvas.GetLeft(hitElement) + hitVertex.Width / 2;
                    element.Y1 = Canvas.GetTop(hitElement) + hitVertex.Height / 2;
                    element.X2 = element.X1;
                    element.Y2 = element.Y1;
                    drawingCanvas.Children.Add(element);
                }

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this, 
                    new MainWindowSettingSecondPPipeNode(container, element));              
                OnChangeState(this, re);
            }
        }

        class MainWindowSettingSecondPPipeNode : MainWindowState
        {
            public MainWindowSettingSecondPPipeNode(MainWindow _container, Line line) : base(_container)
            {
                element = line;
            }

            private Line element;

            private PressurePipe pPipe
            {
                get{ return container.elementDictionary[element] as PressurePipe; }                
            }

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                var relativePos = e.GetPosition(drawingCanvas);
                element.X2 = relativePos.X;
                element.Y2 = relativePos.Y;
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;
                var hitObject = elementDictionary[hitElement];

                if (hitElement == null || !(hitObject is HydroVertex))
                {                
                    element.X2 = hitPosition.X;
                    element.Y2 = hitPosition.Y;
                }
                else
                {
                    hydroObjectGraph.SetVertex2(pPipe, hitObject as HydroVertex);
                    element.X2 = Canvas.GetLeft(hitElement) + hitElement.Width / 2;
                    element.Y2 = Canvas.GetTop(hitElement) + hitElement.Height / 2;
                }

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this,
                    new MainWindowSettingFirstPPipeNode(container));
                OnChangeState(this, re);
            }
        }
    }
}
