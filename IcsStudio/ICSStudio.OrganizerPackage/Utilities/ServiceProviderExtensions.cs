using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using ICSStudio.Interfaces.Common;
using ICSStudio.UIInterfaces.Project;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace ICSStudio.OrganizerPackage.Utilities
{
    public static class ServiceProviderExtensions
    {
        public static IController GetController(this IServiceProvider serviceProvider)
        {
            var projectInfoService =
                serviceProvider?.GetService(typeof(SProjectInfoService)) as IProjectInfoService;
            return projectInfoService?.Controller;
        }

        public static IProjectItem GetSelectedProjectItem(this IServiceProvider serviceProvider)
        {
            IProjectItem selectedProjectItem = null;

            ThreadHelper.ThrowIfNotOnUIThread();
            var monitorSelection =
                serviceProvider?.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

            if (monitorSelection != null)
            {
                IntPtr hierarchyPointer;
                IntPtr selectionContainerPointer;
                IVsMultiItemSelect multiItemSelect;
                uint projectItemId;

                monitorSelection.GetCurrentSelection(out hierarchyPointer,
                    out projectItemId, out multiItemSelect, out selectionContainerPointer);

                if (selectionContainerPointer != IntPtr.Zero)
                {
                    var selectionContainer =
                        Marshal.GetTypedObjectForIUnknown(selectionContainerPointer, typeof(ISelectionContainer)) as
                            ISelectionContainer;

                    if (selectionContainer != null)
                    {
                        uint count;
                        selectionContainer.CountObjects((uint)Constants.GETOBJS_SELECTED,
                            out count);
                        if (count > 0)
                        {
                            var objects = new object[count];
                            selectionContainer.GetObjects((uint)Constants.GETOBJS_SELECTED, count, objects);
                            selectedProjectItem = objects[0] as IProjectItem;
                        }
                    }

                    Marshal.Release(selectionContainerPointer);
                }

                if (hierarchyPointer != IntPtr.Zero) Marshal.Release(hierarchyPointer);
            }

            return selectedProjectItem;
        }

        public static List<IProjectItem> GetSelectedProjectItems(this IServiceProvider serviceProvider)
        {
            List<IProjectItem> selectedProjectItems = new List<IProjectItem>();

            ThreadHelper.ThrowIfNotOnUIThread();
            var monitorSelection =
                serviceProvider?.GetService(typeof(SVsShellMonitorSelection)) as IVsMonitorSelection;

            if (monitorSelection != null)
            {
                IntPtr hierarchyPointer;
                IntPtr selectionContainerPointer;
                IVsMultiItemSelect multiItemSelect;
                uint projectItemId;

                monitorSelection.GetCurrentSelection(out hierarchyPointer,
                    out projectItemId, out multiItemSelect, out selectionContainerPointer);

                if (selectionContainerPointer != IntPtr.Zero)
                {
                    var selectionContainer =
                        Marshal.GetTypedObjectForIUnknown(selectionContainerPointer, typeof(ISelectionContainer)) as
                            ISelectionContainer;

                    if (selectionContainer != null)
                    {
                        uint count;
                        selectionContainer.CountObjects((uint)Constants.GETOBJS_SELECTED,
                            out count);
                        if (count > 0)
                        {
                            var objects = new object[count];
                            selectionContainer.GetObjects((uint)Constants.GETOBJS_SELECTED, count, objects);
                            selectedProjectItems.AddRange(objects.Select(p => p as IProjectItem)); //= objects[0] as IProjectItem;
                        }
                    }

                    Marshal.Release(selectionContainerPointer);
                }

                if (hierarchyPointer != IntPtr.Zero) Marshal.Release(hierarchyPointer);
            }

            return selectedProjectItems;
        }
    }
}