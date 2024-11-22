using B3cBonsai.Models.ViewModels;

namespace B3cBonsaiWeb.Services
{
    public interface IVnPayService
    {

        string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model);
        VnPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
}
