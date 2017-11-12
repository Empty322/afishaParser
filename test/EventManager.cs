using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace afishaParser {
	class EventManager : IEventManager{
		private List<Event> events;
		private string currentDataPath;
		static private EventManager instance;

		/// <summary>
		/// Создает экземпляр класса EventManager
		/// </summary>
		private EventManager() {
			currentDataPath = Directory.GetCurrentDirectory() + "\\data";
			if (!Directory.Exists(currentDataPath)) {
				Directory.CreateDirectory("data");
			}
		}

		/// <summary>
		/// Возвращает экземпляр класса EventManager
		/// </summary>
		/// <returns>Экземпляр класса EventManager</returns>
		static public EventManager GetInstance() {
			if(instance == null)
				instance = new EventManager();
			return instance;
		}

		/// <summary>
		/// Синхронизирует локальные данные с загруженными
		/// </summary>
		/// <param name="events">Лист событий</param>
		public void Synchronize(List<Event> events) {
			this.events = events;
			// если нет файла объектов, то создать его
			if(!File.Exists(currentDataPath + "\\data.dat")) {
				SaveData();
			}
			// если есть, то перезаписать его
			else {
				if (IsDifferent())
					ReplaceData();
			}
		}

		/// <summary>
		/// Сравнивает, отличаются ли данные на диске от загруженных
		/// </summary>
		/// <returns></returns>
		private bool IsDifferent() {
			List<Event> temp = new List<Event>();
			FileStream infile = new FileStream(currentDataPath + "\\data.dat", FileMode.Open, FileAccess.Read);
			using(BinaryReader br = new BinaryReader(infile)) {
				Event tempev = new Event();
				try {
					while(true) {
						tempev.Id = br.ReadInt32();
						br.ReadString();
						br.ReadString();
						br.ReadString();
						br.ReadString();
						br.ReadString();
						br.ReadString();
						br.ReadString();
						temp.Add(tempev);
					}
				}
				catch (EndOfStreamException) {

				}
			}
			foreach (Event ev in events) {
				bool exist = false;
				foreach (Event ev2 in temp) {
					if(ev2.Id == ev.Id)
						exist = true;
				}
				if(!exist) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// Создает файл событий на диске
		/// </summary>
		private void SaveData() {
			if(events == null)
				return;
			FileStream outfile = new FileStream(currentDataPath + "\\data.dat", FileMode.Create, FileAccess.Write);
			using (BinaryWriter bw = new BinaryWriter(outfile)) {
				foreach (Event ev in events) {
					bw.Write(ev.Id);
					bw.Write(ev.Title);
					bw.Write(ev.Location);
					bw.Write(ev.Time);
					bw.Write(ev.ImgPath);
					bw.Write(ev.Date);
					bw.Write(ev.Day);
					bw.Write(ev.Description);
				}
			}
		}

		/// <summary>
		/// Заменяет файл событий на диске
		/// </summary>
		private void ReplaceData() {
			File.Delete("data.dat");
			string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\pics");
			foreach (string file in files) {
				bool exist = false;
				for (int i = 0; i < events.Count - 1; i++) {
					if (events[i].ImgPath == file) {
						exist = true;
						break;
					}
				}
				if(!exist) {
					File.Delete(file);
				}
			}
			SaveData();
		}
	}
}
