using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
    }
}
