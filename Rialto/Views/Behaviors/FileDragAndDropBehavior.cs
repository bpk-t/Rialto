using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Rialto.Views.Behaviors
{
    public class FileDragAndDropBehavior : Behavior<FrameworkElement>
    {
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }

        public static readonly DependencyProperty CommandProperty
            = DependencyProperty.Register("Command", typeof(ICommand), typeof(FileDragAndDropBehavior), new PropertyMetadata(null));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.AllowDrop = true;
            this.AssociatedObject.PreviewDragOver += PreviewDragOver;
            this.AssociatedObject.Drop += Drop;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.PreviewDragOver -= PreviewDragOver;
            this.AssociatedObject.Drop -= Drop;
        }

        private void PreviewDragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent("UniformResourceLocator"))
            {
                e.Effects = DragDropEffects.Link;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = true;
        }

        private void Drop(object sender, DragEventArgs e)
        {
            if (Command == null || !Command.CanExecute(e)) return;

            IList<Uri> uriList = null;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                uriList = (e.Data.GetData(DataFormats.FileDrop) as string[]).Select(x => new Uri(x)).ToList();
            }
            else
            {
                uriList = new Uri[] { new Uri(e.Data.GetData(DataFormats.Text).ToString()) };
            }
            Command.Execute(uriList);
        }
    }
}
