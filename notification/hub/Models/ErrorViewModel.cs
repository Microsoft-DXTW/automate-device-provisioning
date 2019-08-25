using System;

namespace hub.Models
{
    public class ErrorViewModel
    {
        public string ErrorMessage { get; set; }

        public bool ShowErrorMessage => !string.IsNullOrEmpty(ErrorMessage);
    }
}