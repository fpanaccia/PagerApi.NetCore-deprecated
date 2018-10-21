using System;
using System.Collections.Generic;
using System.Text;

namespace PagerApi.NetCore
{
    public static class PagingExtensions
    {
        public static Response<U> Cast<T, U>(this Response<T> Response, Func<T, U> mapper)
        {
            return new Response<U>
            {
                Total = Response.Total,
                Result = mapper(Response.Result),
                Errors = Response.Errors,
                Success = Response.Success
            };
        }
    }
}
