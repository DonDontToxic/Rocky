using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IInquiryDetailRepository : IRepository<InquiryDetail>
    {
        void Update(InquiryDetail obj);

    }
}