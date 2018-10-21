using System;
using System.Collections.Generic;
using System.Text;

namespace PagerApi.NetCore
{
    public class Response<T>
    {
        public bool Success { get; set; } = true;
        public long Total { get; set; }
        public T Result { get; set; }
        public List<ErrorMessage> Errors { get; set; } = new List<ErrorMessage>();
    }
}
