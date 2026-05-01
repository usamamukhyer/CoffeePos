using System.Windows.Input;

namespace DBCafeteria.Controls;

public partial class SelectionCard : ContentView
{
    public static readonly BindableProperty TitleProperty = BindableProperty.Create(nameof(Title), typeof(string), typeof(SelectionCard), string.Empty);
    public static readonly BindableProperty SubtitleProperty = BindableProperty.Create(nameof(Subtitle), typeof(string), typeof(SelectionCard), string.Empty);
    public static readonly BindableProperty IconProperty = BindableProperty.Create(nameof(Icon), typeof(string), typeof(SelectionCard), string.Empty, propertyChanged: OnIconChanged);
    public static readonly BindableProperty IconSourceProperty = BindableProperty.Create(nameof(IconSource), typeof(ImageSource), typeof(SelectionCard), null, propertyChanged: OnIconSourceChanged);
    public static readonly BindableProperty HasIconSourceProperty = BindableProperty.Create(nameof(HasIconSource), typeof(bool), typeof(SelectionCard), false);
    public static readonly BindableProperty HasTextIconProperty = BindableProperty.Create(nameof(HasTextIcon), typeof(bool), typeof(SelectionCard), false);
    public static readonly BindableProperty IsSelectedProperty = BindableProperty.Create(nameof(IsSelected), typeof(bool), typeof(SelectionCard), false);
    public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(SelectionCard));
    public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(SelectionCard));

    public SelectionCard() => InitializeComponent();

    public string Title { get => (string)GetValue(TitleProperty); set => SetValue(TitleProperty, value); }
    public string Subtitle { get => (string)GetValue(SubtitleProperty); set => SetValue(SubtitleProperty, value); }
    public string Icon { get => (string)GetValue(IconProperty); set => SetValue(IconProperty, value); }
    public ImageSource? IconSource { get => (ImageSource?)GetValue(IconSourceProperty); set => SetValue(IconSourceProperty, value); }
    public bool HasIconSource { get => (bool)GetValue(HasIconSourceProperty); private set => SetValue(HasIconSourceProperty, value); }
    public bool HasTextIcon { get => (bool)GetValue(HasTextIconProperty); private set => SetValue(HasTextIconProperty, value); }
    public bool IsSelected { get => (bool)GetValue(IsSelectedProperty); set => SetValue(IsSelectedProperty, value); }
    public ICommand? Command { get => (ICommand?)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
    public object? CommandParameter { get => GetValue(CommandParameterProperty); set => SetValue(CommandParameterProperty, value); }

    private static void OnIconSourceChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SelectionCard)bindable).UpdateIconState();
    }

    private static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        ((SelectionCard)bindable).UpdateIconState();
    }

    private void UpdateIconState()
    {
        HasIconSource = IconSource is not null;
        HasTextIcon = IconSource is null && !string.IsNullOrWhiteSpace(Icon);
    }
}
