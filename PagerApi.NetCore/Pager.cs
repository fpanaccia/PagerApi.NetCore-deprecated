using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PagerApi.NetCore
{
    public static class Pager
    {
        public static async Task<Response<IList<T>>> ToListPagedAsync<T>(this IQueryable<T> source)
        {
            var response = new Response<IList<T>>
            {
                Total = source.Count()
            };

            if (System.Web.HttpContext.Current == null)
            {
                response.Result = await source.ToListAsync();
            }
            else
            {
                var ctxOffset = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "offset");
                var ctxSize = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "size");

                if (ctxOffset != null && ctxOffset.Any() && ctxSize != null && ctxSize.Any())
                {
                    response.Result = await source.Skip(int.Parse(ctxOffset.Single().Value)).Take(int.Parse(ctxSize.Single().Value)).ToListAsync();
                }
                else
                {
                    response.Result = await source.ToListAsync();
                }
            }

            return response;
        }

        public static Response<IList<T>> ToListPaged<T>(this IQueryable<T> source)
        {
            var response = new Response<IList<T>>
            {
                Total = source.Count()
            };

            if (System.Web.HttpContext.Current == null)
            {
                response.Result = source.ToList();
            }
            else
            {
                var ctxOffset = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "offset");
                var ctxSize = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "size");

                if (ctxOffset != null && ctxOffset.Any() && ctxSize != null && ctxSize.Any())
                {
                    response.Result = source.Skip(int.Parse(ctxOffset.Single().Value)).Take(int.Parse(ctxSize.Single().Value)).ToList();
                }
                else
                {
                    response.Result = source.ToList();
                }
            }

            return response;
        }
    }
}
