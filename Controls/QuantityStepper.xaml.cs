using System.Windows.Input;

namespace DBCafeteria.Controls;

public partial class QuantityStepper : ContentView
{
    public static readonly BindableProperty QuantityProperty = BindableProperty.Create(nameof(Quantity), typeof(int), typeof(QuantityStepper), 0);
    public static readonly BindableProperty IncrementCommandProperty = BindableProperty.Create(nameof(IncrementCommand), typeof(ICommand), typeof(QuantityStepper));
    public static readonly BindableProperty DecrementCommandProperty = BindableProperty.Create(nameof(DecrementCommand), typeof(ICommand), typeof(QuantityStepper));
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(QuantityStepper));

    public QuantityStepper() => InitializeComponent();
    public int Quantity { get => (int)GetValue(QuantityProperty); set => SetValue(QuantityProperty, value); }
    public ICommand? IncrementCommand { get => (ICommand?)GetValue(IncrementCommandProperty); set => SetValue(IncrementCommandProperty, value); }
    public ICommand? DecrementCommand { get => (ICommand?)GetValue(DecrementCommandProperty); set => SetValue(DecrementCommandProperty, value); }
    public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }
}
