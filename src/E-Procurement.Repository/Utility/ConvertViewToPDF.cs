using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;
//using Microsoft.Office.Interop.Word;
using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
//using HtmlRenderer;


namespace E_Procurement.Repository.Utility
{
    public class ConvertViewToPDF : IConvertViewToPDF
    {
        private IConverter _converter;
        private IViewRenderService _viewRender;
        private IHostingEnvironment _hostingEnvironment;
        private readonly ISMTPService _emailSender;
        //private string projectRootFolder;

        public ConvertViewToPDF(IConverter converter, IViewRenderService viewRender, IHostingEnvironment env, ISMTPService emailSender)
        {
            _converter = converter;
            _viewRender = viewRender;
            _hostingEnvironment = env;
            _emailSender = emailSender;
        }
   

        public async Task<bool> CreateRFQPDF(RFQGenerationModel rfqGenerationModel, string emailMessage)
        {



            string webRootPath = _hostingEnvironment.WebRootPath;
            //string contentRootPath = _hostingEnvironment.ContentRootPath;

            var filePath = Path.Combine(webRootPath, "Uploads", "RFQ_" + rfqGenerationModel.RFQId + "_" + rfqGenerationModel.VendorId + ".pdf");
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle =  "RFQ DETAILS",
                Out = Path.Combine(webRootPath, "Uploads", "RFQ_" + rfqGenerationModel.RFQId + "_"+rfqGenerationModel.VendorId + ".pdf") //@"D:\PDFCreator\Employee_Report.pdf"
            };
            //Out = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketTemplate", ticketViewModel.TicketNo + ".pdf")) //@"D:\PDFCreator\Employee_Report.pdf"
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/RFQ_Notification.cshtml", rfqGenerationModel),
                //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "css", "Ticket.css") }
                WebSettings = { DefaultEncoding = "utf-8" }
            };
            //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };
            
            _converter.Convert(pdf);
            var subject = "RFQ NOTIFICATION";
            string emailBody = objectSettings.HtmlContent;// await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel);
            string fName = "RFQ_" + rfqGenerationModel.RFQId + "_" + rfqGenerationModel.VendorId + ".pdf";
            await _emailSender.SendEmailAsync(rfqGenerationModel.VendorEmail, subject, emailMessage, filePath, fName);


