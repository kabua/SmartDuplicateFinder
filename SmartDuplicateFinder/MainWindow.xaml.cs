using Ninject;
using SmartDuplicateFinder.Dialog;
using SmartDuplicateFinder.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SmartDuplicateFinder;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
	public MainWindow()
	{
		InitializeComponent();
		AddCommandBindings();

		var view = App.Current.Container.Get<FindDuplicatesView>();
		SetView(view);

		Title = App.Name;
		DataContext = this;
	}

	public bool IsBusy { get; set; }

	public Control CurrentView { get; private set; }

	private void SetView(Control view)
	{
		CurrentView = view;
	}

	private void ShowAbout()
	{
		var dialog = new AboutDialog
		{
			Owner = this
		};

		dialog.ShowDialog();
	}

	private void AddCommandBindings()
	{
		//
		// File Menu
		//

		CommandBindings.Add(new CommandBinding(AppCommands.Exit, (sender, args) => Close()));

		//
		// Help Menu
		//

		CommandBindings.Add(new CommandBinding(AppCommands.AboutHelp, (sender, args) => ShowAbout()));
	}
}