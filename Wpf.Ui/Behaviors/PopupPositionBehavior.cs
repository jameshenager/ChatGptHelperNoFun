using System.Windows;
using System.Windows.Controls.Primitives;
using Microsoft.Xaml.Behaviors;

namespace Wpf.Ui.Common.Behaviors;

public class PopupPositionBehavior : Behavior<Popup>
{
    protected override void OnAttached()
    {
        base.OnAttached();
        var window = Application.Current.MainWindow;
        if (window == null) { return; }

        window.LocationChanged += Window_LocationOrSizeChanged;
        window.SizeChanged += Window_LocationOrSizeChanged;
    }

    protected override void OnDetaching()
    {
        var window = Application.Current.MainWindow;
        if (window != null)
        {
            window.LocationChanged -= Window_LocationOrSizeChanged;
            window.SizeChanged -= Window_LocationOrSizeChanged;
        }
        base.OnDetaching();
    }

    private void Window_LocationOrSizeChanged(object? sender, EventArgs e)
    {
        var popup = AssociatedObject;
        if (popup is not { IsOpen: true, }) { return; }
        var offset = popup.HorizontalOffset;
        popup.HorizontalOffset = offset + 1;
        popup.HorizontalOffset = offset;
    }
}

//public class ElementVisibilityByFocusBehavior : Behavior<UIElement>
//{
//    // DependencyProperty for TargetElement...

//    protected override void OnAttached()
//    {
//        TargetElement.GotFocus += (s, e) => AssociatedObject.Visibility = Visibility.Visible;
//        TargetElement.LostFocus += (s, e) => AssociatedObject.Visibility = Visibility.Collapsed;
//    }

//    // OnDetaching to clean up event handlers...
//}