using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rocky;
using Rocky_DataAccess.Data;
using Rocky_DataAccess.Repository.IRepository;
using Rocky_Models;
using Rocky_Models.ViewModels;
using Rocky_Ultility;

namespace Rocky.Controllers
{
    [Authorize(Roles = WC.AdminRole)]
    public class InquiryController : Controller
    {
        
        private readonly IInquiryHeaderRepository _inqHeadRepo;
        private readonly IInquiryDetailRepository _inqDelRepo;
        [BindProperty]
        public  InquiryVM InquiryVm { get; set; }
        public InquiryController(IInquiryHeaderRepository inqHeadRepo, IInquiryDetailRepository inqDelRepo)
        {
            _inqHeadRepo = inqHeadRepo;
            _inqDelRepo = inqDelRepo;
        }

        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int id)
        {
            InquiryVm = new InquiryVM()
            {
                InquiryHeader = _inqHeadRepo.FirstOrDefault(u=>u.Id == id),
                InquiryDetail = _inqDelRepo.GetAll(u=>u.InquiryHeaderId == id, includeProperties:"Product")
            };
            return View(InquiryVm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DetailsPost()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            InquiryVm.InquiryDetail = _inqDelRepo.GetAll(u =>
                u.InquiryHeaderId == InquiryVm.InquiryHeader.Id);

            foreach (var detail in InquiryVm.InquiryDetail)
            {
                ShoppingCart shoppingCart = new ShoppingCart()
                {
                    ProductId = detail.ProductId
                };
                shoppingCartList.Add(shoppingCart);
            }
            HttpContext.Session.Clear();
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            HttpContext.Session.Set(WC.SessionInquiryId, InquiryVm.InquiryHeader.Id);
            TempData[WC.Success] = "Inquiry converted successfully";

            return RedirectToAction("Index", "Cart");
        }

        [HttpPost]
        public IActionResult Delete()
        {
            InquiryHeader inquiryHeader = _inqHeadRepo.FirstOrDefault(u => u.Id == InquiryVm.InquiryHeader.Id);
            IEnumerable<InquiryDetail> inquiryDetails = _inqDelRepo.GetAll(u => u.InquiryHeaderId == InquiryVm.InquiryHeader.Id);

            _inqDelRepo.RemoveRange(inquiryDetails);
            _inqHeadRepo.Remove(inquiryHeader);
            _inqHeadRepo.Save();
            TempData[WC.Success] = "Action completed...";
            return RedirectToAction(nameof(Index));
        }
        
        #region API CALLS
        [HttpGet]
        public IActionResult GetInquiryList()
        {
            return Json(new
            {
                data = _inqHeadRepo.GetAll()
            });
        }
        #endregion
        

    }
}