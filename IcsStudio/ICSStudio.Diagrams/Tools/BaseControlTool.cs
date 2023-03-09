using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace ICSStudio.Diagrams.Tools
{
    class BaseControlTool
    {
        protected DiagramView View { get; private set; }
        public BaseControlTool(DiagramView view)
        {
            View = view;
            ZoomOutCommand=new RelayCommand(ZoomOut);
            ZoomInCommand=new RelayCommand(ZoomIn);
        }

        #region Zoom

        public RelayCommand ZoomInCommand {  get; }
        public RelayCommand ZoomOutCommand { get; }
        private void ZoomIn()
        {
            var scale = View.Zoom;
            scale = scale + 0.1;
            scale = Math.Min(1.5, scale);
            View.Zoom = scale;
        }

        private void ZoomOut()
        {
            var scale = View.Zoom;
            scale = scale - 0.1;
            scale = Math.Max(0.5, scale);
            View.Zoom = scale;
        }

        #endregion
    }
}
