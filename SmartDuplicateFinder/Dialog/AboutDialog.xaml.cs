using SmartDuplicateFinder.Utils;
using System.Windows;
using System.Windows.Input;

namespace SmartDuplicateFinder.Dialog;

/// <summary>
/// Interaction logic for AboutDialog.xaml
/// </summary>
public partial class AboutDialog : Window
{
	public AboutDialog()
	{
		InitializeComponent();
		CreateCommandBindings();
		GetVersionInfo();

		DataContext = this;
	}

	public string Version { get; private set; }

	private void GetVersionInfo()
	{
		Version = CoreAssembly.Version.ToString(4);
	}

	private void CreateCommandBindings()
	{
		CommandBindings.Add(new CommandBinding(AppCommands.Okay, (o, e) => Close()));
	}
}
