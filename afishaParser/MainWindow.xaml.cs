using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace afishaParser {
	/// <summary>
	/// Логика взаимодействия для MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		private Parser parser;
		private List<Event> events;
		public MainWindow() {
			InitializeComponent();
			parser = new Parser();
			parser.eventLoaded += EventLoaded;
			parser.onCompleted += OnCompleted;
		}

		private void OnCompleted(object obj) {
			Dispatcher.Invoke(() => synchroBtn.IsEnabled = true);
		}

		private void EventLoaded(object sender, Event ev) {
			Dispatcher.Invoke(() =>	dataGrid.Items.Add(ev));
			Console.WriteLine();
		}

		private async void Window_Loaded(object sender, RoutedEventArgs e) {
			DataGridTextColumn title = new DataGridTextColumn();
			title.Header = "Заголовок";
			title.Binding = new Binding("Title");
			title.Width = 180;
			dataGrid.Columns.Add(title);
			DataGridTextColumn location = new DataGridTextColumn();
			location.Header = "Место";
			location.Binding = new Binding("Location");
			dataGrid.Columns.Add(location);
			DataGridTextColumn date = new DataGridTextColumn();
			date.Header = "Дата";
			date.Binding = new Binding("Date");
			dataGrid.Columns.Add(date);
			DataGridTextColumn time = new DataGridTextColumn();
			time.Header = "Время";
			time.Binding = new Binding("Time");
			dataGrid.Columns.Add(time);
			DataGridTextColumn day = new DataGridTextColumn();
			day.Header = "День";
			day.Binding = new Binding("Day");
			dataGrid.Columns.Add(day);

			events = await parser.ParseAsync();
			if(events == null) {
				events = EventManager.GetInstance().LoadData();
				foreach (Event ev in events) {
					dataGrid.Items.Add(ev);
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e) {
			if (events != null)
				EventManager.GetInstance().Synchronize(events);
		}

		private static T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject {
			for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++) {
				var child = VisualTreeHelper.GetChild(parent, i);
				string controlName = child.GetValue(Control.NameProperty) as string;
				if(controlName == name) {
					return child as T;
				}
				else {
					T result = FindVisualChildByName<T>(child, name);
					if(result != null)
						return result;
				}
			}
			return null;
		}
	}
}
