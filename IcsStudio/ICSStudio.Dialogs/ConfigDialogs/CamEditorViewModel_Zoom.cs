using System;
using ICSStudio.Utils.CamEditorUtil;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public partial class CamEditorViewModel
    {
        private void ZoomToFitPositionAxis()
        {
            double valueLength = 0;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            if (CamPoints.Count > 0)
            {
                for (int i = 0; i < CamPoints.Count; i++)
                {
                    var camPoint = CamPoints[i];
                    double y = camPoint.Slave;

                    if (y < minValue)
                        minValue = y;

                    if (y > maxValue)
                        maxValue = y;

                    if (camPoint.Type == SegmentType.Cubic && i < CamPoints.Count - 1)
                    {
                        //y0 + c0 + c1 * dx + c2 * dx * dx + c3 * dx * dx * dx;

                        double x0 = camPoint.Master;
                        double y0 = camPoint.Slave;
                        double c0 = camPoint.C0;
                        double c1 = camPoint.C1;
                        double c2 = camPoint.C2;
                        double c3 = camPoint.C3;

                        double maxX = CamPoints[i + 1].Master - x0;

                        if (c3 != 0)
                        {
                            // b*b-3ac
                            double delta = c2 * c2 - 3 * c3 * c1;
                            if (delta > 0)
                            {
                                double xMax = (-c2 - Math.Sqrt(delta)) / (3 * c3);
                                double xMin = (-c2 + Math.Sqrt(delta)) / (3 * c3);

                                if (xMax >= 0 && xMax <= maxX)
                                {
                                    double yMax = y0 + c0 + c1 * xMax + c2 * xMax * xMax + c3 * xMax * xMax * xMax;
                                    if (yMax > maxValue)
                                        maxValue = yMax;
                                }

                                if (xMin >= 0 && xMin <= maxX)
                                {
                                    double yMin = y0 + c0 + c1 * xMin + c2 * xMin * xMin + c3 * xMin * xMin * xMin;
                                    if (yMin < minValue)
                                        minValue = yMin;
                                }
                            }
                        }
                        else if (c2 != 0)
                        {
                            double xPeak = -c1 / (c2 * 2);
                            if (xPeak >= 0 && xPeak <= maxX)
                            {
                                double yPeak = y0 + c0 + c1 * xPeak + c2 * xPeak * xPeak + c3 * xPeak * xPeak * xPeak;
                                if (yPeak < minValue)
                                    minValue = yPeak;
                                if (yPeak > maxValue)
                                    maxValue = yPeak;
                            }
                        }
                    }
                }

                valueLength = maxValue - minValue;
            }

            double targetScale = GetTargetScale(_plotModel.Height, valueLength);

            // scale
            _positionAxis.ZoomAt(Math.Abs(targetScale / _positionAxis.Scale), 0);
            _positionAxis.MajorStep = 0.2 / Math.Abs(targetScale / 300);

            // pan
            if (CamPoints.Count > 1)
            {
                double delta = maxValue - _positionAxis.ActualMaximum;
                _positionAxis.Pan(10 + delta * Math.Abs(_positionAxis.Scale));
            }
            else if (CamPoints.Count == 1)
            {
                double delta = maxValue - (_positionAxis.ActualMinimum + _positionAxis.ActualMaximum) / 2;
                _positionAxis.Pan(delta * Math.Abs(_positionAxis.Scale));
            }
            else
            {
                double delta = 0 - (_positionAxis.ActualMinimum + _positionAxis.ActualMaximum) / 2;
                _positionAxis.Pan(delta * Math.Abs(_positionAxis.Scale));
            }
        }

        private void ZoomToFitVelocityAxis()
        {
            double valueLength = 0;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            if (CamPoints.Count > 0)
            {
                for (int i = 0; i < CamPoints.Count; i++)
                {
                    var camPoint = CamPoints[i];
                    double velocity = camPoint.C1 * _camOptions.MasterVelocity;

                    if (velocity < minValue)
                        minValue = velocity;

                    if (velocity > maxValue)
                        maxValue = velocity;

                    if (camPoint.Type == SegmentType.Cubic && i < CamPoints.Count - 1)
                    {
                        //(c1 + 2 * c2 * dx + 3 * c3 * dx * dx) * _camOptions.MasterVelocity

                        double x0 = camPoint.Master;
                        double y0 = camPoint.Slave;
                        double c0 = camPoint.C0;
                        double c1 = camPoint.C1;
                        double c2 = camPoint.C2;
                        double c3 = camPoint.C3;

                        double maxX = CamPoints[i + 1].Master - x0;

                        if (c3 != 0)
                        {
                            double xPeak = -c2 / (3 * c3);

                            if (xPeak >= 0 && xPeak <= maxX)
                            {
                                double yPeak =
                                    (c1 + 2 * c2 * xPeak + 3 * c3 * xPeak * xPeak) * _camOptions.MasterVelocity;

                                if (yPeak < minValue)
                                    minValue = yPeak;
                                if (yPeak > maxValue)
                                    maxValue = yPeak;
                            }
                        }

                    }

                }


                valueLength = maxValue - minValue;
            }

            double targetScale = GetTargetScale(_plotModel.Height, valueLength);

            // scale
            _velocityAxis.ZoomAt(Math.Abs(targetScale / _velocityAxis.Scale), 0);
            _velocityAxis.MajorStep = 0.2 / Math.Abs(targetScale / 300);


            // pan
            if (CamPoints.Count > 1)
            {
                double delta = maxValue - _velocityAxis.ActualMaximum;
                _velocityAxis.Pan(10 + delta * Math.Abs(_velocityAxis.Scale));
            }
            else if (CamPoints.Count == 1)
            {
                double delta = maxValue - (_velocityAxis.ActualMinimum + _velocityAxis.ActualMaximum) / 2;
                _velocityAxis.Pan(delta * Math.Abs(_velocityAxis.Scale));
            }
            else
            {
                double delta = 0 - (_velocityAxis.ActualMinimum + _velocityAxis.ActualMaximum) / 2;
                _velocityAxis.Pan(delta * Math.Abs(_velocityAxis.Scale));
            }
        }

        private void ZoomToFitAccelerationAxis()
        {
            double valueLength = 0;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            if (CamPoints.Count > 0)
            {
                for (int i = 0; i < CamPoints.Count; i++)
                {
                    var camPoint = CamPoints[i];
                    double acceleration = 2 * camPoint.C2 * _camOptions.MasterVelocity * _camOptions.MasterVelocity;

                    if (acceleration < minValue)
                        minValue = acceleration;

                    if (acceleration > maxValue)
                        maxValue = acceleration;

                    if (camPoint.Type == SegmentType.Cubic && i < CamPoints.Count - 1)
                    {
                        //(2 * c2 + 6 * c3 * dx) * _camOptions.MasterVelocity * _camOptions.MasterVelocity

                        double x0 = camPoint.Master;
                        double y0 = camPoint.Slave;
                        double c0 = camPoint.C0;
                        double c1 = camPoint.C1;
                        double c2 = camPoint.C2;
                        double c3 = camPoint.C3;
                        double maxX = CamPoints[i + 1].Master - x0;

                        double accelerationEnd =
                            (2 * c2 + 6 * c3 * maxX) * _camOptions.MasterVelocity * _camOptions.MasterVelocity;

                        if (accelerationEnd < minValue)
                            minValue = accelerationEnd;
                        if (accelerationEnd > maxValue)
                            maxValue = accelerationEnd;
                    }
                }

                foreach (var camPoint in CamPoints)
                {
                    double acceleration = 2 * camPoint.C2 * _camOptions.MasterVelocity * _camOptions.MasterVelocity;

                    if (acceleration < minValue)
                        minValue = acceleration;

                    if (acceleration > maxValue)
                        maxValue = acceleration;
                }

                valueLength = maxValue - minValue;
            }

            double targetScale = GetTargetScale(_plotModel.Height, valueLength);

            // scale
            _accelerationAxis.ZoomAt(Math.Abs(targetScale / _accelerationAxis.Scale), 0);
            _accelerationAxis.MajorStep = 0.2 / Math.Abs(targetScale / 300);

            //pan
            if (CamPoints.Count > 1)
            {
                double delta = maxValue - _accelerationAxis.ActualMaximum;
                _accelerationAxis.Pan(10 + delta * Math.Abs(_accelerationAxis.Scale));
            }
            else if (CamPoints.Count == 1)
            {
                double delta = maxValue - (_accelerationAxis.ActualMinimum + _accelerationAxis.ActualMaximum) / 2;
                _accelerationAxis.Pan(delta * Math.Abs(_accelerationAxis.Scale));
            }
            else
            {
                double delta = 0 - (_accelerationAxis.ActualMinimum + _accelerationAxis.ActualMaximum) / 2;
                _accelerationAxis.Pan(delta * Math.Abs(_accelerationAxis.Scale));
            }
        }

        private void ZoomToFitJerkAxis()
        {
            double valueLength = 0;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            if (CamPoints.Count > 0)
            {
                foreach (var camPoint in CamPoints)
                {
                    double jerk = (6 * camPoint.C3) *
                                  _camOptions.MasterVelocity *
                                  _camOptions.MasterVelocity *
                                  _camOptions.MasterVelocity;

                    if (jerk < minValue)
                        minValue = jerk;

                    if (jerk > maxValue)
                        maxValue = jerk;
                }

                valueLength = maxValue - minValue;
            }

            double targetScale = GetTargetScale(_plotModel.Height, valueLength);

            // scale
            _jerkAxis.ZoomAt(Math.Abs(targetScale / _jerkAxis.Scale), 0);
            _jerkAxis.MajorStep = 0.2 / Math.Abs(targetScale / 300);

            //pan
            if (CamPoints.Count > 1)
            {
                double delta = maxValue - _jerkAxis.ActualMaximum;
                _jerkAxis.Pan(10 + delta * Math.Abs(_jerkAxis.Scale));
            }
            else if (CamPoints.Count == 1)
            {
                double delta = maxValue - (_jerkAxis.ActualMinimum + _jerkAxis.ActualMaximum) / 2;
                _jerkAxis.Pan(delta * Math.Abs(_jerkAxis.Scale));
            }
            else
            {
                double delta = 0 - (_jerkAxis.ActualMinimum + _jerkAxis.ActualMaximum) / 2;
                _jerkAxis.Pan(delta * Math.Abs(_jerkAxis.Scale));
            }
        }

        private void ZoomToFitAllAxes()
        {
            ZoomToFitPositionAxis();
            ZoomToFitVelocityAxis();
            ZoomToFitAccelerationAxis();
            ZoomToFitJerkAxis();

            ZoomToFitMasterAxis();
        }

        private void ZoomToFitMasterAxis()
        {
            double valueLength = 0;

            double minValue = double.MaxValue;
            double maxValue = double.MinValue;

            if (CamPoints.Count > 0)
            {
                foreach (var camPoint in CamPoints)
                {
                    double x = camPoint.Master;

                    if (x < minValue)
                        minValue = x;

                    if (x > maxValue)
                        maxValue = x;
                }

                valueLength = maxValue - minValue;
            }

            double displayLength = _plotModel.Width - 40;
            if (displayLength < 50)
                displayLength = 50;

            double targetScale = GetTargetScale(displayLength, valueLength);

            // scale
            _masterAxis.ZoomAt(Math.Abs(targetScale / _masterAxis.Scale), 0);
            _masterAxis.MajorStep = 0.2 / Math.Abs(targetScale / 300);

            //pan
            if (CamPoints.Count > 1)
            {
                double delta = _masterAxis.ActualMinimum - minValue;
                _masterAxis.Pan(delta * Math.Abs(_masterAxis.Scale) + 10);
            }
            else if (CamPoints.Count == 1)
            {
                double delta = _masterAxis.ActualMinimum - minValue +
                               (_masterAxis.ActualMaximum - _masterAxis.ActualMinimum) / 4;
                _masterAxis.Pan(delta * Math.Abs(_masterAxis.Scale));
            }
            else
            {
                double delta = _masterAxis.ActualMinimum - 0 +
                               (_masterAxis.ActualMaximum - _masterAxis.ActualMinimum) / 4;
                _masterAxis.Pan(delta * Math.Abs(_masterAxis.Scale));
            }
        }

        private double GetTargetScale(double displayLength, double valueLength)
        {
            // 0.2-1-300
            const double kMinDisplayLength = 10;
            const double kEpsilon = 1e-38;

            double targetScale = 300;

            if (displayLength > kMinDisplayLength && Math.Abs(valueLength) > kEpsilon)
            {
                double scale = displayLength / (valueLength * 300);

                double exponent = Math.Log10(scale);

                double upExponent = Math.Ceiling(exponent);
                double downExponent = Math.Floor(exponent);

                double[] checkScales =
                {
                    Math.Pow(10, downExponent),
                    2 * Math.Pow(10, downExponent),
                    4 * Math.Pow(10, downExponent),
                    Math.Pow(10, upExponent)
                };


                int minIndex = 0;
                for (int i = 0; i < 4; i++)
                {
                    if (Math.Abs(checkScales[i] - scale) < kEpsilon)
                    {
                        minIndex = i;
                        break;
                    }

                    if (checkScales[i] < scale)
                    {
                        minIndex = i;
                    }

                    if (checkScales[i] > scale)
                        break;
                }

                targetScale *= checkScales[minIndex];
            }

            return targetScale;
        }

        private void OnContentRenderedCommand(EventArgs args)
        {
            ZoomToFitAllAxes();

            _plotModel.InvalidatePlot(false);
        }
    }
}