            return true;
        }

        //public async Task<bool> CreatePOPDF(RFQGenerationModel rfqGenerationModel)
        //{
        //    try
        //    {

            
        //        string webRootPath = _hostingEnvironment.WebRootPath;
        //        //string contentRootPath = _hostingEnvironment.ContentRootPath;

        //        var filePath = Path.Combine(webRootPath, "Uploads", "PO_" + rfqGenerationModel.PONumber + ".pdf");
        //        var globalSettings = new GlobalSettings
        //        {
        //            ColorMode = ColorMode.Color,
        //            Orientation = Orientation.Portrait,
        //            PaperSize = PaperKind.A4,
        //            Margins = new MarginSettings { Top = 10 },
        //            DocumentTitle = "RFQ DETAILS",
        //            Out = Path.Combine(webRootPath, "Uploads", "PO_" + rfqGenerationModel.PONumber + ".pdf") //@"D:\PDFCreator\Employee_Report.pdf"
        //        };
        //        //Out = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketTemplate", ticketViewModel.TicketNo + ".pdf")) //@"D:\PDFCreator\Employee_Report.pdf"
        //        //string HTMLC = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel);
        //        var objectSettings = new ObjectSettings
        //        {
        //            PagesCount = true,
        //            HtmlContent = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO_Notification.cshtml", rfqGenerationModel),
        //            //WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "css", "Ticket.css") }
        //            WebSettings = { DefaultEncoding = "utf-8" }
        //        };
        //        //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
        //        //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
        //        var pdf = new HtmlToPdfDocument()
        //        {
        //            GlobalSettings = globalSettings,
        //            Objects = { objectSettings }
        //        };

        //        _converter.Convert(pdf);

        //        // send pdf to  mail
        //        var subject = "PURCHASE ORDER NOTIFICATION";

        //        var message = @"A new PO has been sent for your approval.</br>
        //                         <div>
        //                        <p>Dear Sir,

        //                        <p><b>OFFER TO SUPPLY</p></b>
        //                        <p> Reference your Quotation and our negotiation meeting with you, we are pleased to inform you that</p>
        //                        <p> your company has been selected to carry out the above mentioned under the following terms and conditions:</p>
        //                        </div>";

        //        message += "</br><b>RFQ Number  : </b>" + rfqGenerationModel.RFQId.ToString();
        //        message += "</br><b>PO Number  : </b>" + rfqGenerationModel.PONumber;
        //        message += "</br>Kindly, log on to the Application and approve accordingly.";
        //        message += "</br>Regards";
        //        //string emailBody = objectSettings.HtmlContent;// await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel);
        //        string emailBody = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO_Notification.cshtml", rfqGenerationModel);
        //        string fName = "PO_" + rfqGenerationModel.PONumber + ".pdf";
        //        await _emailSender.SendEmailAsync(rfqGenerationModel.VendorEmail, subject, emailBody, filePath, fName);

        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public async Task<bool> CreatePOPDF(RFQGenerationModel rfqGenerationModel)
        {
            try
            {


                string webRootPath = _hostingEnvironment.WebRootPath;

                var filePath = Path.Combine(webRootPath, "Uploads", "PO_" + rfqGenerationModel.PONumber + ".pdf");


                 string FilePath = Directory.GetCurrentDirectory() + "\\Views\\PDFTemplates\\POGenTemplate.html";
                 StreamReader str = new StreamReader(FilePath);
                string htmlContent = str.ReadToEnd();
                htmlContent = htmlContent.Replace("[createdDate]", rfqGenerationModel.CreatedDate.ToShortDateString()).Replace("[VendorName]", rfqGenerationModel.VendorName)
                     .Replace("[VendorAddress]", rfqGenerationModel.VendorAddress).Replace("[Reference]", rfqGenerationModel.Reference).Replace(" [PONumber]", rfqGenerationModel.PONumber)
                     .Replace("[POTitle]", rfqGenerationModel.POTitle).Replace("[POCost]", rfqGenerationModel.POCost).Replace("[POWarranty]", rfqGenerationModel.POWarranty)
                      .Replace("[POTerms]", rfqGenerationModel.POTerms).Replace("[POValidity]", rfqGenerationModel.POValidity).Replace("[URL]", rfqGenerationModel.URL)
                     .Replace("[Signature1]", rfqGenerationModel.Signature1).Replace("[Signature2]", rfqGenerationModel.Signature2);
                    
                int i = 0;
                string app = "";
                foreach (var item in rfqGenerationModel.RFQDetails)
                {
                    i = i + 1;
                    app += $"<tr>" +
                        $"<td>{i}</td>" +
                        $"<td>{item.ItemName}</td>" +
                        $"<td>{item.Description}</td>" +
                        $"<td>{item.QuotedQuantity}</td>" +
                        $"<td >{item.QuotedPrice}</td>" +
                        $"<td align=\"right\">{item.QuotedAmount}</td>" +
                        $"</tr>";
                }



                htmlContent = htmlContent.Replace("[ApprovalConfig]", app).Replace("[TotalAmount]", rfqGenerationModel.TotalAmount.ToString());

                // Create a new PDF document
                Document document = new Document();

                // Create a new MemoryStream to store the PDF output
                MemoryStream output = new MemoryStream();

                // Create a new PdfWriter to write the PDF output to the MemoryStream
                 PdfWriter writer = PdfWriter.GetInstance(document, output);

                // Open the document for writing
                 document.Open();

                // Parse the HTML string into a series of iTextSharp elements
                var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(htmlContent), null);

                // Loop through each element and add it to the document
                
                for (int x = 0; x < parsedHtmlElements.Count; x++)
                {
                    if (x != 0)
                    {
                        document.Add(parsedHtmlElements[x]);
                    }
                    }
                    //// Close the document
                    document.Close();

                    //// Save the PDF to a file
                    File.WriteAllBytes(filePath, output.ToArray());

                    

                

                //string html = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO_Notification.cshtml", rfqGenerationModel);

                //// Create a new PDF document
                //PdfDocument pdfDoc = new PdfDocument(new PdfWriter("myPDF.pdf"));

                //// Create a PDF document object
                //Document doc = new Document(pdfDoc);

                //// Convert the HTML string to an image
                //Image image = HtmlConverter.ConvertToImage(html);

                //// Add the image to the PDF document
                //doc.Add(new Image(image).ScaleToFit(pdfDoc.GetDefaultPageSize().GetWidth(), pdfDoc.GetDefaultPageSize().GetHeight()));

                //// Close the document
                //doc.Close();



                // send pdf to  mail
                var subject = "PURCHASE ORDER NOTIFICATION";

                var message = @"A new PO has been sent for your approval.</br>
                                 <div>
                                <p>Dear Sir,

                                <p><b>OFFER TO SUPPLY</p></b>
                                <p> Reference your Quotation and our negotiation meeting with you, we are pleased to inform you that</p>
                                <p> your company has been selected to carry out the above mentioned under the following terms and conditions:</p>
                                </div>";

                message += "</br><b>RFQ Number  : </b>" + rfqGenerationModel.RFQId.ToString();
                message += "</br><b>PO Number  : </b>" + rfqGenerationModel.PONumber;
                message += "</br>Kindly, log on to the Application and approve accordingly.";
                message += "</br>Regards";
                //string emailBody = objectSettings.HtmlContent;// await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel);
                //string emailBody = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO_Notification.cshtml", rfqGenerationModel);
                string fName = "PO_" + rfqGenerationModel.PONumber + ".pdf";
               // await _emailSender.SendEmailAsync(rfqGenerationModel.VendorEmail, subject, htmlContent, filePath, fName);
                await _emailSender.SendEmailAsync("uidenthon@gmail.com", subject, htmlContent, filePath, fName);

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
