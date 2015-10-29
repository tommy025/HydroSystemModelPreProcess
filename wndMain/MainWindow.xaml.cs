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

        public FrameworkElement SelectedElement
        {
            get { return selectedElement; }           
            set
            {
                if (selectedElement == value ||
                    selectedElement != null && 
                    !elementDictionary.ContainsKey(selectedElement))
                    return;

                if (selectedElement != null)
                    selectedElement.Tag = "";

                selectedElement = value;
                if (selectedElement != null)
                    selectedElement.Tag = "Selected";
            }
        }

        public TranslateTransform Transform
        { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            mainWindowState = new MainWindowSelecting(this);
            hydroObjectGraph = new HydroObjectGraph();
            elementDictionary = new Dictionary<FrameworkElement, HydroObject>();
            Transform = new TranslateTransform();
        }

        private Rectangle AddConnectNode(Point position)
        {
            var cNode = new ConnectNode();
            var element = ConnectNode.GetVisualElement();
            hydroObjectGraph.Add(cNode);
            elementDictionary.Add(element, cNode);

            Canvas.SetLeft(element, position.X - element.Width / 2);
            Canvas.SetTop(element, position.Y - element.Height / 2);
            element.RenderTransform = Transform;
            drawingCanvas.Children.Add(element);

            return element;
        }

        private Line AddPressurePipe(Visibility visibility)
        {
            var pPipe = new PressurePipe();
            var element = PressurePipe.GetVisualElement();
            hydroObjectGraph.Add(pPipe);
            elementDictionary.Add(element, pPipe);         

            Canvas.SetZIndex(element, -1);
            element.RenderTransform = Transform;
            drawingCanvas.Children.Add(element);
            element.Visibility = visibility;

            return element;
        }

        private void MoveNode(Rectangle node, Point position)
        {
            var selectedObject = elementDictionary[SelectedElement];

            Canvas.SetLeft(SelectedElement, position.X);
            Canvas.SetTop(SelectedElement, position.Y);

            var connectEdges = hydroObjectGraph.GetEdges(selectedObject as HydroVertex);
            foreach (var edge in connectEdges)
            {
                var element = elementDictionary.First(kvp => { return kvp.Value == edge; }).Key as Line;
                if (hydroObjectGraph.GetVertex1(edge as HydroEdge) == selectedObject)
                {
                    element.X1 = Canvas.GetLeft(SelectedElement) + SelectedElement.Width / 2;
                    element.Y1 = Canvas.GetTop(SelectedElement) + SelectedElement.Height / 2;
                }
                else
                {
                    element.X2 = Canvas.GetLeft(SelectedElement) + SelectedElement.Width / 2;
                    element.Y2 = Canvas.GetTop(SelectedElement) + SelectedElement.Height / 2;
                }
            }
        }     

        private void MoveScreen(Vector v)
        {
            Transform.X += v.X;
            Transform.Y += v.Y;
        }

        private Point TransformToCanvasCoSys(Point p)
        {
            return Transform.Inverse.Transform(p);
        }

        private void SetPipeFirstPoint(FrameworkElement element, Point position, bool shouldTransform)
        {
            if (element is Line)
            {
                var pPipe = element as Line;
                if (shouldTransform)
                    position = TransformToCanvasCoSys(position);

                pPipe.X1 = position.X;
                pPipe.Y1 = position.Y;
            }
        }

        private void SetPipeSecondPoint(FrameworkElement element, Point position, bool shouldTransform)
        {
            if (element is Line)
            {
                var pPipe = element as Line;
                if (shouldTransform)
                    position = TransformToCanvasCoSys(position);

                pPipe.X2 = position.X;
                pPipe.Y2 = position.Y;
            }
        }

        private bool SetPipeFirstNode(FrameworkElement element, Rectangle node)
        {
            if (element is Line)
            {
                var pPipe = element as Line;
                var hEdge = elementDictionary[element] as HydroEdge;

                if (node == null)
                {
                    hydroObjectGraph.SetVertex1(hEdge, null);
                    return true;
                }

                var hVertex = elementDictionary[node] as HydroVertex;
                if (hydroObjectGraph.GetVertex2(hEdge) == hVertex)
                    return false;

                if (hydroObjectGraph.GetVertex1(hEdge) != hVertex)
                    hydroObjectGraph.SetVertex1(hEdge, hVertex);

                SetPipeFirstPoint(element, new Point(
                    Canvas.GetLeft(node) + node.Width / 2,
                    Canvas.GetTop(node) + node.Height / 2),
                    false);

                return true;
            }
            else
                return false;
        }

        private bool SetPipeSecondNode(FrameworkElement element, Rectangle node)
        {
            if (element is Line)
            {
                var pPipe = element as Line;
                var hEdge = elementDictionary[element] as HydroEdge;

                if (node == null)
                {
                    hydroObjectGraph.SetVertex1(hEdge, null);
                    return true;
                }

                var hVertex = elementDictionary[node] as HydroVertex; 
                if (hydroObjectGraph.GetVertex1(hEdge) == hVertex)
                    return false;

                var hOtherVertex = hydroObjectGraph.GetVertex1(hEdge);
                if (hydroObjectGraph.IsConnected(hOtherVertex, hVertex) &&
                    !hydroObjectGraph.IsBetween(hEdge, hVertex, hOtherVertex)) 
                    return false;

                if (hydroObjectGraph.GetVertex2(hEdge) != hVertex)
                    hydroObjectGraph.SetVertex2(hEdge, hVertex);

                SetPipeSecondPoint(element, new Point(
                    Canvas.GetLeft(node) + node.Width / 2,
                    Canvas.GetTop(node) + node.Height / 2),
                    false);

                return true;
            }
            else
                return false;
        }

        private void RemoveObjectAndElementData(FrameworkElement element)
        {
            var hydroObject = elementDictionary[element];
            hydroObjectGraph.Remove(hydroObject);
            elementDictionary.Remove(element);
            drawingCanvas.Children.Remove(element);
        }

        private abstract class MainWindowState
        {
            protected MainWindow container;

            protected CommandBinding commandBinding;

            public MainWindowState(MainWindow _container)
            {
                container = _container;
                drawingCanvas.MouseMove += OnDrawingCanvasMouseMove;
                drawingCanvas.MouseLeftButtonDown += OnDrawingCanvasMouseLeftButtonDown;
                drawingCanvas.MouseLeftButtonUp += OnDrawingCanvasMouseLeftButtonUp;

                commandBinding = new CommandBinding(MainWindow.ChangeState);
                commandBinding.Executed += OnChangeState;
                commandBinding.CanExecute += CanChangeState;
                container.CommandBindings.Add(commandBinding);
            }

            protected FrameworkElement SelectedElement
            {
                get { return container.SelectedElement; }
                set { container.SelectedElement = value; }
                
            }

            protected Canvas drawingCanvas
            {
                get{ return container.drawingCanvas; }
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
                    newState = new MainWindowReconnecting(container);
                }
                else if (e.OriginalSource == container.rbtnShiftScr)
                {
                    newState = new MainWindowShiftingScreen(container);
                }
                else if (e.OriginalSource == container.rbtnCNode)
                {
                    newState = new MainWindowAddingCNode(container);
                }
                else if (e.OriginalSource == container.rbtnPPipe)
                {
                    newState = new MainWindowSettingFirstPPipeNode(container, null);
                }

                LeaveState(newState);
                container.mainWindowState = newState;
            }

            /// <summary>
            /// Overrided method should not remove 'base.LeaveState' unless you know what you're doing.
            /// </summary>
            /// <param name="newState"></param>
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

        private class MainWindowSelecting : MainWindowState
        {
            public MainWindowSelecting(MainWindow _container) : base(_container)
            { }

            protected bool isDragging;

            protected Vector clickOffset;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if (SelectedElement != null)
                {
                    if (SelectedElement is Rectangle && isDragging == true)
                    { 
                        var drawPosition = e.GetPosition(drawingCanvas) - clickOffset;
                        container.MoveNode(SelectedElement as Rectangle, drawPosition);
                    }
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitResult = VisualTreeHelper.HitTest(drawingCanvas, hitPosition);
                if (hitResult.VisualHit == null || hitResult.VisualHit == drawingCanvas)
                    return;

                SelectedElement = hitResult.VisualHit as FrameworkElement;
                clickOffset = hitPosition - new Point(Canvas.GetLeft(SelectedElement), Canvas.GetTop(SelectedElement));
                isDragging = true;
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {
                isDragging = false;
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                SelectedElement = null;
            }
        }

        private class MainWindowDeleting : MainWindowState
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
                    container.RemoveObjectAndElementData(hitElement);
                }
            }
        }

        private class MainWindowReconnecting : MainWindowState
        {
            public MainWindowReconnecting(MainWindow _container) : base(_container)
            {

            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(drawingCanvas, hitPosition).VisualHit as FrameworkElement;

                if (hitElement is Line)
                {
                    var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this,
                        new MainWindowSettingFirstPPipeNode(container, hitElement as Line));
                    OnChangeState(this, re);
                }
                    
            }
        }

        private class MainWindowShiftingScreen : MainWindowState
        {
            public MainWindowShiftingScreen(MainWindow _container) : base(_container)
            {

            }

            protected bool isDragging;

            protected Point lastPos;

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if (isDragging == true)
                {
                    var vector = e.GetPosition(drawingCanvas) - lastPos;                 
                    container.MoveScreen(vector);
                    lastPos = e.GetPosition(drawingCanvas);
                }              
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {               
                isDragging = true;
                lastPos = e.GetPosition(drawingCanvas);
            }

            protected override void OnDrawingCanvasMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
            {              
                isDragging = false;
                lastPos = e.GetPosition(drawingCanvas);
            }
        }

        private class MainWindowAddingCNode : MainWindowState
        {
            public MainWindowAddingCNode(MainWindow _container) : base(_container)
            { }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = container.TransformToCanvasCoSys(e.GetPosition(drawingCanvas));
                container.AddConnectNode(hitPosition);
            }
        }

        private class MainWindowSettingFirstPPipeNode : MainWindowState
        {
            public MainWindowSettingFirstPPipeNode(MainWindow _container, Line line) : base(_container)
            {
                if (line == null)
                {
                    element = container.AddPressurePipe(Visibility.Hidden);
                    isCreating = true;
                }
                else
                {
                    element = line;
                    originalElement = VisualTreeHelper.HitTest(drawingCanvas, new Point(line.X1, line.Y1)).VisualHit as Rectangle;
                    container.SetPipeFirstNode(element, null);
                }
            }

            private bool isCreating;

            private Rectangle originalElement;

            private Line element
            { get; set; }

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                if(!isCreating)
                {
                    var mousePos = e.GetPosition(drawingCanvas);
                    container.SetPipeFirstPoint(element, mousePos, true);
                }
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(drawingCanvas, hitPosition).VisualHit as FrameworkElement;
                if (hitElement == drawingCanvas || hitElement == element)
                {
                    container.SetPipeFirstPoint(element, hitPosition, true);
                }
                else if (hitElement is Rectangle)
                {
                    if (container.SetPipeFirstNode(element, hitElement as Rectangle) == false)
                        return;
                }
                else
                    return;
                
                container.SetPipeSecondPoint(element, hitPosition, true);
                element.Visibility = Visibility.Visible;

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this, 
                    new MainWindowSettingSecondPPipeNode(container, element, isCreating));              
                OnChangeState(this, re);
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                if(!isCreating)
                {
                    if(!(newState is MainWindowSettingSecondPPipeNode))
                    {
                        container.SetPipeFirstNode(element, originalElement);
                    }
                }    
                else
                {
                    if (!(newState is MainWindowSettingSecondPPipeNode))
                    {
                        container.RemoveObjectAndElementData(element);
                    }
                }         
            }
        }

        private class MainWindowSettingSecondPPipeNode : MainWindowState
        {
            public MainWindowSettingSecondPPipeNode(MainWindow _container, Line line, bool _isCreating) : base(_container)
            {
                if (line == null)
                    throw new ArgumentNullException("Input line element must not be null!");

                element = line;
                originalElement = VisualTreeHelper.HitTest(drawingCanvas, new Point(line.X2, line.Y2)).VisualHit as Rectangle;
                container.SetPipeSecondNode(element, null);
                isCreating = _isCreating;
            }

            private bool isCreating;

            private Rectangle originalElement;

            private Line element
            { get; set; }

            protected override void OnDrawingCanvasMouseMove(object sender, MouseEventArgs e)
            {
                var mousePos = e.GetPosition(drawingCanvas);
                container.SetPipeSecondPoint(element, mousePos, true);
            }

            protected override void OnDrawingCanvasMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
            {
                var hitPosition = e.GetPosition(drawingCanvas);
                var hitElement = VisualTreeHelper.HitTest(drawingCanvas, hitPosition).VisualHit as FrameworkElement;
                if (hitElement == drawingCanvas || hitElement == element)
                {
                    container.SetPipeSecondPoint(element, hitPosition, true);
                }
                else if (hitElement is Rectangle)
                {
                    if (container.SetPipeSecondNode(element, hitElement as Rectangle) == false)
                        return;
                }
                else
                    return;

                var re = new RoutedPropertyChangedEventArgs<MainWindowState>(this, (isCreating ?
                    new MainWindowSettingFirstPPipeNode(container, null) as MainWindowState :
                    new MainWindowReconnecting(container) as MainWindowState));
                OnChangeState(this, re);
            }

            protected override void LeaveState(MainWindowState newState)
            {
                base.LeaveState(newState);
                if (!isCreating)
                {
                    if (!(newState is MainWindowReconnecting))
                        container.SetPipeSecondNode(element, originalElement);
                }
                else
                {
                    if (!(newState is MainWindowSettingFirstPPipeNode))
                        container.RemoveObjectAndElementData(element);
                }
            }
        }
    }
}
