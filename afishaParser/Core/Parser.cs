using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using AngleSharp.Parser.Html;
using AngleSharp.Dom.Html;
using System.Windows;
using System.Diagnostics;

namespace afishaParser {
	public class Parser {

		protected readonly string url;

		public Parser(string url) {
			this.url = url;
		}

		public event Action<object, Event> EventLoaded;
		public event Action<object> OnCompleted;

		/// <summary>
		/// Загрузить лист событий
		/// </summary>
		/// <returns>Лист событий</returns>
		public List<Event> Parse() {
			List<Event> events = new List<Event>();
			// получить html строку
			string html = "";
			try {
				html = GetHtml(url);
			}
			catch {
				return null;
			}
			// разбить ее на части
			HtmlParser domParser = new HtmlParser();
			IHtmlDocument doc = domParser.Parse(html);
			List<string> htmlEvents = DivideHtml(doc);
			// вытащить из этих частей инф.
			for(int i = 0; i < htmlEvents.Count - 1; i++) {
				doc = domParser.Parse(htmlEvents[i]);
				events.Add(GetInfo(doc));
				if(EventLoaded != null)
					EventLoaded?.Invoke(this, events[i]);
			}
			if(OnCompleted != null)
				OnCompleted?.Invoke(this);

			return events;
		}

		/// <summary>
		/// Загрузить лист событий асинхронно
		/// </summary>
		/// <returns>Лист событий</returns>
		public async Task< List<Event> > ParseAsync() {
			Task< List<Event> > task = new Task< List<Event> >(Parse);
			task.Start();
			await task;
			return task.Result;
		}

		/// <summary>
		/// Достать инф. о событии из блока исходного кода страницы
		/// </summary>
		/// <param name="htmlEvent">Блок исходного кода в котором содержится инф. о событии</param>
		/// <returns>Информация о событии</returns>
		protected virtual Event GetInfo(IHtmlDocument htmlEvent) {
			Event temp = new Event();
			//заглавие
			var title = htmlEvent.QuerySelectorAll("span").Where(i => i.ClassName != null && i.ClassName == "gig_title");
			var t = title.ToArray();
			temp.Title = title.ElementAt(0).TextContent.Trim(' ', '\n').Replace("'", "\"");
			//место проведения
			var location = htmlEvent.QuerySelector("[itemprop=\"name\"]");
			temp.Location = location.TextContent.Replace("'", "\"");
			//описание
			var description = htmlEvent.QuerySelector("[itemprop=\"description\"]");
			temp.Description = description.TextContent.Replace("'", "\"");
			//время
			var time = htmlEvent.QuerySelectorAll("span").Where(i => i.ClassName != null && i.ClassName == "gig_time");
			temp.Time = time.ElementAt(0).TextContent.Trim(' ', '\n').Replace("'", "\"");
			//дата
			var date = htmlEvent.QuerySelector("[itemprop=\"startDate\"]");
			temp.Date = date.TextContent.Substring(0, 10);
			//день
			var day = htmlEvent.QuerySelectorAll("span").Where(i => i.ClassName != null && i.ClassName == "gig_day");
			temp.Day = day.ElementAt(0).TextContent.Trim(' ', '\n').Replace("'", "\"");
			//путь к картинке
			var ImgSrc = htmlEvent.QuerySelectorAll("img").Where(i => i.ClassName != null && i.ClassName == "gig_img");
			string imgUrl = ImgSrc.ElementAt(0).GetAttribute("src");
			//id по номеру картинки
			int sidx = imgUrl.LastIndexOf("/") + 1;
			int len = imgUrl.LastIndexOf(".") - sidx;
			temp.Id = Convert.ToInt32(imgUrl.Substring(sidx, len));

			//Попытаться всачать картинку
			try
			{
				//скачать картинку
				temp.ImgPath = DownloadPic(ImgSrc.ElementAt(0).GetAttribute("src"));
			}
			//если не удалось
			catch
			{
				//поставить картинку по умолчанию
				temp.ImgPath = "Resource/default.png";
			}			
			return temp;
		}

		/// <summary>
		/// Скачать изображение
		/// </summary>
		/// <param name="src">Путь к изображению</param>
		/// <returns>Путь к скачанному изображению</returns>
		protected virtual string DownloadPic(string src) {
			// создать папку pics если ее нет
			if(!Directory.Exists(Directory.GetCurrentDirectory() + "pics"))
				Directory.CreateDirectory("pics");
			int idx = src.LastIndexOf('/');
			string path = Directory.GetCurrentDirectory() + "\\pics\\" + src.Substring(idx + 1);
			// если такой картинки нет, то скачать ее
			if(!File.Exists(path)) {
				WebClient client = new WebClient();
				client.DownloadFile(url + src, path);
			}
			return path;
		}

		/// <summary>
		/// Разделить страницу на блоки с инф. о событии
		/// </summary>
		/// <param name="doc">Исходный код страницы</param>
		/// <returns>HTML блок с инф. о событии</returns>
		protected virtual List<string> DivideHtml(IHtmlDocument doc) {
			List<string> htmlEvents = new List<string>();
			var items = doc.QuerySelectorAll("div").Where(item => item.ClassName != null && item.ClassName == "gig_block");
			foreach(var item in items) {
				htmlEvents.Add(item.InnerHtml);
			}
			return htmlEvents;
		}

		/// <summary>
		/// Получить исходный код страницы
		/// </summary>
		/// <returns>исходный код страницы</returns>
		protected string GetHtml(string url) {
			string html = "";
			HttpClient client = new HttpClient();
			HttpResponseMessage res;
			try {
				res = client.GetAsync(url).Result;
			}
			catch (AggregateException) {
				// Проблемы с соединением
				throw new AggregateException();
			}
			html = res.Content.ReadAsStringAsync().Result;
			return html;
		}
	}
}
