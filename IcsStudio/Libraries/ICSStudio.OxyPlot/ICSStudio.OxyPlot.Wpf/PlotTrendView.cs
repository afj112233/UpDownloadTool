using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ICSStudio.Gui.Utils;
using ICSStudio.Interfaces.Common;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using ICSStudio.UIInterfaces.Dialog;

namespace OxyPlot.Wpf
{
    /// <summary>
    /// Represents a control that displays a <see cref="PlotModel" />.
    /// </summary>
    [TemplatePart(Name = PartGrid, Type = typeof(Grid))]
    public class PlotTrendView : PlotBase
    {
        /// <summary>
        /// Identifies the <see cref="Controller"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IPlotController), typeof(PlotTrendView));

        /// <summary>
        /// Identifies the <see cref="Model"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty =
            DependencyProperty.Register("Model", typeof(PlotModel), typeof(PlotTrendView), new PropertyMetadata(null, ModelChanged));

        /// <summary>
        /// The model lock.
        /// </summary>
        private readonly object modelLock = new object();

        /// <summary>
        /// The current model (synchronized with the <see cref="Model" /> property, but can be accessed from all threads.
        /// </summary>
        private PlotModel currentModel;

        /// <summary>
        /// The default plot controller.
        /// </summary>
        private IPlotController defaultController;

