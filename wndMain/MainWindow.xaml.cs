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
                set
                {
                    if (value != drawingCanvas)
                        container.selectedElement = value;
                }
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
                else if(e.OriginalSource == container.rbtnDelete)
                {
                    newState = new MainWindowDeleting(container);
                }
                else if (e.OriginalSource == container.rbtnReConn)
                {
                    newState = null;
                }
                else if (e.OriginalSource == container.rbtnShiftScr)
                {
                    newState = null;
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

            protected void RemoveObjectAndElementData(HydroObject hydroObject, FrameworkElement element)
            {
                hydroObjectGraph.Remove(hydroObject);
                elementDictionary.Remove(element);
                drawingCanvas.Children.Remove(element);
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
                if (selectedElement != null)
                {
                    var selectedObject = elementDictionary[selectedElement];
                    if (selectedObject is HydroVertex && isDragging == true)
                    {  
                        var drawPosition = e.GetPosition(drawingCanvas) - clickOffset;
                        Canvas.SetLeft(selectedElement, drawPosition.X);
                        Canvas.SetTop(selectedElement, drawPosition.Y);

                        var connectEdges = hydroObjectGraph.GetEdges(selectedObject as HydroVertex);
                        foreach (var edge in connectEdges)
                        {
                            var element = elementDictionary.First(kvp => { return kvp.Value == edge; }).Key as Line;
                            if (hydroObjectGraph.GetVertex1(edge as HydroEdge) == selectedObject)
                            {
                                element.X1 = Canvas.GetLeft(selectedElement) + selectedElement.Width / 2;
                                element.Y1 = Canvas.GetTop(selectedElement) + selectedElement.Height / 2;
                            }
                            else
                            {
                                element.X2 = Canvas.GetLeft(selectedElement) + selectedElement.Width / 2;
                                element.Y2 = Canvas.GetTop(selectedElement) + selectedElement.Height / 2;
                            }
                        }
                    }
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                if (selectedElement != null)
                    selectedElement.Tag = "";

                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                if (hitResult.VisualHit == null || hitResult.VisualHit == drawingCanvas)
                    return;

                selectedElement = hitResult.VisualHit as FrameworkElement;
                selectedElement.Tag = "Selected";

                clickOffset = hitPosition - new Point(Canvas.GetLeft(selectedElement), Canvas.GetTop(selectedElement));
                isDragging = true;
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isDragging = false;
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);

                if (selectedElement != null)
                    selectedElement.Tag = null;
            }
        }

        class MainWindowDeleting : MainWindowState
        {
            public MainWindowDeleting(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;

                if (hitElement != drawingCanvas)
                {
                    var hitObject = elementDictionary[hitElement];
                    if (hitObject is HydroObject)
                    {
                        RemoveObjectAndElementData(hitObject, hitElement);
                    }
                }
            }
        }

        class MainWindowReconnecting : MainWindowState
        {
            public MainWindowReconnecting(MainWindow _container) : base(_container)
            {

            }
        }

        class MainWindowShiftingScreen : MainWindowState
        {
            public MainWindowShiftingScreen(MainWindow _container) : base(_container)
            {

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

            private Line element;

            private PressurePipe pPipe
            { get; set; }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                var hitElement = hitResult.VisualHit as FrameworkElement;

                if(hitElement == drawingCanvas)
                {
                    pPipe = hydroObjectGraph.AddPressurePipe(null, null);
                    element = pPipe.VisualElement as Line;
                    elementDictionary.Add(element, pPipe);
                    element.X1 = hitPosition.X;
                    element.Y1 = hitPosition.Y;
                }
                else
                {
                    var hitObject = elementDictionary[hitElement];
                    if (hitObject is HydroVertex)
                    {
                        pPipe = hydroObjectGraph.AddPressurePipe(hitObject as HydroVertex, null);
                        element = pPipe.VisualElement as Line;
                        elementDictionary.Add(element, pPipe);
                        var hitVertex = hitElement as FrameworkElement;
                        element.X1 = Canvas.GetLeft(hitElement) + hitVertex.Width / 2;
                        element.Y1 = Canvas.GetTop(hitElement) + hitVertex.Height / 2;
                    }
                }

                element.X2 = element.X1;
                element.Y2 = element.Y1;
                Canvas.SetZIndex(element, -1);
                drawingCanvas.Children.Add(element);

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
                get{ return elementDictionary[element] as PressurePipe; }                
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

                if (hitElement == drawingCanvas)
                {
                    element.X2 = hitPosition.X;
                    element.Y2 = hitPosition.Y;
                }
                else
                {
                    var hitObject = elementDictionary[hitElement];
                    if (hitObject is HydroVertex)
                    {        
                        hydroObjectGraph.SetVertex2(pPipe, hitObject as HydroVertex);
                        element.X2 = Canvas.GetLeft(hitElement) + hitElement.Width / 2;
                        element.Y2 = Canvas.GetTop(hitElement) + hitElement.Height / 2;
                    }
                }

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this,
                    new MainWindowSettingFirstPPipeNode(container));
                OnChangeState(this, re);
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);

                if (!(newState is MainWindowSettingFirstPPipeNode))
                {
                    RemoveObjectAndElementData(pPipe, element);
                }
            }
        }
    }
}
