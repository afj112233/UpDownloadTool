using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Imagin.Common.Extensions;
using Imagin.Common.Input;
using Imagin.Controls.Extended;

namespace ICSStudio.Gui.Controls.ExtendedPropertyGrid
{
    /// <summary>
    /// ExtendedPropertyGrid.xaml
    /// </summary>
    public partial class ExtendedPropertyGrid
    {
        public ExtendedPropertyGrid()
        {
            CommandBindings.Add(new CommandBinding(EditCollectionCommand, 
                EditCollectionCommand_Executed, EditCollectionCommand_CanExecute));
            CommandBindings.Add(new CommandBinding(ResetCommand, ResetCommand_Executed, ResetCommand_CanExecute));

            Properties = new ExtendedPropertyModelCollection();
            ListCollectionView = new ListCollectionView(Properties);

            PropertyColumnWidth = new DataGridLength(5d, DataGridLengthUnitType.Star);
            ValueColumnWidth = new DataGridLength(4d, DataGridLengthUnitType.Star);
            UnitColumnWidth = new DataGridLength(3d, DataGridLengthUnitType.Star);

            InitializeComponent();
        }

        #region Properties

        public event EventHandler<EventArgs<object>> SelectedObjectChanged;

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty SelectedObjectProperty =
            DependencyProperty.Register("SelectedObject", typeof(object), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedObjectChanged));
        /// <summary>
        /// 
        /// </summary>
        public object SelectedObject
        {
            get
            {
                return GetValue(SelectedObjectProperty);
            }
            set
            {
                SetValue(SelectedObjectProperty, value);
            }
        }
        static async void OnSelectedObjectChanged(DependencyObject Object, DependencyPropertyChangedEventArgs e)
        {
            await Object.As<ExtendedPropertyGrid>().OnSelectedObjectChanged(e.NewValue);
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty PropertiesProperty = 
            DependencyProperty.Register("Properties", typeof(ExtendedPropertyModelCollection), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public ExtendedPropertyModelCollection Properties
        {
            get
            {
                return (ExtendedPropertyModelCollection)GetValue(PropertiesProperty);
            }
            set
            {
                SetValue(PropertiesProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ListCollectionViewProperty = 
            DependencyProperty.Register("ListCollectionView", typeof(ListCollectionView), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public ListCollectionView ListCollectionView
        {
            get
            {
                return (ListCollectionView)GetValue(ListCollectionViewProperty);
            }
            set
            {
                SetValue(ListCollectionViewProperty, value);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty PropertyColumnWidthProperty = DependencyProperty.Register("PropertyColumnWidth", typeof(DataGridLength), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(default(DataGridLength), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public DataGridLength PropertyColumnWidth
        {
            get
            {
                return (DataGridLength)GetValue(PropertyColumnWidthProperty);
            }
            set
            {
                SetValue(PropertyColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty ValueColumnWidthProperty = DependencyProperty.Register("ValueColumnWidth", typeof(DataGridLength), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(default(DataGridLength), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public DataGridLength ValueColumnWidth
        {
            get
            {
                return (DataGridLength)GetValue(ValueColumnWidthProperty);
            }
            set
            {
                SetValue(ValueColumnWidthProperty, value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static DependencyProperty UnitColumnWidthProperty = DependencyProperty.Register("UnitColumnWidth", typeof(DataGridLength), typeof(ExtendedPropertyGrid), new FrameworkPropertyMetadata(default(DataGridLength), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        /// <summary>
        /// 
        /// </summary>
        public DataGridLength UnitColumnWidth
        {
            get
            {
                return (DataGridLength)GetValue(UnitColumnWidthProperty);
            }
            set
            {
                SetValue(UnitColumnWidthProperty, value);
            }
        }

        #endregion

        #region  Virtual

        protected virtual async Task OnSelectedObjectChanged(object value)
        {
            if (value != null)
            {
                Properties.Reset(value);

                //IsLoading = true;
                await SetObject(value);
                //IsLoading = false;

                // sorted, added by gjc
                ListCollectionView.SortDescriptions.Clear();
                ListCollectionView.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
                //end add
            }
            //else if (AcceptsNullObjects)
            //    Properties.Reset(null);

            SelectedObjectChanged?.Invoke(this, new EventArgs<object>(value));
        }

        protected virtual async Task SetObject(object value)
        {
            //await Properties.BeginFromObject(value);
            await Properties.BeginFromObjectExtended(value);

        }

        #endregion

        #region Commands

        public static readonly RoutedUICommand EditCollectionCommand = new RoutedUICommand("EditCollectionCommand", "EditCollectionCommand", typeof(ExtendedPropertyGrid));
        void EditCollectionCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            var window = new Window()
            {
                Content = new CollectionEditor()
                {
                    Collection = e.Parameter
                },
                Height = 425,
                Title = "Edit Collection",
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 720
            };
            window.ShowDialog();
        }
        void EditCollectionCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = e.Parameter != null;
        }

        public static readonly RoutedUICommand ResetCommand = new RoutedUICommand("ResetCommand", "ResetCommand", typeof(ExtendedPropertyGrid));
        void ResetCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            foreach (var i in Properties)
                i.Value = null;
        }
        void ResetCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Properties != null && Properties.Count > 0;
        }


        #endregion


        #region Methond

        public void Refresh()
        {
            if (SelectedObject != null)
            {
                Properties?.Refresh();
            }
        }

        public async void Reload()
        {
            if (SelectedObject != null && Properties != null)
            {
                Properties.Reset(SelectedObject);
                await Properties.BeginFromObjectExtended(SelectedObject);
            }
        }

        #endregion
    }
}
