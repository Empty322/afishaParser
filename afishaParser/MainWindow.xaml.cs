using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;

namespace afishaParser {
	public partial class MainWindow : Window {
		public MainWindow() {
			InitializeComponent();
			DataContext = new WindowViewModel(this, new DataViewModel(new FilterOptions(null)));			
		}		
	}
}
