using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rocky_Models;

namespace Rocky_DataAccess.Repository.IRepository
{
    public interface IInquiryHeaderRepository : IRepository<InquiryHeader>
    {
        void Update(InquiryHeader obj);

    }
}