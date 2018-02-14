using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Diagnostics;
using System.Configuration;

namespace afishaParser
{
	public class EventManagerBD
	{

		#region Private Member

		private List<Event> events;
		static private EventManagerBD instance;

		#endregion

		#region Public Properties

		public SqlConnection Connection { get; set; }

		#endregion

		#region Constructor and GetIndtance

		/// <summary>
		/// Создает экземпляр класса EventManager
		/// </summary>
		private EventManagerBD()
		{
			Connection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=" + Directory.GetCurrentDirectory() + "\\afishaParserBD.mdf;Integrated Security=True;Connect Timeout=30");
			events = null;
		}

		/// <summary>
		/// Возвращает экземпляр класса EventManager
		/// </summary>
		/// <returns>Экземпляр класса EventManager</returns>
		static public EventManagerBD GetInstance()
		{
			if(instance == null)
				instance = new EventManagerBD();
			return instance;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Синхронизирует локальные данные с загруженными
		/// </summary>
		/// <param name="events">Лист событий</param>
		public void Synchronize(List<Event> events)
		{
			this.events = events;
			if(IsDifferent())
				ReplaceData();
		}

		/// <summary>
		/// Загружает события из файла
		/// </summary>
		/// <returns></returns>
		public List<Event> LoadData()
		{
			List<Event> temp = new List<Event>();

			SqlCommand command = new SqlCommand("SELECT * FROM Events", Connection);
			try
			{
				Connection.Open();
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						Event ev = new Event
						{
							Id = (int)reader["Id"],
							Title = (string)reader["Title"],
							Location = (string)reader["Location"],
							Time = (string)reader["Time"],
							ImgPath = (string)reader["ImgPath"],
							Date = (string)reader["Date"],
							Day = (string)reader["Day"],
							Description = (string)reader["Description"]
						};
						temp.Add(ev);
					}
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{
				Connection.Close();
			}
			return temp;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Сравнивает, отличаются ли данные на диске от загруженных
		/// </summary>
		/// <returns>Возвращает True, если данные различны</returns>
		private bool IsDifferent()
		{
			List<Event> temp = LoadData();
			foreach(Event ev in events)
			{
				bool exist = false;
				foreach(Event ev2 in temp)
				{
					if(ev2.Id == ev.Id)
						exist = true;
				}
				if(!exist)
					return true;
			}
			return false;
		}

		/// <summary>
		/// Создает файл событий
		/// </summary>
		public void SaveData()
		{
			if(events == null)
				return;
			SqlCommand command = new SqlCommand("DELETE FROM Events", Connection);
			try
			{
				Connection.Open();
				command.ExecuteNonQuery();

				foreach(Event ev in events)
				{
					command = new SqlCommand(string.Format("INSERT INTO Events VALUES " +
						"({0}, N'{1}', N'{2}', N'{3}', N'{4}', " +
					"N'{5}', N'{6}', N'{7}')", ev.Id, ev.Title, ev.Location, ev.Time, ev.ImgPath, ev.Date, ev.Day, ev.Description), Connection);
					command.ExecuteNonQuery();
				}
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			finally
			{
				Connection.Close();
			}
		}

		/// <summary>
		/// Заменяет файл событий на диске
		/// </summary>
		private void ReplaceData()
		{
			if(!Directory.Exists(Directory.GetCurrentDirectory() + "\\pics"))
				Directory.CreateDirectory("pics");
			string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + "\\pics");
			foreach(string file in files)
			{
				bool exist = false;
				for(int i = 0; i < events.Count - 1; i++)
				{
					if(events[i].ImgPath == file)
					{
						exist = true;
						break;
					}
				}
				if(!exist)
				{
					File.Delete(file);
				}
			}
			SaveData();
		}

		#endregion

	}
}
