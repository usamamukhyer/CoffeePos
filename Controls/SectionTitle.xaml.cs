namespace DBCafeteria.Controls;

public partial class SectionTitle : ContentView
{
    public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string), typeof(SectionTitle), string.Empty);
    public SectionTitle() => InitializeComponent();
    public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }
}
