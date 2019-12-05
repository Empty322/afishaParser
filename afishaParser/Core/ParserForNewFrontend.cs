using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;

namespace afishaParser {
	public class ParserForNewFrontend : Parser {
		protected const string gigClassName = "gig";
		protected const string titleClassName = "search_cell";
		protected const string placeClassName = "place";
		protected const string dateClassName = "date";
		protected const string dayClassName = "right-date";
		protected const string imageClassName = "image";
		protected const string reviewClassName = "gig-review";



		public ParserForNewFrontend(string url) : base(url) { }

		/// <summary>
		/// Достать инф. о событии из блока исходного кода страницы
		/// </summary>
		/// <param name="htmlEvent">Блок исходного кода в котором содержится инф. о событии</param>
		/// <returns>Информация о событии</returns>
		protected override Event GetInfo(IHtmlDocument htmlEvent) {
			Event eventInfo = new Event();

			var topElement = htmlEvent.QuerySelector("a." + gigClassName);
			eventInfo.Id = Convert.ToInt32(topElement.GetAttribute("data-id"));

			var title = htmlEvent.QuerySelector("strong." + titleClassName);
			eventInfo.Title = title.TextContent.Trim(' ', '\n').Replace("'", "\"");

			var location = htmlEvent.QuerySelector("div." + placeClassName + ">span[itemprop=\"name\"]");
			eventInfo.Location = location.TextContent.Replace("'", "\"");

			string gigUrl = htmlEvent.QuerySelector("link[itemprop=\"url\"]").GetAttribute("href");
			string gigHtml = GetHtml(gigUrl);
			HtmlParser domParser = new HtmlParser();
			IHtmlDocument gigPage = domParser.Parse(gigHtml);
			var review = gigPage.QuerySelector("div." + reviewClassName);

			foreach (var node in review.ChildNodes) {
				if (node.NodeType == AngleSharp.Dom.NodeType.Text) {
					eventInfo.Description += node.TextContent.Trim().Replace("'", "\"") + '\n';
				}
			}

			DateTime date = DateTime.Parse( htmlEvent
				.QuerySelector("div." + dateClassName + ">time[itemprop=\"startDate\"]")
				.GetAttribute("datetime")
			);
			eventInfo.Time = date.ToShortTimeString();
			eventInfo.Date = date.ToShortDateString();

			var day = htmlEvent.QuerySelector("div." + dayClassName);
			eventInfo.Day = day.TextContent.Trim(' ', '\n').Replace("'", "\"");

			string imgUrl = htmlEvent.QuerySelector("link[itemprop=\"image\"]").GetAttribute("href");

			try {
				eventInfo.ImgPath = Path.Combine(
					Directory.GetCurrentDirectory(),
					"pics", 
					eventInfo.Id.ToString() + imgUrl.Substring(imgUrl.LastIndexOf('.'))
				);
				DownloadPic(imgUrl, eventInfo.ImgPath);
			}
			catch {
				eventInfo.ImgPath = "Resource/default.png";
			}
			return eventInfo;
		}

		/// <summary>
		/// Скачать изображение
		/// </summary>
		/// <param name="imgUrl">Источник изображения</param>
		/// <param name="destPath">Файл назначения</param>
		protected void DownloadPic(string imgUrl, string destPath) {
			// создать папку pics если ее нет
			if(!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), "pics")))
				Directory.CreateDirectory("pics");

			// скачать картинку если она ранее не скачивалась
			if(!File.Exists(destPath)) {
				WebClient client = new WebClient();
				client.DownloadFile(imgUrl, destPath);
			}
		}

		/// <summary>
		/// Разделить страницу на блоки с инф. о событии
		/// </summary>
		/// <param name="doc">Исходный код страницы</param>
		/// <returns>HTML блок с инф. о событии</returns>
		protected override List<string> DivideHtml(IHtmlDocument doc) {
			List<string> htmlEvents = new List<string>();
			var items = doc.QuerySelectorAll("a." + gigClassName);
			foreach(var item in items) {
				 htmlEvents.Add(item.OuterHtml);
			}
			return htmlEvents;
		}
	}
}