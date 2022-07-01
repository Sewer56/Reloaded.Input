using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Reloaded.Input.Configurator.Model;

namespace Reloaded.Input.Configurator.WPF.Utilities;

// Adapted from, https://gist.github.com/angularsen/90040fb174f71c5ab3ad, originally MIT licensed.
// Not reusable, hardcoded for this launcher.

public class MarginSetter
{
    private static Thickness GetLastItemMargin(Panel obj) => (Thickness)obj.GetValue(LastItemMarginProperty);
    
    public static Thickness GetMargin(DependencyObject obj) => (Thickness)obj.GetValue(MarginProperty);

    public static void SetLastItemMargin(DependencyObject obj, Thickness value) => obj.SetValue(LastItemMarginProperty, value);

    public static void SetMargin(DependencyObject obj, Thickness value) => obj.SetValue(MarginProperty, value);

    public static bool GetEnable(DependencyObject obj) => (bool)obj.GetValue(EnableProperty);

    public static void SetEnable(DependencyObject obj, bool value) => obj.SetValue(EnableProperty, value);

    private static void EnableChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {        
        // Make sure this is put on a panel
        var panel = sender as Panel;
        if (panel == null)
            return;

        if (!(bool)e.NewValue) 
            return;

        // This is inefficient but it'll do for now.
        panel.SizeChanged -= SizeChanged;
        panel.SizeChanged += SizeChanged;
    }

    private static void SizeChanged(object sender, SizeChangedEventArgs e) => SetMarginsOnPanelLoaded(sender, null!);

    private static void MarginChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Make sure this is put on a panel
        var panel = sender as Panel;
        if (panel == null) 
            return;
        
        if (panel.IsLoaded)
            SetMarginsOnPanelLoaded(panel, null!);
    }

    private static void SetMarginsOnPanelLoaded(object sender, RoutedEventArgs e)
    {
        var panel = (Panel)sender;

        // Go over the children and set margin for them:
        for (var i = 0; i < panel.Children.Count; i++)
        {
            UIElement child = panel.Children[i];
            var fe = child as FrameworkElement;
            if (fe == null) 
                continue;

            bool isLastItem = i == panel.Children.Count - 1;
            fe.Margin = isLastItem ? GetLastItemMargin(panel) : GetMargin(panel);
        }
    }

    // Using a DependencyProperty as the backing store for Margin. This enables animation, styling, binding, etc...
    public static readonly DependencyProperty EnableProperty =
        DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(MarginSetter),
            new UIPropertyMetadata(false, EnableChanged));

    public static readonly DependencyProperty MarginProperty =
        DependencyProperty.RegisterAttached("Margin", typeof(Thickness), typeof(MarginSetter),
            new UIPropertyMetadata(new Thickness(), MarginChanged));

    public static readonly DependencyProperty LastItemMarginProperty =
        DependencyProperty.RegisterAttached("LastItemMargin", typeof(Thickness), typeof(MarginSetter),
            new UIPropertyMetadata(new Thickness(), MarginChanged));
}

public class Spacing
{
    public static double GetHorizontal(DependencyObject obj) => (double)obj.GetValue(HorizontalProperty);

    public static double GetVertical(DependencyObject obj) => (double)obj.GetValue(VerticalProperty);

    public static void SetHorizontal(DependencyObject obj, double space) => obj.SetValue(HorizontalProperty, space);

    public static void SetVertical(DependencyObject obj, double value) => obj.SetValue(VerticalProperty, value);

    private static void HorizontalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;

        MarginSetter.SetMargin(obj, new Thickness(0, 0, space, 0));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }
    
    private static void VerticalChangedCallback(object sender, DependencyPropertyChangedEventArgs e)
    {
        var space = (double)e.NewValue;
        var obj = (DependencyObject)sender;
        MarginSetter.SetMargin(obj, new Thickness(0, 0, 0, space));
        MarginSetter.SetLastItemMargin(obj, new Thickness(0));
    }

    public static readonly DependencyProperty VerticalProperty =
        DependencyProperty.RegisterAttached("Vertical", typeof(double), typeof(Spacing),
            new UIPropertyMetadata(0d, VerticalChangedCallback));

    public static readonly DependencyProperty HorizontalProperty =
        DependencyProperty.RegisterAttached("Horizontal", typeof(double), typeof(Spacing),
            new UIPropertyMetadata(0d, HorizontalChangedCallback));
}