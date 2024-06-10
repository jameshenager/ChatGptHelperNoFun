using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Xaml.Behaviors;

namespace Wpf.Ui.Common.Behaviors;

public class FrameworkElementFadeInFadeOutBehavior : Behavior<FrameworkElement>
{
    // Dependency Property for FadeInDuration
    public static readonly DependencyProperty FadeInDurationProperty = DependencyProperty.Register(
        nameof(FadeInDuration),
        typeof(TimeSpan),
        typeof(FrameworkElementFadeInFadeOutBehavior),
        new PropertyMetadata(TimeSpan.FromSeconds(0.5))); // Default value

    // Dependency Property for FadeOutDuration
    public static readonly DependencyProperty FadeOutDurationProperty = DependencyProperty.Register(
        nameof(FadeOutDuration),
        typeof(TimeSpan),
        typeof(FrameworkElementFadeInFadeOutBehavior),
        new PropertyMetadata(TimeSpan.FromSeconds(0.5))); // Default value

    public TimeSpan FadeInDuration
    {
        get => (TimeSpan)GetValue(FadeInDurationProperty);
        set => SetValue(FadeInDurationProperty, value);
    }

    public TimeSpan FadeOutDuration
    {
        get => (TimeSpan)GetValue(FadeOutDurationProperty);
        set => SetValue(FadeOutDurationProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Loaded += (_, _) => FadeIn();
    }

    public void FadeIn()
    {
        var animation = new DoubleAnimation(0, 1, new Duration(FadeInDuration));
        AssociatedObject.BeginAnimation(UIElement.OpacityProperty, animation);
    }

    public void FadeOut()
    {
        var animation = new DoubleAnimation(1, 0, new Duration(FadeOutDuration));
        AssociatedObject.BeginAnimation(UIElement.OpacityProperty, animation);
    }
}