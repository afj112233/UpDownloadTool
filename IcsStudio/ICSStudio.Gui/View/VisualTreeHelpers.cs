using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ICSStudio.Gui.View
{
    public static class VisualTreeHelpers
    {
        public static T FindVisualParentOfType<T>(DependencyObject visual)
            where T : class
        {
            if (visual == null)
                throw new ArgumentNullException(nameof(visual));

            T obj = visual as T;
            if (obj != null)
                return obj;

            T obj1 = default(T);
            for (visual = VisualTreeHelper.GetParent(visual);
                visual != null && (object) obj1 == null;
                visual = VisualTreeHelper.GetParent(visual))
                obj1 = visual as T;
            return obj1;
        }

        public static FrameworkElement FindVisualParentByName(
            DependencyObject visual, string name)
        {
            if (visual == null)
                throw new ArgumentNullException(nameof(visual));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The name must not be null or empty", nameof(name));
            for (; visual != null; visual = VisualTreeHelper.GetParent(visual))
            {
                FrameworkElement frameworkElement = visual as FrameworkElement;

                if (frameworkElement != null && frameworkElement.Name == name)
                    return frameworkElement;
            }

            return null;
        }

        public static DependencyObject FindVisualRoot(DependencyObject visual)
        {
            if (visual == null)
                throw new ArgumentNullException(nameof(visual));
            for (DependencyObject parent = VisualTreeHelper.GetParent(visual);
                parent != null;
                parent = VisualTreeHelper.GetParent(visual))
                visual = parent;
            return visual;
        }

        public static bool IsObjectChildOf(DependencyObject child, DependencyObject parent)
        {
            if (child == null || parent == null)
                throw new ArgumentNullException();
            for (DependencyObject reference = child;
                reference != null;
                reference = VisualTreeHelper.GetParent(reference))
            {
                if (reference == parent)
                    return true;
            }

            return false;
        }

        public static T FindFirstVisualChildOfType<T>(
            DependencyObject visual)
            where T : class
        {
            Stack<DependencyObject> dependencyObjectStack = new Stack<DependencyObject>();
            dependencyObjectStack.Push(visual);

            T obj = default(T);
            while (dependencyObjectStack.Count > 0)
            {
                DependencyObject reference = dependencyObjectStack.Pop();

                T temp = reference as T;
                if (temp == null)
                {
                    int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
                    for (int childIndex = 0; childIndex < childrenCount; ++childIndex)
                        dependencyObjectStack.Push(VisualTreeHelper.GetChild(reference, childIndex));
                }
                else
                {
                    obj = temp;
                    break;
                }

            }

            return obj;
        }

        public static FrameworkElement GetChildObject(DependencyObject obj, string name)
        {
            if (obj == null)
                return null;
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i) as FrameworkElement;
                if (child == null) continue;
                if (child.Name == name)
                {
                    return child;
                }
                else
                {
                    var grandChild = GetChildObject(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;
        }

        public static T GetChildObject<T>(DependencyObject obj, string name) where T : FrameworkElement
        {
            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child is T && (((T) child).Name == name || string.IsNullOrEmpty(name)))
                {
                    return (T) child;
                }
                else
                {
                    var grandChild = GetChildObject<T>(child, name);
                    if (grandChild != null)
                        return grandChild;
                }
            }

            return null;
        }
    }
}
