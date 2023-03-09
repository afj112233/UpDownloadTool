using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using ICSStudio.Interfaces.Common;
using ICSStudio.SimpleServices.Common;
using Microsoft.VisualStudio.Shell;

namespace ICSStudio.EditorPackage.Reference.Extend
{
    internal class ScopeTreeViewVM
    {
        private readonly IController _controller;

        public ScopeTreeViewVM(SelectionTextType type, IController controller)
        {
            _controller = controller;
            SetScopeList(type);
        }
        
        private void SetScopeList(SelectionTextType type)
        {
            if (type == SelectionTextType.Scope || type == SelectionTextType.ScopeExceptAoi)
            {
                var l36 = new ScopeItem(true) {Name = _controller.Name, Index = 1, CanGetValue = true};
                var program = new ScopeItem() {Name = "Program"};
                {
                    foreach (var controllerProgram in _controller.Programs)
                    {
                        var p = new ScopeItem() {Name = controllerProgram.Name, CanGetValue = true};
                        program.Children.Add(p);
                    }
                }
                //TODO(zyl):add EP and ES
                var ep = new ScopeItem() {Name = "Equipment Phases"};
                var es = new ScopeItem() {Name = "Equipment Sequences"};


                Scopes.Add(l36);
                Scopes.Add(program);
                Scopes.Add(ep);
                Scopes.Add(es);
                if (type != SelectionTextType.ScopeExceptAoi)
                {
                    var aoi = new ScopeItem() {Name = "UD Function Block" };
                    {
                        foreach (var aoiDefinition in _controller.AOIDefinitionCollection)
                        {
                            var item = new ScopeItem() {Name = aoiDefinition.Name, CanGetValue = true};
                            aoi.Children.Add(item);
                        }
                    }
                    Scopes.Add(aoi);
                }
            }
            else if (type == SelectionTextType.Label)
            {
                int i = 1;
                foreach (var program in _controller.Programs)
                {
                    var p = new ScopeItem() {Name = program.Name, Index = i};
                    i++;
                    foreach (var routine in program.Routines)
                    {
                        if (routine is RLLRoutine)
                        {
                            var rll = new ScopeItem() {Name = routine.Name, CanGetValue = true};
                            p.Children.Add(rll);
                        }
                    }

                    Scopes.Add(p);
                }

                foreach (var aoi in _controller.AOIDefinitionCollection)
                {
                    var a = new ScopeItem() {Name = aoi.Name, Index = i};
                    i++;
                    foreach (var routine in aoi.Routines)
                    {
                        if (routine is RLLRoutine)
                        {
                            var rll = new ScopeItem() {Name = routine.Name, CanGetValue = true};
                            a.Children.Add(rll);
                        }
                    }

                    Scopes.Add(a);
                }
            }
            else
            {
                var showAll = new ScopeItem() {Name = "Show All", Index = 1, CanGetValue = true};
                var destructive = new ScopeItem() {Name = "Destructive", CanGetValue = true};
                var nonDestruction = new ScopeItem() {Name = "Non-destruction", CanGetValue = true};
                //TODO(zyl):add reference ,contain,routine and element
                var reference = new ScopeItem() {Name = "Reference"};
                {
                    var t1 = new ScopeItem() {Name = "test1", CanGetValue = true};
                    reference.Children.Add(t1);
                }
                var contain = new ScopeItem() {Name = "Contain"};
                {
                    var t2 = new ScopeItem() {Name = "test2", CanGetValue = true};
                    contain.Children.Add(t2);
                }
                ;
                var routine = new ScopeItem() {Name = "Routine"};
                var element = new ScopeItem() {Name = "Element"};

                Scopes.Add(showAll);
                Scopes.Add(destructive);
                Scopes.Add(nonDestruction);
                Scopes.Add(reference);
                Scopes.Add(contain);
                Scopes.Add(routine);
                Scopes.Add(element);
            }

        }

        public List<ScopeItem> Scopes { set; get; } = new List<ScopeItem>();

        public void Update(SelectionTextType type)
        {
            SetScopeList(type);
        }
    }

    internal class ScopeItem
    {
        public ScopeItem(bool isController=false)
        {
            IsController = isController;
        }

        public string Name { set; get; }
        public int Index { set; get; } = 2;
        public bool CanGetValue { set; get; } = false;
        public List<ScopeItem> Children { set; get; } = new List<ScopeItem>();

        public bool IsController { get; }
    }

}
