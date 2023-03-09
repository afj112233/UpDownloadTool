using System.Windows;
using ICSStudio.QuickWatchPackage.View.Models;
using ICSStudio.Interfaces.Common;

namespace ICSStudio.QuickWatchPackage.View.UI
{
    internal class ValidationParam : DependencyObject
    {
        public static readonly DependencyProperty TagItemProperty =
            DependencyProperty.Register("TagItem", typeof(TagItem),
                typeof(ValidationParam), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ControllerProperty =
            DependencyProperty.Register("Controller", typeof(IController),
                typeof(ValidationParam), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty ScopeProperty =
            DependencyProperty.Register("Scope", typeof(ITagCollectionContainer),
                typeof(ValidationParam), new FrameworkPropertyMetadata(null));

        public static readonly DependencyProperty DataContentProperty = DependencyProperty.Register(
            "DataContent", typeof(MonitorTagsViewModel), typeof(ValidationParam), new PropertyMetadata(default(MonitorTagsViewModel)));

        public MonitorTagsViewModel DataContent
        {
            get { return (MonitorTagsViewModel) GetValue(DataContentProperty); }
            set { SetValue(DataContentProperty, value); }
        }

        public TagItem TagItem
        {
            get { return (TagItem) GetValue(TagItemProperty); }
            set { SetValue(TagItemProperty, value); }
        }

        public IController Controller
        {
            get { return (IController) GetValue(ControllerProperty); }
            set { SetValue(ControllerProperty, value); }
        }

        public ITagCollectionContainer Scope
        {
            get { return (ITagCollectionContainer) GetValue(ScopeProperty); }
            set { SetValue(ScopeProperty, value); }
        }
    }
}