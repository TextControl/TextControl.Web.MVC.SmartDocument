using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using tx_pdf_container.Models;
using System.IO;
using TXTextControl;
using System.Text;

namespace tx_pdf_container.Controllers {
	public class HomeController : Controller {
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger) {
			_logger = logger;
		}

		public IActionResult Index() {
			return View(Directory.GetFiles("App_Data"));
		}

		public IActionResult Editor(string DocumentName) {

			if (DocumentName == null) {
				DocumentName = SmartDocument.CreateNewDocument();
			}

			return View(SmartDocument.ExtractSmartDocument(DocumentName));
		}

		[HttpPost]
		public IActionResult SaveDocument([FromBody] SmartDocument smartDocument) {

			SmartDocument.SaveDocument(smartDocument);
			return Ok();

		}

		[HttpPost]
		public IActionResult SaveAnnotations([FromBody] SmartDocument smartDocument) {

			SmartDocument.SaveAnnotations(smartDocument);
			return Ok();

		}

		public IActionResult Viewer(string DocumentName) {
			return View(SmartDocument.ExtractSmartDocument(DocumentName));
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error() {
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
	}
}
