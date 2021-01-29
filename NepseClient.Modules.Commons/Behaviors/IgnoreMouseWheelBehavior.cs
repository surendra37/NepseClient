﻿using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Xaml.Behaviors;

namespace NepseClient.Modules.Commons.Behaviors
{
    /// <summary>
    /// Captures and eats MouseWheel events so that a nested ListBox does not
    /// prevent an outer scrollable control from scrolling.
    /// </summary>
    public sealed class IgnoreMouseWheelBehavior : Behavior<UIElement>
    {

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewMouseWheel += AssociatedObject_PreviewMouseWheel;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewMouseWheel -= AssociatedObject_PreviewMouseWheel;
            base.OnDetaching();
        }

        static void AssociatedObject_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!(sender is DependencyObject))
            {
                return;
            }

            DependencyObject parent = VisualTreeHelper.GetParent((DependencyObject)sender);
            if (!(parent is UIElement))
            {
                return;
            }

            ((UIElement)parent).RaiseEvent(
                new MouseWheelEventArgs(e.MouseDevice, e.Timestamp, e.Delta) { RoutedEvent = UIElement.MouseWheelEvent });
            e.Handled = true;
        }

    }
}
