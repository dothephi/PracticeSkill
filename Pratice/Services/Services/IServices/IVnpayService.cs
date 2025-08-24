using Model.Models.DTO.Vnpay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Services.IServices
{
    public interface IVnpayService
    {
        string CreatePaymentUrl(VnpayPaymentRequest request, string ipAddress);
    }
}
