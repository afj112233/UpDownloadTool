using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using ICSStudio.Diagrams;
using ICSStudio.Diagrams.Controls;
using ICSStudio.SimpleServices.Chart;

namespace ICSStudio.Diagram.Flowchart
{
	public class FlowNode: INotifyPropertyChanged
	{
		[Browsable(false)]

        public int ID { get; set; }
		public NodeKinds Kind { get; private set; }

		//private int _column;
		//public int Column
		//{
		//	get { return _column; }
		//	set 
		//	{ 
		//		_column = value;
		//		OnPropertyChanged("Column");
		//	}
		//}

		//private int _row;
		//public int Row
		//{
		//	get { return _row; }
		//	set 
		//	{ 
		//		_row = value;
		//		OnPropertyChanged("Row");
		//	}
		//}

		private string _text;
        private double _x;
        private double _y;
        private DiagramView _view;
        private IContent _content;

        public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged("Text");
			}
		}

        public IContent Content
        {
            set { _content = value; }
            get { return _content; }
        }

        public FlowNode(NodeKinds kind)
		{
			Kind = kind;
		}

        public DiagramView View => _view;

        public double X
        {
            set { _x = value; OnPropertyChanged("X");}
            get { return _x; }
        }

        public double Y
        {
            set { _y = value;OnPropertyChanged("Y"); }
            get { return _y; }
        }

        public IEnumerable<PortKinds> GetPorts()
		{
			switch(Kind)
			{
                case NodeKinds.Transition:
                case NodeKinds.Step:
                    yield return PortKinds.Bottom;
                    yield return PortKinds.Top;
                    break;
                case NodeKinds.Stop:
                    yield return PortKinds.Top;
                    break;
			}
		}

        public double Offset
        {
            get
            {
                switch (Kind)
                {
                    case NodeKinds.Transition:
                        return 40;
                    case NodeKinds.Step:
                        return 40;
                    case NodeKinds.Stop:
                        return 20;
                    default:
                        return 0;
                }
            }
        }

        public void SetDiagramView(DiagramView view)
        {
            if (View == null) _view = view;
        }

        public double FindPortX()
        {
            double x = 0;
            x = X + Offset;
            return x;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged(string name)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(name));
		}

		#endregion
	}

	public enum NodeKinds { Transition,Step,Stop,Branch,StepAndTransition, SimultaneousBranchDiverge,SelectionBranchDiverge,SubroutineOrReturn,TextBox}
}
