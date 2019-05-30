using AutoMapper;
using E_Procurement.Repository.ItemRepo;
using Microsoft.AspNetCore.Mvc;

namespace E_Procurement.WebUI.Controllers
{
    public class ItemController : Controller
    {
        private readonly IItemCategoryRepository _itemCategoryService;
        private readonly IMapper _mapper;

        public ItemController(IItemCategoryRepository itemCategoryService, IMapper mapper)
        {
            _itemCategoryService = itemCategoryService;
            _mapper = mapper;
        }
        //public ActionResult ItemCategory()
        //{
        //    ItemCategory booking = new ItemCategory();



        //    var Id = Request.Query["Id"].ToString();
        //    var transactionResult = _itemCategoryService.GetCategories(Id);
        //    var BookedTran = _bookingTransaction.Entities.Where(b => b.PaymentReference == reference).SingleOrDefault();

        //    //var SuccessfulBooked = _bookingTransactionStatusView.Entities.Where(b => b.BookingRef == BookedTran.BookingRef).SingleOrDefault();
        //    try
        //    {
        //        var apiUrl = ConfigurationManager.AppSettings["UpdatePaymentComplete"];
        //        //"http://localhost:64082/api/BookingTransaction/UpdateBookingPayment";
        //        var jsonSerializer = new JavaScriptSerializer();
        //        var myJsonUpdate = jsonSerializer.Serialize(new { PaymentReference = transactionResult.PaymentReference });
        //        using (HttpClient client = new HttpClient())
        //        {
        //            HttpResponseMessage response;
        //            var content = new StringContent(myJsonUpdate, Encoding.UTF8, "text/json");
        //            client.BaseAddress = new Uri(apiUrl);
        //            client.DefaultRequestHeaders.Clear();
        //            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        //            response = client.PostAsync(apiUrl, content).Result;
        //            if (response.IsSuccessStatusCode)
        //            {
        //                var data = response.Content.ReadAsStringAsync().Result;
        //                var ret = JsonConvert.DeserializeObject<BookingDetailsModel>(data);
        //                booking = ret.data;

        //            }
        //            response.EnsureSuccessStatusCode();

        //        }
        //        if (transactionResult.Message == "Fail")
        //        {
        //            return View("PaymentMade", transactionResult);
        //        }
        //        return View(booking);
        //    }
        //    catch (Exception ex)
        //    {
        //        Response.Write(ex.Message);
        //        return View(booking);
        //    }

        //}
    }
}