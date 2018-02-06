using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace afishaParser {
	public class FilterOptions : BaseViewModel {

		private string day;

		private string location;

		private DateTime? fromDate;

		private DateTime? toDate;

		private IEnumerable<string> locations;


		public ICommand ClearFilters { get; set; }
		
		public ICommand SubmitFilters { get; set; }

		public string Day {
			get { return day; }
			set {
				day = value;
				OnPropertyChanged(nameof(Day));
			}
		}
		public string Location {
			get { return location; }
			set {
				location = value;
				OnPropertyChanged(nameof(Location));
			}
		}
		public DateTime? FromDate {
			get { return fromDate; }
			set {
				fromDate = value;
				OnPropertyChanged(nameof(FromDate));
			}
		}
		public DateTime? ToDate {
			get { return toDate; }
			set {
				toDate = value;
				OnPropertyChanged(nameof(ToDate));
			}
		}

		public IEnumerable<string> Locations {
			get { return locations; }
			set {
				locations = value;
				OnPropertyChanged(nameof(Locations));
			}
		}

		public bool IsEmpty {
			get {
				return Day == null && Location == null && FromDate == null && ToDate == null;
			}
		}

		public FilterOptions(IEnumerable<string> locations) {
			Locations = locations;
			ClearFilters = new RelayCommand(() => {
				Day = null;
				Location = null;
				FromDate = null;
				ToDate = null;
			});
		}
	}
}
