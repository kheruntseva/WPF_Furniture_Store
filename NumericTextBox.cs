using System;
using System.Windows;
using System.Windows.Controls;

namespace mywpf.Controls
{
    public class NumericTextBox : TextBox
    {
        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register(
                "MaxValue",
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(double.MaxValue,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,
                    new CoerceValueCallback(CoerceMaxValue)));

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register(
                "MinValue",
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(double.MinValue,
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    null,
                    new CoerceValueCallback(CoerceMinValue)));

        public static readonly DependencyProperty NumericValueProperty =
            DependencyProperty.Register(
                "NumericValue",
                typeof(double),
                typeof(NumericTextBox),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    new PropertyChangedCallback(OnNumericValueChanged),
                    new CoerceValueCallback(CoerceNumericValue)),
                new ValidateValueCallback(IsValidDouble));

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public double NumericValue
        {
            get => (double)GetValue(NumericValueProperty);
            set => SetValue(NumericValueProperty, value);
        }

        public NumericTextBox()
        {
            TextChanged += OnTextChanged;
        }

        private static bool IsValidDouble(object value)
        {
            return value is double;
        }

        private static object CoerceNumericValue(DependencyObject d, object value)
        {
            var control = (NumericTextBox)d;
            double val = (double)value;

            if (val < control.MinValue) return control.MinValue;
            if (val > control.MaxValue) return control.MaxValue;
            return val;
        }

        private static object CoerceMaxValue(DependencyObject d, object value)
        {
            var control = (NumericTextBox)d;
            double max = (double)value;
            return max < control.MinValue ? control.MinValue : max;
        }

        private static object CoerceMinValue(DependencyObject d, object value)
        {
            var control = (NumericTextBox)d;
            double min = (double)value;
            return min > control.MaxValue ? control.MaxValue : min;
        }

        private static void OnNumericValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericTextBox)d;
            control.Text = e.NewValue.ToString();
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(Text, out double result))
            {
                NumericValue = result;
            }
            else if (!string.IsNullOrEmpty(Text))
            {
                Text = NumericValue.ToString();
            }
        }
    }
}