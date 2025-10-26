using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Windows.Shapes;

namespace mywpf.Controls
{
    public class ColorPicker : ComboBox
    {
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
                new FrameworkPropertyMetadata(Colors.Black, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public Color SelectedColor
        {
            get => (Color)GetValue(SelectedColorProperty);
            set => SetValue(SelectedColorProperty, value);
        }

        public ColorPicker()
        {
            // Стандартные цвета
            Items.Add(Colors.Black);
            Items.Add(Colors.White);
            Items.Add(Colors.Red);
            Items.Add(Colors.Green);
            Items.Add(Colors.Blue);
            Items.Add(Colors.Yellow);

            // Создаем DataTemplate правильно
            var factory = new FrameworkElementFactory(typeof(StackPanel));
            factory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);

            var rectFactory = new FrameworkElementFactory(typeof(Rectangle));
            rectFactory.SetValue(Rectangle.WidthProperty, 20.0);
            rectFactory.SetValue(Rectangle.HeightProperty, 20.0);
            rectFactory.SetValue(Rectangle.MarginProperty, new Thickness(0, 0, 5, 0));
            rectFactory.SetBinding(Rectangle.FillProperty, new Binding());

            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, new Binding());

            factory.AppendChild(rectFactory);
            factory.AppendChild(textFactory);

            ItemTemplate = new DataTemplate
            {
                VisualTree = factory
            };

            // Привязка SelectedItem к SelectedColor
            SetBinding(SelectedItemProperty, new Binding("SelectedColor")
            {
                Source = this,
                Mode = BindingMode.TwoWay
            });
        }
    }
}