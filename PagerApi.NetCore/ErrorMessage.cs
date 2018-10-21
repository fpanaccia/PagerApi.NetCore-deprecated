using System;
using System.Collections.Generic;
using System.Text;

namespace PagerApi.NetCore
{
    public class ErrorMessage
    {
        public ErrorMessage(string error)
        {
            IsError = true;
            _Text = error;
        }

        public ErrorMessage(string text, bool isError)
        {
            IsError = isError;
            _Text = text;
        }

        public ErrorMessage(Exception ex)
        {
            IsError = true;
            Exception = ex;
        }

        private string _Text { get; set; }

        public string Text
        {
            get { return !string.IsNullOrEmpty(_Text) ? _Text : $"[{Exception.GetType().FullName}] {Exception.Message}"; }
            set { _Text = value; }
        }

        public Exception Exception { get; set; }

        public bool IsError { get; private set; }
    }
}
