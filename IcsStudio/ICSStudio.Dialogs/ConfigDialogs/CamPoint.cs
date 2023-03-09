using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ICSStudio.Gui.Annotations;
using ICSStudio.Utils.CamEditorUtil;

namespace ICSStudio.Dialogs.ConfigDialogs
{
    public class CamPoint : INotifyPropertyChanged
    {
        private int _status;
        private SegmentType _type;
        private float _master;
        private float _slave;
        private double _c0;
        private double _c1;
        private double _c2;
        private double _c3;

        public CamPoint()
        {
        }

        private CamPoint(CamPoint point)
        {
            _status = point._status;
            _type = point._type;
            _master = point._master;
            _slave = point._slave;
            _c0 = point._c0;
            _c1 = point._c1;
            _c2 = point._c2;
            _c3 = point._c3;
        }

        public void ChangedCamPoint(CamPoint newPoint)
        {
            _master = newPoint.Master;
            _slave = newPoint.Slave;
           _type = newPoint.Type;
           
            OnPropertyChanged(nameof(Master));
            OnPropertyChanged(nameof(Slave));
            OnPropertyChanged(nameof(Type));
        }

        public int Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                }
            }
        }

        public float Master
        {
            get { return _master; }
            set { Set(ref _master, value); }
        }

        public float Slave
        {
            get { return _slave; }
            set { Set(ref _slave,value); }
        }

        public SegmentType Type
        {
            get { return _type; }
            set { Set(ref _type, value); }
        }

        public double C0
        {
            get { return _c0; }
            set
            {
                if (_c0 != value)
                {
                    _c0 = value;
                }
            }
        }

        public double C1
        {
            get { return _c1; }
            set
            {
                if (_c1 != value)
                {
                    _c1 = value;
                }
            }
        }

        public double C2
        {
            get { return _c2; }
            set
            {
                if (_c2 != value)
                {
                    _c2 = value;
                }
            }
        }

        public double C3
        {
            get { return _c3; }
            set
            {
                if (_c3 != value)
                {
                    _c3 = value;
                }
            }
        }

        private bool _isBadCamPoint;

        public bool IsBadCamPoint
        {
            get
            {
                return _isBadCamPoint;
            }
            set
            {
                if (_isBadCamPoint != value)
                {
                    _isBadCamPoint = value;
                    OnPropertyChanged(nameof(IsBadCamPoint));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged(CamPoint oldPoint, CamPoint newPoint,
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new CamPointPropertyChangedEventArgs(oldPoint, newPoint, propertyName));
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T field, T newValue = default(T),
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue)) return;

            CamPoint oldPoint = new CamPoint(this);

            T oldValue = field;
            field = newValue;

            CamPoint newPoint = new CamPoint(this);

            OnPropertyChanged(oldPoint, newPoint, propertyName);
        }
    }

    public class CamPointPropertyChangedEventArgs : PropertyChangedEventArgs
    {
        public CamPointPropertyChangedEventArgs(CamPoint oldPoint, CamPoint newPoint, string propertyName) : base(
            propertyName)
        {
            OldPoint = oldPoint;
            NewPoint = newPoint;
        }

        public CamPoint OldPoint { get; }
        public CamPoint NewPoint { get; }
    }
}
