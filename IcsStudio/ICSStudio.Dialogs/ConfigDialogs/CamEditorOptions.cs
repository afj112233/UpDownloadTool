using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using ICSStudio.Gui.Annotations;
using ICSStudio.Utils;
using OxyPlot;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    [Serializable]
    public class CamEditorOptions : INotifyPropertyChanged, ISerializable, ICloneable
    {

        private const string ConfigFileName = "CamEditorOptions.cfg";

        private CamEditorOptions()
        {
            MasterVelocity = 1;

            Position = new AxisOptions()
            {
                SlaveValue = "Position",
                Color = OxyColors.Blue,
                Width = 1,
                Visible = true,
                Style = LineStyle.Automatic,
                Marker = MarkerType.Circle
            };

            Velocity = new AxisOptions()
            {
                SlaveValue = "Velocity",
                Color = OxyColors.Red,
                Width = 1,
                Visible = true,
                Style = LineStyle.LongDash,
                Marker = MarkerType.Square
            };

            Acceleration = new AxisOptions()
            {
                SlaveValue = "Acceleration",
                Color = OxyColors.Green,
                Width = 1,
                Visible = true,
                Style = LineStyle.Dot,
                Marker = MarkerType.Triangle
            };

            Jerk = new AxisOptions()
            {
                SlaveValue = "Jerk",
                Color = OxyColors.DarkRed,
                Width = 1,
                Visible = true,
                Style = LineStyle.DashDot,
                Marker = MarkerType.Diamond
            };
        }

        public double _masterVelocity;

        public double MasterVelocity
        {
            get { return _masterVelocity; }
            set
            {
                if (_masterVelocity != value)
                {
                    _masterVelocity = value;
                    OnPropertyChanged(nameof(MasterVelocity));
                }
            }
        }

        public static CamEditorOptions Create()
        {
            // 序列化
            try
            {
                string folder = AssemblyUtils.AssemblyDirectory;
                string path = $@"{folder}\{ConfigFileName}";

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {

                    BinaryFormatter formatter = new BinaryFormatter();
                    CamEditorOptions options = (CamEditorOptions) (formatter.Deserialize(fs));
                    return options;
                }
            }
            catch (Exception)
            {
                //ignore
            }

            return new CamEditorOptions();
        }

        public void Save(double master)
        {
            try
            {
                string folder = AssemblyUtils.AssemblyDirectory;
                string path = $@"{folder}\{ConfigFileName}";

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MasterVelocity = master;
                    formatter.Serialize(fs, this);
                }
            }
            catch (Exception)
            {
                //ignore
            }
        }

        protected CamEditorOptions(SerializationInfo info, StreamingContext context)
        {
            GridColor = OxyColor.FromUInt32((uint) info.GetValue(nameof(GridColor), typeof(uint)));
            Position = (AxisOptions) info.GetValue(nameof(Position), typeof(AxisOptions));
            Velocity = (AxisOptions) info.GetValue(nameof(Velocity), typeof(AxisOptions));
            Acceleration = (AxisOptions) info.GetValue(nameof(Acceleration), typeof(AxisOptions));
            Jerk = (AxisOptions) info.GetValue(nameof(Jerk), typeof(AxisOptions));
            EnableGrid = (bool) info.GetValue(nameof(EnableGrid), typeof(bool));
            MasterVelocity = 1;
        }

        private AxisOptions _position;

        public AxisOptions Position
        {
            get { return _position; }
            set
            {
                if (_position != null)
                {
                    _position.PropertyChanged -= OnAxisOptionsPropertyChanged;
                }

                _position = value;

                if (_position != null)
                {
                    _position.PropertyChanged += OnAxisOptionsPropertyChanged;
                }
            }
        }

        private AxisOptions _velocity;

        public AxisOptions Velocity
        {
            get { return _velocity; }
            set
            {
                if (_velocity != null)
                {
                    _velocity.PropertyChanged -= OnAxisOptionsPropertyChanged;
                }

                _velocity = value;

                if (_velocity != null)
                {
                    _velocity.PropertyChanged += OnAxisOptionsPropertyChanged;
                }

            }
        }

        private AxisOptions _acceleration;

        public AxisOptions Acceleration
        {
            get { return _acceleration; }
            set
            {
                if (_acceleration != null)
                {
                    _acceleration.PropertyChanged -= OnAxisOptionsPropertyChanged;
                }

                _acceleration = value;

                if (_acceleration != null)
                {
                    _acceleration.PropertyChanged += OnAxisOptionsPropertyChanged;
                }

            }
        }

        private AxisOptions _jerk;

        public AxisOptions Jerk
        {
            get { return _jerk; }
            set
            {
                if (_jerk != null)
                {
                    _jerk.PropertyChanged -= OnAxisOptionsPropertyChanged;
                }

                _jerk = value;

                if (_jerk != null)
                {
                    _jerk.PropertyChanged += OnAxisOptionsPropertyChanged;
                }

            }
        }

        private void OnAxisOptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch ((sender as AxisOptions).SlaveValue)
            {
                case "Position":
                    OnPropertyChanged(nameof(Position));
                    break;
                case "Velocity":
                    OnPropertyChanged(nameof(Velocity));
                    break;
                case "Acceleration":
                    OnPropertyChanged(nameof(Acceleration));
                    break;
                case "Jerk":
                    OnPropertyChanged(nameof(Jerk));
                    break;
            }
        }

        private bool _enableGrid;

        public bool EnableGrid
        {
            get { return _enableGrid; }
            set
            {
                if (_enableGrid != value)
                {
                    _enableGrid = value;
                    OnPropertyChanged(nameof(EnableGrid));
                }
            }
        }

        private OxyColor _gridColor;

        public OxyColor GridColor
        {
            get { return _gridColor; }
            set
            {
                if (_gridColor != value)
                {
                    _gridColor = value;
                    OnPropertyChanged(nameof(GridColor));
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(GridColor), GridColor.ToUint(), typeof(uint));
            info.AddValue(nameof(Position), Position, typeof(AxisOptions));
            info.AddValue(nameof(Velocity), Velocity, typeof(AxisOptions));
            info.AddValue(nameof(Acceleration), Acceleration, typeof(AxisOptions));
            info.AddValue(nameof(Jerk), Jerk, typeof(AxisOptions));
            info.AddValue(nameof(EnableGrid), EnableGrid, typeof(bool));
            info.AddValue(nameof(MasterVelocity), MasterVelocity, typeof(double));
        }

        public object Clone()
        {
            CamEditorOptions clone = new CamEditorOptions();
            clone.GridColor = GridColor;
            clone.Position = Position.Clone() as AxisOptions;
            clone.Velocity = Velocity.Clone() as AxisOptions;
            clone.Acceleration = Acceleration.Clone() as AxisOptions;
            clone.Jerk = Jerk.Clone() as AxisOptions;
            clone.EnableGrid = EnableGrid;
            clone.MasterVelocity = MasterVelocity;
            return clone;
        }
    }
}