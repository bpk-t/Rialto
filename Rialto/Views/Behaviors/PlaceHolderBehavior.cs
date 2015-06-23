using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace Rialto.Views.Behaviors
{
    /// <summary>
    /// TextBoxにプレースホルダーを表示するビヘイビア
    /// </summary>
    public class PlaceHolderBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty PlaceHolderTextProperty = DependencyProperty.Register(
            "PlaceHolderText",
            typeof(string),
            typeof(PlaceHolderBehavior),
            new UIPropertyMetadata(null));

        public string PlaceHolderText
        {
            get { return (string)GetValue(PlaceHolderTextProperty); }
            set { SetValue(PlaceHolderTextProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Background = CreateVisualBrush();
            this.AssociatedObject.TextChanged += OnTextChanged;
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= OnTextChanged;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            if (string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Background = CreateVisualBrush();
            }
            else
            {
                textBox.Background = new SolidColorBrush(Colors.Transparent);
            }
        }
        
        private VisualBrush CreateVisualBrush()
        {
            var visual = new Label()
            {
                Content = PlaceHolderText,
                Padding = new Thickness(5, 1, 1, 1),
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center,
            };
            return new VisualBrush(visual)
            {
                Stretch = Stretch.None,
                TileMode = TileMode.None,
                AlignmentX = AlignmentX.Left,
                AlignmentY = AlignmentY.Center,
            };
        }
    }
}
