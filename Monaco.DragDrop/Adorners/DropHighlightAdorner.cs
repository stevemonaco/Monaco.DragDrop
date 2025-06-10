using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using Monaco.DragDrop.Adorners;

namespace Monaco.DragDrop;
public class DropHighlightAdorner : DropAdornerBase
{
    private readonly DropAdornerKind _dropAdornerType;

    static DropHighlightAdorner()
    {
        OpacityProperty.OverrideDefaultValue<DropHighlightAdorner>(0.7d);
        BackgroundProperty.OverrideDefaultValue<DropHighlightAdorner>(Brushes.Green);
        BorderBrushProperty.OverrideDefaultValue<DropHighlightAdorner>(Brushes.Purple);
        BorderThicknessProperty.OverrideDefaultValue<DropHighlightAdorner>(new Thickness(2));
    }

    public DropHighlightAdorner() : this(DropAdornerKind.Solid)
    {
    }
    
    public DropHighlightAdorner(DropAdornerKind dropAdornerType)
    {
        _dropAdornerType = dropAdornerType;
    }

    public override void Attach()
    {
        if (TargetControl is null)
            return;

        base.Attach();

        var rect = GetAdornerRect();

        Width = rect.Width;
        Height = rect.Height;
        RenderTransform = new TranslateTransform(rect.X, rect.Y);
        
        switch (_dropAdornerType)
        {
            case DropAdornerKind.Solid:
                Background = Brushes.Green;
                BorderBrush = Brushes.DarkGreen;
                break;
            case DropAdornerKind.Border:
                Background = Brushes.Transparent;
                BorderBrush = Brushes.Green;
                break;
            case DropAdornerKind.None:
            default:
                Background = Brushes.Transparent;
                BorderBrush = Brushes.Transparent;
                break;
        }

        Child = new TextBlock()
        {
            //[!TextBlock.TextProperty] = new Binding()
            //{
            //    Source = this,
            //    Path = nameof(ErrorMessage)
            //},
            Text = ErrorMessage,
            FontSize = 24,
            Foreground = Brushes.White,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextWrapping = TextWrapping.Wrap
        };
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ErrorMessageProperty
            && change.NewValue is string errorMessage
            && Child is TextBlock errorText)
        {
            errorText.Text = errorMessage;
        }
    }
}
