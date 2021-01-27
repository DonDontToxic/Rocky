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