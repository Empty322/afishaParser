using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace afishaParser {
	public partial class MainWindow : Window {
		private Parser parser;
		public MainWindow() {
			InitializeComponent();
			DataContext = new WindowViewModel(this, new DataViewModel());
			filters.DataContext = new FilterOptions(null);
			parser = new Parser();
			parser.eventLoaded += EventLoaded;
			parser.onCompleted += OnCompleted;
		}

		private void OnCompleted(object obj) {
			Dispatcher.Invoke(() => {
				((WindowViewModel)DataContext).DataViewModel.Sort(searchBox.Text);
			});
		}

		private void EventLoaded(object sender, Event ev) {
			Dispatcher.Invoke(() => {
				((WindowViewModel)DataContext).DataViewModel.Events.Add(ev);
			});
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			((WindowViewModel)DataContext).DataViewModel.Events = await parser.ParseAsync();
			if(((WindowViewModel)DataContext).DataViewModel.Events == null) {
				((WindowViewModel)DataContext).DataViewModel.Events = EventManager.GetInstance().LoadData();
				((WindowViewModel)DataContext).DataViewModel.Sort(searchBox.Text);
			}
			filters.DataContext = new FilterOptions(
				((WindowViewModel)DataContext).DataViewModel.Events.Select(ev => ev.Location).Distinct()
			);
		}

		private void SynchronizeBtn(object sender, RoutedEventArgs e) {
			if (((WindowViewModel)DataContext).DataViewModel.Events != null)
				EventManager.GetInstance().Synchronize(((WindowViewModel)DataContext).DataViewModel.Events);
		}

		private void SearchBoxTextChanged(object sender, TextChangedEventArgs e) {
			((WindowViewModel)DataContext).DataViewModel.Sort((sender as TextBox).Text);
		}

		private void ClearSearchBoxBtn(object sender, RoutedEventArgs e) {
			searchBox.Clear();
		}

		private void ShowFiltersBtn(object sender, RoutedEventArgs e) {
			if(filters.Visibility == Visibility.Collapsed)
				filters.Visibility = Visibility.Visible;
			else
				filters.Visibility = Visibility.Collapsed;
		}
		
		private void SubmitFiltersBtn(object sender, RoutedEventArgs e) {
			((WindowViewModel)DataContext).DataViewModel.Sort(filters.DataContext as FilterOptions);
			if(filters.Visibility == Visibility.Visible)
				filters.Visibility = Visibility.Collapsed;
		}

		private void ClearFiltersBtn(object sender, RoutedEventArgs e) {
			filters.DataContext = new FilterOptions(
				((WindowViewModel)DataContext).DataViewModel.Events.Select(ev => ev.Location).Distinct()
			);
		}
	}
}
