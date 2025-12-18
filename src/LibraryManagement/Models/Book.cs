using System;
using System.Collections.Generic;
using System.Text;

namespace LibraryManagement.Models
{
    public class Book
    {
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public bool IsAvailable { get; set; } = true;
    }
}
