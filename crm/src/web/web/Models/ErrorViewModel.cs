using System;

namespace web.Models
{
    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(ErrorMessage);
    }
}