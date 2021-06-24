using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TXTextControl;

namespace tx_pdf_container {
	public class SmartDocument {

		public string Document { get; set; }
		public string Annotations { get; set; } = "";
		public string Name { get; set; }

		// This method extracts the embedded files from the PDF document
		// and returns a SmartDocument object
		public static SmartDocument ExtractSmartDocument(string DocumentName) {

			SmartDocument smartDocument = new SmartDocument();

			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
				
				tx.Create();

				// the load PDF document
				TXTextControl.LoadSettings loadSettings = new LoadSettings();
				tx.Load("App_Data/" + DocumentName,
					TXTextControl.StreamType.AdobePDF,
					loadSettings);

				// loop through all attachments to find the original document
				// and the annotations
				foreach (EmbeddedFile file in loadSettings.EmbeddedFiles) {

					if (file.FileName == "original.tx")
						smartDocument.Document = Convert.ToBase64String((byte[])file.Data);

					if (file.FileName == "annotations.json")
						smartDocument.Annotations = System.Text.Encoding.UTF8.GetString((byte[])file.Data);
				}

				smartDocument.Name = DocumentName;
			}

			return smartDocument;
		}

		// this method creates a new, blank SmartDocument
		public static string CreateNewDocument() {

			var DocumentName = Guid.NewGuid().ToString() + ".pdf";

			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
				tx.Create();

				byte[] dataTx;

				// save the blank document in the internal TX format
				tx.Save(out dataTx, TXTextControl.BinaryStreamType.InternalUnicodeFormat);

				// create an attachment
				EmbeddedFile embeddedFile = new EmbeddedFile("original.tx", dataTx, null);
				embeddedFile.Relationship = "Source";

				TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings() {
					EmbeddedFiles = new EmbeddedFile[] { embeddedFile }
				};

				// save a PDF with the attached Text Control document embedded
				tx.Save("App_Data/" + DocumentName,
					TXTextControl.StreamType.AdobePDF,
					saveSettings);
			}

			return DocumentName;
		}

		// saves an edited document to the SmartDocument
		public static void SaveDocument(SmartDocument smartDocument) {
			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
				tx.Create();

				// load the edited document
				tx.Load(Convert.FromBase64String(smartDocument.Document), 
					TXTextControl.BinaryStreamType.InternalUnicodeFormat);

				// create an attachment from the edited document
				EmbeddedFile embeddedFile = new EmbeddedFile("original.tx",
					Convert.FromBase64String(smartDocument.Document),
					null);
				embeddedFile.Relationship = "Source";

				// attache the files
				TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings() {
					EmbeddedFiles = new EmbeddedFile[] { embeddedFile }
				};

				// save the document as PDF with the edited original attachment
				tx.Save("App_Data/" + smartDocument.Name, 
					TXTextControl.StreamType.AdobePDF,
					saveSettings);
			}
		}

		// saves the annotations in the SmartDocument
		public static void SaveAnnotations(SmartDocument smartDocument) {

			using (TXTextControl.ServerTextControl tx = new TXTextControl.ServerTextControl()) {
			
				tx.Create();

				// load the SmartDocument PDF
				TXTextControl.LoadSettings loadSettings = new LoadSettings();
				tx.Load("App_Data/" + smartDocument.Name,
					TXTextControl.StreamType.AdobePDF,
					loadSettings);

				// find the original embedded document
				foreach (EmbeddedFile file in loadSettings.EmbeddedFiles) {

					if (file.FileName == "original.tx")
						smartDocument.Document = Convert.ToBase64String((byte[])file.Data);

				}

				// load the original document
				tx.Load(Convert.FromBase64String(smartDocument.Document),
					BinaryStreamType.InternalUnicodeFormat);

				// create an attachment for the original document
				EmbeddedFile efOriginal = new EmbeddedFile("original.tx",
					Convert.FromBase64String(smartDocument.Document),
					null);
				efOriginal.Relationship = "Source";

				// create an attachment for the annotations
				EmbeddedFile efAnnotations = new EmbeddedFile("annotations.json",
					Encoding.UTF8.GetBytes(smartDocument.Annotations),
					null);
				efAnnotations.Relationship = "Source";

				// attach the files
				TXTextControl.SaveSettings saveSettings = new TXTextControl.SaveSettings() {
					EmbeddedFiles = new EmbeddedFile[] { efOriginal, efAnnotations }
				};

				// save the SmartDocument as PDF with attachments
				tx.Save("App_Data/" + smartDocument.Name, 
					TXTextControl.StreamType.AdobePDF, 
					saveSettings);
			}
		}
	}
}
