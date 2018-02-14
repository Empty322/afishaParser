using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Runtime.InteropServices;

namespace afishaParser {
	public class WindowViewModel : BaseViewModel {
		#region Private Member

		private Window window;

		private int outerMarginSize = 10;

		private int windowRadius = 5;

		#endregion

		#region Public Properties

		public  DataViewModel DataViewModel { get; private set; }

		public int WindowMinimumWidth { get; set; } = 700;

		public int WindowMinimumHeight { get; set; } = 400;

		public Thickness InnerContentPadding {
			get {
				return new Thickness(ResizeBorder);
			}
		}

		public int ResizeBorder { get; set; } = 3;

		public Thickness ResizeBorderThickness {
			get {
				return new Thickness(ResizeBorder + OuterMarginSize);
			}
		}

		public int OuterMarginSize {
			get {
				return window.WindowState == WindowState.Maximized ? 0 : outerMarginSize;
			}
			set {
				outerMarginSize = value;
			}
		}

		public Thickness OuterMarginSizeThickness {
			get {
				return new Thickness(OuterMarginSize);
			}
		}

		public int WindowRadius {
			get {
				return window.WindowState == WindowState.Maximized ? 0 : windowRadius;
			}
			set {
				windowRadius = value;
			}
		}

		public CornerRadius WindowCornerRadius {
			get {
				return new CornerRadius(WindowRadius);
			}
		}

		public int TitleHeight { get; set; } = 42;

		public GridLength TitleHeightGridLenght {
			get {
				return new GridLength(TitleHeight + ResizeBorder);
			}
		}

		#endregion

		#region Commands

		public ICommand MinimizeCommand { get; set; }
		public ICommand MaximizeCommand { get; set; }
		public ICommand CloseCommand { get; set; }
		public ICommand MenuCommand { get; set; }

		#endregion

		#region Constructor

		public WindowViewModel(Window window, DataViewModel dvm) {
			this.window = window;
			this.DataViewModel = dvm;

			window.StateChanged += (sender, e) =>
			{
				OnPropertyChanged(nameof(ResizeBorderThickness));
				OnPropertyChanged(nameof(OuterMarginSize));
				OnPropertyChanged(nameof(OuterMarginSizeThickness));
				OnPropertyChanged(nameof(WindowRadius));
				OnPropertyChanged(nameof(WindowCornerRadius));
			};

			MinimizeCommand = new RelayCommand(() => { window.WindowState = WindowState.Minimized; });
			MaximizeCommand = new RelayCommand(() => { window.WindowState ^= WindowState.Maximized; });
			CloseCommand = new RelayCommand(() => { window.Close(); });
			MenuCommand = new RelayCommand(() => { SystemCommands.ShowSystemMenu(window, (GetMousePosition())); });

			WindowResizer resiser = new WindowResizer(window);
		}

		#endregion

		#region Helpers

		[DllImport("user32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		internal static extern bool GetCursorPos(ref Win32Point pt);

		internal struct Win32Point {
			public Int32 x;
			public Int32 y;
		}

		private static Point GetMousePosition() {
			Win32Point w32Mouse = new Win32Point();
			GetCursorPos(ref w32Mouse);
			return new Point(w32Mouse.x, w32Mouse.y);
		}

		#endregion
	}
}
