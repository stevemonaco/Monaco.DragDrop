using Avalonia.Controls;
using Avalonia.Media;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using AvaDragDrop = Avalonia.Input.DragDrop;
using System.Diagnostics;

namespace Monaco.DragDrop;

/// <summary>
/// Adorner that displays an insertion separator between child items in a collection control
/// </summary>
public class DropInsertionAdorner : DropAdornerBase
{
    static DropInsertionAdorner()
    {
        OpacityProperty.OverrideDefaultValue<DropInsertionAdorner>(0.7d);
        BackgroundProperty.OverrideDefaultValue<DropInsertionAdorner>(Brushes.Green);
        BorderBrushProperty.OverrideDefaultValue<DropInsertionAdorner>(Brushes.Purple);
        BorderThicknessProperty.OverrideDefaultValue<DropInsertionAdorner>(new Thickness(2));
    }

    public DropInsertionAdorner()
    {
        IsHitTestVisible = false;
        AdornerLayer.SetIsClipEnabled(this, false);
        ClipToBounds = false;
    }

    public override void Attach()
    {
        if (TargetControl is null)
            return;

        base.Attach();
        TargetControl.AddHandler(AvaDragDrop.DragOverEvent, TargetControl_DragOver, RoutingStrategies.Bubble, true);
        TargetControl.AddHandler(AvaDragDrop.DragEnterEvent, TargetControl_DragEnter, RoutingStrategies.Bubble, true);

        var rect = GetAdornerRect();

        Width = rect.Width;
        Height = 4;

        Debug.WriteLine($"Attach: {TargetControl.GetType().ToString()}");
        //IsVisible = false;

        //TargetControl.RenderTransform = new TranslateTransform(0, 40);
        //RenderTransform = new TranslateTransform(40, 40);

        //if (VerticalAlignment == VerticalAlignment.Top)
        //{
        //    RenderTransform = new TranslateTransform(rect.X, rect.Y);
        //}
        //else if (VerticalAlignment == VerticalAlignment.Bottom)
        //{
        //    RenderTransform = new TranslateTransform(rect.X, 0); // TargetControl.Height + 4);
        //}

        //Child = new TextBlock()
        //{
        //    //[!TextBlock.TextProperty] = new Binding()
        //    //{
        //    //    Source = this,
        //    //    Path = nameof(ErrorMessage)
        //    //},
        //    Text = ErrorMessage,
        //    FontSize = 24,
        //    Foreground = Brushes.White,
        //    HorizontalAlignment = HorizontalAlignment.Center,
        //    VerticalAlignment = VerticalAlignment.Center,
        //    TextWrapping = TextWrapping.Wrap
        //};
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        Debug.WriteLine("Detached from Visual Tree");
    }

    public override void Detach()
    {
        if (TargetControl is null)
            return;

        RemovePseudoclasses();
        Debug.WriteLine("Detach");
        base.Detach();
        TargetControl.RemoveHandler(AvaDragDrop.DragOverEvent, TargetControl_DragOver);

        TargetControl.RenderTransform = null;
    }

    protected virtual void TargetControl_DragEnter(object? sender, DragEventArgs e)
    {
        if (TargetControl is null)
            return;

        var dragLocation = e.GetPosition(TargetControl);
        Update(dragLocation);
    }

    protected virtual void TargetControl_DragOver(object? sender, DragEventArgs e)
    {
        if (TargetControl is null)
            return;

        var dragLocation = e.GetPosition(TargetControl);
        Update(dragLocation);
    }

    public virtual void Update(Point dragLocation)
    {
        if (TargetControl is null)
            return;

        IsVisible = true;

        if (dragLocation.Y < (Math.Floor(TargetControl.Bounds.Height / 2)))
        {
            VerticalAlignment = VerticalAlignment.Top;
        }
        else
        {
            VerticalAlignment = VerticalAlignment.Bottom;
        }

        // Half adorner thickness on each side
        if (VerticalAlignment == VerticalAlignment.Top)
            Margin = new Thickness(0, -2, 0, -2);
        else if (VerticalAlignment == VerticalAlignment.Bottom)
            Margin = new Thickness(0, -2, 0, -2);

        UpdatePseudoclasses(VerticalAlignment == VerticalAlignment.Top);
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

    private void UpdatePseudoclasses(bool isTop)
    {
        if (isTop)
        {
            ((IPseudoClasses)TargetControl!.Classes).Set(":droptop", true);
            ((IPseudoClasses)TargetControl!.Classes).Set(":dropbottom", false);
        }
        else
        {
            ((IPseudoClasses)TargetControl!.Classes).Set(":dropbottom", true);
            ((IPseudoClasses)TargetControl!.Classes).Set(":droptop", false);
        }
    }

    private void RemovePseudoclasses()
    {
        ((IPseudoClasses)TargetControl!.Classes).Set(":droptop", false);
        ((IPseudoClasses)TargetControl!.Classes).Set(":dropbottom", false);
    }
}
