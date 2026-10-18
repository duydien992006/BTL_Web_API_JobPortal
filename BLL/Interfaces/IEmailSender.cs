using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Interfaces
{
    public interface IEmailSender
    {
        // true = đã gửi thật, false = chưa cấu hình SMTP nên không gửi
        Task<bool> GuiAsync(string den, string tieuDe, string noiDungHtml);
    }
}