        /// <summary>
        /// Initializes static members of the <see cref="PlotView" /> class.
        /// </summary>
        static PlotTrendView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PlotTrendView), new FrameworkPropertyMetadata(typeof(PlotTrendView)));
            PaddingProperty.OverrideMetadata(typeof(PlotTrendView), new FrameworkPropertyMetadata(new Thickness(8), AppearanceChanged));
        }

        //public PlotTrendView()
        //{
        //    AddFlag = true;
        //}
        /// <summary>
        /// Gets or sets the model.
        /// </summary>
        /// <value>The model.</value>
        public PlotModel Model
        {
            get
            {
                return (PlotModel)this.GetValue(ModelProperty);
            }

            set
            {
                if (Model != null)
                    Model.Updated -= this.Value_Updated;
                this.SetValue(ModelProperty, value);
                if (value != null)
                    value.Updated += Value_Updated;
            }
        }

        private void Value_Updated(object sender, System.EventArgs e)
        {
            this.InvalidateArrange();
        }

        /// <summary>
        /// Gets or sets the Plot controller.
        /// </summary>
        /// <value>The Plot controller.</value>
        public IPlotController Controller
        {
            get
            {
                return (IPlotController)this.GetValue(ControllerProperty);
            }

            set
            {
                this.SetValue(ControllerProperty, value);
            }
        }

        /// <summary>
        /// Gets the actual model.
        /// </summary>
        /// <value>The actual model.</value>
        public override PlotModel ActualModel
        {
            get
            {
                return this.currentModel;
            }
        }

        /// <summary>
        /// Gets the actual PlotView controller.
        /// </summary>
        /// <value>The actual PlotView controller.</value>
        public override IPlotController ActualController
        {
            get
            {
                return this.Controller ?? (this.defaultController ?? (this.defaultController = new PlotController()));
            }
        }

        /// <summary>
        /// Called when the visual appearance is changed.
        /// </summary>
        protected void OnAppearanceChanged()
        {
            this.InvalidatePlot(false);
        }

        /// <summary>
        /// Called when the visual appearance is changed.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void AppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotTrendView)d).OnAppearanceChanged();
        }

        /// <summary>
        /// Called when the model is changed.
        /// </summary>
        /// <param name="d">The sender.</param>
        /// <param name="e">The <see cref="System.Windows.DependencyPropertyChangedEventArgs" /> instance containing the event data.</param>
        private static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((PlotTrendView)d).OnModelChanged();
        }

        /// <summary>
        /// Called when the model is changed.
        /// </summary>
        private void OnModelChanged()
        {
            lock (this.modelLock)
            {
                if (this.currentModel != null)
                {
                    ((IPlotModel)this.currentModel).AttachPlotView(null);
                    this.currentModel = null;
                }

                if (this.Model != null)
                {
                    ((IPlotModel)this.Model).AttachPlotView(this);
                    this.currentModel = this.Model;
                }
            }

            this.InvalidatePlot();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2)
            {
                var position = e.GetPosition(this);
                ThreadHelper.ThrowIfNotOnUIThread();
                var uiShell = (IVsUIShell)Microsoft.VisualStudio.Shell.Package.GetGlobalService(typeof(SVsUIShell));
                var service = (ICreateDialogService) Microsoft.VisualStudio.Shell.Package.GetGlobalService(
                    typeof(SCreateDialogService));
               
                if (position.Y < Model.PlotArea.Top)
                {
                    var window = service.CreateGraphTitle((ITrend)Model.Trend);
                    window.ShowDialog(uiShell);
                    return;
                }
                else if (position.Y > Model.PlotArea.Top && position.Y < Model.PlotArea.Bottom)
                {
                    if (position.X < Model.PlotArea.Left)
                    {
                        var window = service.CreateRSTrendXProperties((ITrend)Model.Trend, 1);
                        window.ShowDialog(uiShell);
                        return;
                    }

                    //}else if (position.X > Model.PlotArea.Left && position.X < Model.PlotArea.Right)
                    //{
                    //    MessageBox.Show("click plotview");
                    //}
                    //else
                    //{
                    //    MessageBox.Show("none");
                    //}
                }
                else
                {
                    var window = service.CreateRSTrendXProperties((ITrend) Model.Trend, 2);
                    window.ShowDialog(uiShell);
                    return;
                }
            }

            if (e.ChangedButton == MouseButton.Left)
            {
                //base.OnMouseDown(e);
                if (e.Handled)
                {
                    return;
                }

                //this.Focus();
                //this.CaptureMouse();

                // store the mouse down point, check it when mouse button is released to determine if the context menu should be shown
                ActualController?.AddMouseManipulator(this,
                    new TrackerManipulator(this) {Snap = false, PointsOnly = false}, e.ToMouseDownEventArgs(this));
                var axisX = Model.Axes.FirstOrDefault(a => a.IsHorizontal()&&a.IsAxisVisible);
                ScreenPoint screenPoint = e.GetPosition(this).ToScreenPoint();
                if (Model.IsShowDelta)
                {
                    if (Model.InIsolated)
                    {
                        foreach (var plotModel in Model.ParentCollection)
                        {
                            plotModel.ValueBarDeltaX = axisX.InverseTransform(screenPoint.X);
                        }
                    }
                    else
                    {
                        Model.ValueBarDeltaX = axisX.InverseTransform(screenPoint.X);
                    }
                }
                else
                {
                    if (Model.InIsolated)
                    {
                        foreach (var plotModel in Model.ParentCollection)
                        {
                            plotModel.ValueBarX = axisX.InverseTransform(screenPoint.X);
                        }
                    }
                    else
                        Model.ValueBarX = axisX.InverseTransform(screenPoint.X);
                }

                return;
            }

            if (ContextMenu != null)
            {
                ContextMenu.Visibility = Visibility.Visible;
                ContextMenu.IsOpen = true;
            }

            e.Handled = true;
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (e.Delta < 0)
            {
                e.Handled = true;
                return;
            }

            if (ActualModel != null)
            {
                if (ActualModel.InIsolated)
                {
                    e.Handled = true;
                    return;
                }
                ActualModel.IsZoom = true;
            }
            var zoom = (MenuItem)ContextMenu?.Items.GetItemAt(3);
            if (zoom != null)
            {
                zoom.IsEnabled = true;
            }

            Model.IsOnMouseWheel = true;
            base.OnMouseWheel(e);
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            HideTracker();
        }
    }
}
