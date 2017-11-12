using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace afishaParser {
	struct Event {
		public int Id { get; set; }
		public string Title { get; set; }
		public string Location { get; set; }
		public string Time { get; set; }
		public string ImgPath { get; set; }
		public string Date { get; set; }
		public string Day { get; set; }
		public string Description { get; set; }
	}
}
