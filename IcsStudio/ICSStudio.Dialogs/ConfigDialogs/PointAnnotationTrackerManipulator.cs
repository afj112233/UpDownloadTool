using System.Collections.ObjectModel;
using System.Linq;
using OxyPlot;
using OxyPlot.Annotations;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    class PointAnnotationTrackerManipulator : MouseManipulator
    {
        private const double MouseHitTolerance = 10;
        private ObservableCollection<CamPoint> _points;
        private PointAnnotation _currentPoint;

        public PointAnnotationTrackerManipulator(IPlotView plotView, ObservableCollection<CamPoint> camPoints) :
            base(plotView)
        {
            _currentPoint = null;
            _points = camPoints;
        }

        public override void Started(OxyMouseEventArgs e)
        {
            base.Started(e);

            Delta(e);
        }

        public override void Delta(OxyMouseEventArgs e)
        {
            base.Delta(e);
            e.Handled = true;

            if (PlotView.ActualModel == null)
                return;

            var args = new HitTestArguments(e.Position, MouseHitTolerance);
            var firstHit = PlotView.ActualModel.HitTest(args).FirstOrDefault(x => x.Element is PointAnnotation);
            if (firstHit != null)
            {
                _currentPoint = firstHit.Element as PointAnnotation;
            }
            else
            {
                _currentPoint = null;
            }

            if (_currentPoint != null)
            {
                int index = -1;
                for (int i = 0; i < _points.Count; i++)
                {
                    if (_points[i].Master== _currentPoint.X)
                    {
                        index = i;
                    }
                }

                TrackerHitResult result = new TrackerHitResult();
                result.Text = "Master: " + $"{_currentPoint.X}" + "\n: " + $"{_currentPoint.Y}" + "\nIndex :" + index;
                result.Position = e.Position;

                PlotView.ShowTracker(result);
                PlotView.ActualModel.RaiseTrackerChanged(result);
            }
            else
            {
                PlotView.HideTracker();
                PlotView.ActualModel.RaiseTrackerChanged(null);
            }

        }

        public override void Completed(OxyMouseEventArgs e)
        {
            base.Completed(e);
            e.Handled = true;

            _currentPoint = null;
            PlotView.HideTracker();

            if (PlotView.ActualModel != null)
            {
                PlotView.ActualModel.RaiseTrackerChanged(null);
            }
        }
    }
}

