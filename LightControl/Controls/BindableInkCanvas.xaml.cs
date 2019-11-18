using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LightControl.Controls
{
    /// <summary>
    /// Interaktionslogik für BindableInkCanvas.xaml
    /// </summary>
    public partial class BindableInkCanvas : InkCanvas
    {
        #region ItemTemplate
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(BindableInkCanvas), new PropertyMetadata(new PropertyChangedCallback(OnItemTemplatePropertyChanged)));

        private static void OnItemTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => (d as BindableInkCanvas)?.ItemTemplateChanged(e);

        private void ItemTemplateChanged(DependencyPropertyChangedEventArgs e)
        {
            Children.Clear();
            if (ItemsSource != null)
            {
                foreach (var item in ItemsSource)
                    AddItemToCanvas(item);
            }
        }
        #endregion

        #region ItemsSource
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(BindableInkCanvas), new PropertyMetadata(new PropertyChangedCallback(OnItemsSourcePropertyChanged)));

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
            => (sender as BindableInkCanvas)?.OnItemsSourcePropertyChanged(e);

        private void OnItemsSourcePropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            Children.Clear();

            if (e.OldValue is INotifyCollectionChanged oldINotify)
                oldINotify.CollectionChanged -= ItemsSource_CollectionChanged;

            if (e.NewValue is INotifyCollectionChanged newINotify)
                newINotify.CollectionChanged += ItemsSource_CollectionChanged;

            foreach (var item in e.NewValue as IEnumerable)
                AddItemToCanvas(item);
        }

        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ItemTemplate == null)
                return;

            foreach (var item in e.NewItems)
                AddItemToCanvas(item);

            foreach (var item in e.OldItems)
                foreach (var child in Children)
                    if (child is FrameworkElement fe && fe.DataContext == item)
                        Children.Remove(fe);
        }
        #endregion



        public ICommand SelectionCommand
        {
            get { return (ICommand)GetValue(SelectionCommandProperty); }
            set { SetValue(SelectionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SetSelection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectionCommandProperty =
            DependencyProperty.Register("SelectionCommand", typeof(ICommand), typeof(BindableInkCanvas));



        public ICommand RightClickCommand
        {
            get { return (ICommand)GetValue(RightClickCommandProperty); }
            set { SetValue(RightClickCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RightClickCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RightClickCommandProperty =
            DependencyProperty.Register("RightClickCommand", typeof(ICommand), typeof(BindableInkCanvas));

        private void AddItemToCanvas(object item)
        {
            if (ItemTemplate?.LoadContent() is FrameworkElement fe)
            {
                fe.DataContext = item;
                Children.Add(fe);
            }
        }

        public BindableInkCanvas()
        {
            InitializeComponent();
        }

        bool fireCommand = true;
        private void InkCanvas_SelectionChanged(object sender, EventArgs e)
        {
            if (fireCommand)
            {
                IEnumerable<object> items = GetSelectedElements()
                    .Select(i => (i is FrameworkElement fe) ? fe.DataContext : null)
                    .Where(i => i != null);
                if (SelectionCommand != null && SelectionCommand.CanExecute(items))
                    SelectionCommand.Execute(items);
                fireCommand = false;
                Select(Array.Empty<UIElement>());
            }
            else
            {
                fireCommand = true;
            }
        }


        private object rightClickTarget;
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (e != null && e.ChangedButton == MouseButton.Right && RightClickCommand != null)
            {
                var pos = e.GetPosition(this);
                rightClickTarget = null;
                VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(GridHitTestResult), new PointHitTestParameters(pos));
                if (rightClickTarget != null && RightClickCommand.CanExecute(rightClickTarget))
                    RightClickCommand.Execute(rightClickTarget);
            }
            base.OnMouseUp(e);
        }

        public HitTestResultBehavior GridHitTestResult(HitTestResult result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (result.VisualHit is Panel panel)
            {
                rightClickTarget = panel.DataContext;
                return HitTestResultBehavior.Stop;
            }
            return HitTestResultBehavior.Continue;
        }
    }
}
