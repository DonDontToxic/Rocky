using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Rocky;
using Rocky.Data;
using Rocky.Models;
using Rocky.Models.ViewModels;
using Rocky.Utility;

namespace Rocky.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IEmailSender _emailSender;
        [BindProperty]
        public ProductUserVM ProductUserVm { get; set; }
    

        public CartController(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment, IEmailSender emailSender)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _emailSender = emailSender ;
        }
        
        public IActionResult Index()
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
                && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                // session exsist
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>
                    (WC.SessionCart).ToList();
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));
            return View(prodList);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public IActionResult IndexPost()
        {

            return RedirectToAction(nameof(Summary));
        }
        
  
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            // var userId  = User.FindFirstValue(ClaimTypes.Name);
            
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
               && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                // session exsist
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>
                    (WC.SessionCart).ToList();
            }

            List<int> prodInCart = shoppingCartList.Select(i => i.ProductId).ToList();
            IEnumerable<Product> prodList = _db.Product.Where(u => prodInCart.Contains(u.Id));

            ProductUserVm = new ProductUserVM()
            {
                ApplicationUser = _db.ApplicationUser.FirstOrDefault(u => u.Id == claim.Value),
                ProductList = prodList.ToList()
            };
            return View(ProductUserVm);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
        public async Task<IActionResult> SummaryPost(ProductUserVM ProductUserVm)
        {
            var PathToTemplate = _webHostEnvironment.WebRootPath + Path.DirectorySeparatorChar.ToString()
                +"templates"+ Path.DirectorySeparatorChar.ToString()+
                "Inquiry.html";
            var subject = "New Inquiry";
            string HtmlBody = "";
            using (StreamReader sr = System.IO.File.OpenText(PathToTemplate))
            {
                HtmlBody = sr.ReadToEnd();
            }

            StringBuilder productListSB = new StringBuilder();
            foreach (var prod in ProductUserVm.ProductList)
            {
                productListSB.Append(
                    $" - Name: {prod.Name} <span style = 'font-size: 14px;'> (ID: {prod.Id})</span><b/>");
            }

            string messageBody = string.Format(HtmlBody,
                ProductUserVm.ApplicationUser.FullName,
                ProductUserVm.ApplicationUser.Email,
                ProductUserVm.ApplicationUser.PhoneNumber,
                productListSB.ToString());
            
            await _emailSender.SendEmailAsync(WC.Email_Admin, subject, messageBody);
            
            return RedirectToAction(nameof(InquiryConfirmation));
        }
        public IActionResult InquiryConfirmation()
        {
            HttpContext.Session.Clear();
            
            return View();
        }

        public IActionResult Remove(int id)
        {
            List<ShoppingCart> shoppingCartList = new List<ShoppingCart>();
            if(HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart) != null
               && HttpContext.Session.Get<IEnumerable<ShoppingCart>>(WC.SessionCart).Count() > 0)
            {
                // session exsist
                shoppingCartList = HttpContext.Session.Get<IEnumerable<ShoppingCart>>
                    (WC.SessionCart).ToList();
            }
            shoppingCartList.Remove(shoppingCartList.FirstOrDefault(u => u.ProductId == id));
            HttpContext.Session.Set(WC.SessionCart, shoppingCartList);
            return RedirectToAction(nameof(Index));        
        }
    }


}