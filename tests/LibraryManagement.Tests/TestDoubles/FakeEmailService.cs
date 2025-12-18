using LibraryManagement.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Tests.TestDoubles
{
    public class FakeEmailService : IEmailService
    {
        public List<EmailRecord> SentEmails { get; } = new();
        public int EmailsSentCount => SentEmails.Count;

        public void SendEmail(string to, string subject, string body)
        {
            SentEmails.Add(new EmailRecord(to, subject, body));
        }
    }

    public record EmailRecord(string To, string Subject, string Body);
}