using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using System.Text.RegularExpressions;

namespace Ductilator.Behaviors;

/// <summary>
/// Behavior to restrict TextBox input to numeric values only
/// Allows decimal numbers, negative numbers, and scientific notation
/// </summary>
public class NumericTextBoxBehavior : Behavior<TextBox>
{
    // Pattern allows: optional minus, digits, optional decimal point, more digits, optional scientific notation
    // This regex is more permissive to allow intermediate typing states
    private static readonly Regex NumericRegex = new Regex(@"^-?(\d*\.?\d*)?([eE][+-]?\d*)?$", RegexOptions.Compiled);

    protected override void OnAttached()
    {
        base.OnAttached();
        if (AssociatedObject != null)
        {
            AssociatedObject.AddHandler(InputElement.TextInputEvent, OnTextInput, RoutingStrategies.Tunnel);
            AssociatedObject.AddHandler(InputElement.KeyDownEvent, OnKeyDown, RoutingStrategies.Tunnel);
        }
    }

    protected override void OnDetaching()
    {
        base.OnDetaching();
        if (AssociatedObject != null)
        {
            AssociatedObject.RemoveHandler(InputElement.TextInputEvent, OnTextInput);
            AssociatedObject.RemoveHandler(InputElement.KeyDownEvent, OnKeyDown);
        }
    }

    private void OnTextInput(object? sender, TextInputEventArgs e)
    {
        if (AssociatedObject == null || string.IsNullOrEmpty(e.Text))
            return;

        var textBox = AssociatedObject;
        var currentText = textBox.Text ?? string.Empty;
        var caretIndex = textBox.CaretIndex;

        // Normalize selection to handle reverse selections (e.g., drag right-to-left).
        var selectionStart = Math.Min(textBox.SelectionStart, textBox.SelectionEnd);
        var selectionEnd = Math.Max(textBox.SelectionStart, textBox.SelectionEnd);
        var selectionLength = selectionEnd - selectionStart;

        // Simulate the new text after this input
        string newText;
        if (selectionLength > 0)
        {
            newText = currentText.Remove(selectionStart, selectionLength);
            newText = newText.Insert(selectionStart, e.Text);
        }
        else
        {
            newText = currentText.Insert(caretIndex, e.Text);
        }

        // Special handling for decimal point - allow leading decimal point (will be treated as 0.)
        // and allow trailing decimal point during typing
        if (!string.IsNullOrEmpty(newText))
        {
            // Check if this would create multiple decimal points
            int decimalCount = 0;
            foreach (char c in newText)
            {
                if (c == '.') decimalCount++;
            }
            
            // Block if trying to add more than one decimal point
            if (decimalCount > 1)
            {
                e.Handled = true;
                return;
            }
        }

        // If the new text doesn't match the numeric pattern, cancel the input
        if (!NumericRegex.IsMatch(newText))
        {
            e.Handled = true;
        }
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // Allow control keys (backspace, delete, arrow keys, etc.)
        if (e.Key == Key.Back || e.Key == Key.Delete || 
            e.Key == Key.Left || e.Key == Key.Right ||
            e.Key == Key.Home || e.Key == Key.End ||
            e.Key == Key.Tab || e.Key == Key.Enter)
        {
            return;
        }

        // Allow Ctrl+A, Ctrl+C, Ctrl+V, Ctrl+X
        if ((e.KeyModifiers & KeyModifiers.Control) != 0)
        {
            if (e.Key == Key.A || e.Key == Key.C || e.Key == Key.V || e.Key == Key.X)
            {
                return;
            }
        }
    }
}
