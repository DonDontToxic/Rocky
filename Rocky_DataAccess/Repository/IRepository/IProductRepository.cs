using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IProductRepository : IRepository<Product>
    {
        void Update(Product obj);

        IEnumerable<SelectListItem> GetAllDropDownList(string obj);
    }
}