using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Linq;

namespace afishaParser {
	public class DataViewModel : BaseViewModel {

		#region Private Member

		private Event selectedEvent;

		private Parser parser;

		private string searchText = "";

		#endregion

		#region Public Properties

		public List<Event> Events { get; set; }

		public ObservableCollection<Event> SortedEvents { get; set; }

		public FilterOptions FilterOptions { get; set; }

		public Event SelectedEvent {
			get { return selectedEvent; }
			set { selectedEvent = value;
				OnPropertyChanged(nameof(SelectedEvent));
			}
		}
				
		public string SearchText {
			get { return searchText; }
			set {
				searchText = value;
				OnPropertyChanged(nameof(SearchText));
				Sort();
			}
		}

		#endregion

		#region Commands

		public ICommand ClearSearchText { get; set; }

		#endregion

		#region Constructor

		public DataViewModel(FilterOptions fo) {
			FilterOptions = fo;
			FilterOptions.SubmitFilters = new RelayCommand(() => Sort());
			SortedEvents = new ObservableCollection<Event>();
			Events = new List<Event>();
			ClearSearchText = new RelayCommand(() => SearchText = "");
			StartParse();
		}

		#endregion

		#region Private Methods

		private void Sort() {
			SortedEvents.Clear();
			foreach(Event ev in Events)
				SortedEvents.Add(ev);
			if(!FilterOptions.IsEmpty) {
				if(FilterOptions.Day != "" && FilterOptions.Day != null) {
					for(int i = 0; i < SortedEvents.Count; i++) {
						if(SortedEvents[i].Day != FilterOptions.Day) {
							SortedEvents.RemoveAt(i);
							i--;
						}
					}
				}
				if(FilterOptions.Location != "" && FilterOptions.Location != null) {
					for(int i = 0; i < SortedEvents.Count; i++) {
						if(SortedEvents[i].Location != FilterOptions.Location) {
							SortedEvents.RemoveAt(i);
							i--;
						}
					}
				}
				if(FilterOptions.FromDate != null && FilterOptions.ToDate != null) {
					for(int i = 0; i < SortedEvents.Count; i++) {
						DateTime date = StringToDateTime(SortedEvents[i].Date);
						if(FilterOptions.FromDate > date || date > FilterOptions.ToDate) {
							SortedEvents.RemoveAt(i);
							i--;
						}
					}
				}
				else if(FilterOptions.FromDate != null && FilterOptions.ToDate == null) {
					for(int i = 0; i < SortedEvents.Count; i++) {
						DateTime date = StringToDateTime(SortedEvents[i].Date);
						if(FilterOptions.FromDate > date) {
							SortedEvents.RemoveAt(i);
							i--;
						}
					}
				}
				else if(FilterOptions.ToDate != null && FilterOptions.FromDate == null) {
					for(int i = 0; i < SortedEvents.Count; i++) {
						DateTime date = StringToDateTime(SortedEvents[i].Date);
						if(FilterOptions.ToDate < date) {
							SortedEvents.RemoveAt(i);
							i--;
						}
					}
				}
			}

			if(SearchText != "")
				for(int i = 0; i < SortedEvents.Count; i++)
					if(!SortedEvents[i].Title.ToLower().Contains(SearchText.ToLower())) {
						SortedEvents.RemoveAt(i);
						i--;
					}

			SelectDefault();
		}

		private async void StartParse() {
			parser = new Parser();
			Events = await parser.ParseAsync();
			if(Events == null)
				Events = EventManager.GetInstance().LoadData();
			FilterOptions.Locations = new List<string>(Events.Select((ev) => ev.Location).Distinct());
			Sort();
		}

		private void SelectDefault() {
			if(SelectedEvent == null && SortedEvents.Count != 0)
				SelectedEvent = SortedEvents[0];
		}

		private DateTime StringToDateTime(string str) {
			int year = Int32.Parse(str.Substring(0, 4));
			int month = Int32.Parse(str.Substring(5, 2));
			int day = Int32.Parse(str.Substring(8, 2));
			return new DateTime(year, month, day);
		}

		#endregion
	}
}
