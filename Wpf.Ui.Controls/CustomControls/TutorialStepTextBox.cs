using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Wpf.Ui.Controls.CustomControls;

public class TutorialStepTextBox : TextBox
{
    public static readonly DependencyProperty TutorialStepActiveProperty = DependencyProperty.Register(
        nameof(TutorialStepActive),
        typeof(bool),
        typeof(TutorialStepTextBox),
        new PropertyMetadata(false, OnTutorialStepActiveChanged));

    public bool TutorialStepActive
    {
        get => (bool)GetValue(TutorialStepActiveProperty);
        set => SetValue(TutorialStepActiveProperty, value);
    }

    public static readonly DependencyProperty TutorialStepColorProperty = DependencyProperty.Register(
        nameof(TutorialStepColor),
        typeof(Brush),
        typeof(TutorialStepTextBox),
        new PropertyMetadata(new SolidColorBrush(Colors.LightPink))); // Default color

    public Brush TutorialStepColor
    {
        get => (Brush)GetValue(TutorialStepColorProperty);
        set => SetValue(TutorialStepColorProperty, value);
    }

    private static void OnTutorialStepActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is TutorialStepTextBox textBox)
        {
            textBox.Background = (bool)e.NewValue ? textBox.TutorialStepColor : new SolidColorBrush(Colors.White);
        }
    }
}