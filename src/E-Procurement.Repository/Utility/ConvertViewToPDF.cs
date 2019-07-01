using E_Procurement.Repository.Dtos;
using E_Procurement.Repository.Interface;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DinkToPdf;
using System.IO;

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
   

        public async Task<bool> CreateRFQPDF(RFQGenerationModel rfqGenerationModel)
        {
            string webRootPath = _hostingEnvironment.WebRootPath;
            //string contentRootPath = _hostingEnvironment.ContentRootPath;

            var filePath = Path.Combine(webRootPath, "TicketTemplate", rfqGenerationModel.RFQId + ".pdf");
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 10 },
                DocumentTitle =  "RFQ DETAILS",
                Out = Path.Combine(webRootPath, "TicketTemplate", rfqGenerationModel.RFQId + ".pdf") //@"D:\PDFCreator\Employee_Report.pdf"
            };
            //Out = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketTemplate", ticketViewModel.TicketNo + ".pdf")) //@"D:\PDFCreator\Employee_Report.pdf"
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = await _viewRender.RenderToStringAsync("/Views/Shared/TicketTemplate.cshtml", rfqGenerationModel),
                WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "css", "Ticket.css") }
            };
            //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
            //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            _converter.Convert(pdf);


            return true;
        }

        public async Task<bool> CreatePOPDF(RFQGenerationModel rfqGenerationModel)
        {
            try
            {

            
                string webRootPath = _hostingEnvironment.WebRootPath;
                //string contentRootPath = _hostingEnvironment.ContentRootPath;

                var filePath = Path.Combine(webRootPath, "Uploads", "PO_" + rfqGenerationModel.PONumber + ".pdf");
                var globalSettings = new GlobalSettings
                {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                    Margins = new MarginSettings { Top = 10 },
                    DocumentTitle = "RFQ DETAILS",
                    Out = Path.Combine(webRootPath, "Uploads", "PO_" + rfqGenerationModel.PONumber + ".pdf") //@"D:\PDFCreator\Employee_Report.pdf"
                };
                //Out = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "TicketTemplate", ticketViewModel.TicketNo + ".pdf")) //@"D:\PDFCreator\Employee_Report.pdf"
                var objectSettings = new ObjectSettings
                {
                    PagesCount = true,
                    HtmlContent = await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel),
                    WebSettings = { DefaultEncoding = "utf-8", UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "css", "amanda.css") }
                };
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
                var pdf = new HtmlToPdfDocument()
                {
                    GlobalSettings = globalSettings,
                    Objects = { objectSettings }
                };

                _converter.Convert(pdf);

                // send pdf to  mail
                var subject = "RFQ APPROVAL NOTIFICATION";

                var message = @"A new RFQ has been sent for your approval.</br>
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
                string emailBody = objectSettings.HtmlContent;// await _viewRender.RenderToStringAsync("/Views/PDFTemplates/PO.cshtml", rfqGenerationModel);
                await _emailSender.SendEmailAsync(rfqGenerationModel.VendorEmail, subject, emailBody, filePath);

                return true;
            }
            catch(Exception ex)
            {
                throw ex;
            }
        }
    }
}
