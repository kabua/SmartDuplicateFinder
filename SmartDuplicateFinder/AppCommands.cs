using System;
using System.Windows.Input;

namespace SmartDuplicateFinder;

public static class AppCommands
{
    private static readonly Type OwnerType = typeof(AppCommands);

    // Menu Item commands
    //
    public static readonly RoutedUICommand Exit = new("_Exit", "Exit", OwnerType, new InputGestureCollection(new[] { new KeyGesture(Key.F4, ModifierKeys.Alt) }));
    public static readonly RoutedUICommand AboutHelp = new ("About...", "AboutHelp", OwnerType);

    // Standard Dialog box commands
    //
    public static readonly RoutedUICommand Okay = new("Ok", "Ok", OwnerType);
    public static readonly RoutedUICommand Cancel = new("Cancel", "Cancel", OwnerType);
    public static readonly RoutedUICommand Close = new("Close", "Close", OwnerType);

    // Other types of commands, like buttons etc.
    //
    public static readonly RoutedUICommand Refresh = new ("_Refresh", "Refresh", OwnerType);
    public static readonly RoutedUICommand ClearAll = new ("Clear _All", "Clear", OwnerType);
}
