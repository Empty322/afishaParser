using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace afishaParser {
	public class DataViewModel : BaseViewModel {
		private Event selectedEvent;

		public List<Event> Events { get; set; }

		public ObservableCollection<Event> SortedEvents { get; set; }

		public Event SelectedEvent {
			get { return selectedEvent; }
			set { selectedEvent = value;
				OnPropertyChanged(nameof(SelectedEvent));
			}
		}

		public DataViewModel() {
			SortedEvents = new ObservableCollection<Event>();
			Events = new List<Event>();
		}

		public void Sort(FilterOptions fo) {
			SortedEvents.Clear();
			foreach(Event ev in Events)
				SortedEvents.Add(ev);
			if(fo.Day != "" && fo.Day != null) {
				for(int i = 0; i < SortedEvents.Count; i++) {
					if(SortedEvents[i].Day != fo.Day) {
						SortedEvents.RemoveAt(i);
						i--;
					}
				}
			}
			if(fo.Location != "" && fo.Location != null) {
				for(int i = 0; i < SortedEvents.Count; i++) {
					if(SortedEvents[i].Location != fo.Location) {
						SortedEvents.RemoveAt(i);
						i--;
					}
				}
			}
			if(fo.FromDate != null && fo.ToDate != null) {
				for(int i = 0; i < SortedEvents.Count; i++) {
					DateTime date = StringToDateTime(SortedEvents[i].Date);
					if (fo.FromDate > date || date > fo.ToDate ) {
						SortedEvents.RemoveAt(i);
						i--;
					}
				}
			}
			else if(fo.FromDate != null && fo.ToDate == null) {
				for(int i = 0; i < SortedEvents.Count; i++) {
					DateTime date = StringToDateTime(SortedEvents[i].Date);
					if(fo.FromDate > date) {
						SortedEvents.RemoveAt(i);
						i--;
					}
				}
			}
			else if(fo.ToDate != null && fo.FromDate == null) {
				for(int i = 0; i < SortedEvents.Count; i++) {
					DateTime date = StringToDateTime(SortedEvents[i].Date);
					if(fo.ToDate < date) {
						SortedEvents.RemoveAt(i);
						i--;
					}
				}
			}
			SelectDefault();
		}

		public void Sort(string str) {
			SortedEvents.Clear();
			for(int i = 0; i < Events.Count; i++)
				if(Events[i].Title.ToLower().Contains(str.ToLower()))
					SortedEvents.Add(Events[i]);
			SelectDefault();
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
	}
}
