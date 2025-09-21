using B3cBonsai.Models.ViewModels;
using Microsoft.AspNetCore.Http;

namespace B3cBonsai.Utility.Services
{
    public interface IVnPayService
    {

        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
