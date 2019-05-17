using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace PagerApi.NetCore
{
    public static class Pager
    {
        private static readonly FieldInfo QueryCompilerField = typeof(EntityQueryProvider).GetTypeInfo().DeclaredFields.First(x => x.Name == "_queryCompiler");
        private static readonly TypeInfo QueryCompilerTypeInfo = typeof(QueryCompiler).GetTypeInfo();
        private static readonly FieldInfo DataBaseField = QueryCompilerTypeInfo.DeclaredFields.Single(x => x.Name == "_database");
        private static readonly PropertyInfo DatabaseDependenciesField = typeof(Database).GetTypeInfo().DeclaredProperties.Single(x => x.Name == "Dependencies");

        public static async Task<Response<IList<TSource>>> ToListPagedAsync<TSource>(this IQueryable<TSource> source)
            where TSource : class
        {
            var response = new Response<IList<TSource>>
            {
                Total = source.Count(),
                Result = await Resolve(source).ToListAsync()
            };

            return response;
        }

        public static Response<IList<TSource>> ToListPaged<TSource>(this IQueryable<TSource> source)
            where TSource : class
        {
            var response = new Response<IList<TSource>>
            {
                Total = source.Count(),
                Result = Resolve(source).ToList()
            };

            return response;
        }

        public static async Task<Response<IList<TSource>>> ToListPagedAsync<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> orderBy, bool descending = false)
            where TSource : class
        {
            var response = new Response<IList<TSource>>
            {
                Total = source.Count(),
                Result = await Resolve(source, orderBy, descending).ToListAsync()
            };
            
            return response;
        }

        public static Response<IList<TSource>> ToListPaged<TSource, TKey>(this IQueryable<TSource> source, Expression<Func<TSource, TKey>> orderBy, bool descending = false)
            where TSource : class
        {
            var response = new Response<IList<TSource>>
            {
                Total = source.Count(),
                Result = Resolve(source, orderBy, descending).ToList()
            };

            return response;
        }

        private static IQueryable<TSource> Resolve<TSource>(IQueryable<TSource> source)
            where TSource : class
        {
            var query = ResolveQuery(source);
            var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(source.Provider);
            var database = DataBaseField.GetValue(queryCompiler);

            if (database is RelationalDatabase)
            {
                var entityType = typeof(TSource);
                var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
                var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
                var entity = queryCompilationContext.Model.FindEntityType(entityType);

                if (entity != null)
                {
                    var pk = entity.FindPrimaryKey().Properties.Select(x => x.Name).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(pk))
                    {
                        return query.OrderBy(pk);
                    }
                }
            }

            return query;
        }

        private static IQueryable<TSource> Resolve<TSource, TKey>(IQueryable<TSource> source, Expression<Func<TSource, TKey>> orderBy = null, bool descending = false)
            where TSource : class
        {
            var query = ResolveQuery(source);
            if (orderBy != null)
            {
                if (descending)
                {
                    return query.OrderByDescending(orderBy);
                }
                else
                {
                    return query.OrderBy(orderBy);
                }
            }
            else
            {
                var queryCompiler = (QueryCompiler)QueryCompilerField.GetValue(source.Provider);
                var database = DataBaseField.GetValue(queryCompiler);

                if (database is RelationalDatabase)
                {
                    var entityType = typeof(TSource);
                    var databaseDependencies = (DatabaseDependencies)DatabaseDependenciesField.GetValue(database);
                    var queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(false);
                    var entity = queryCompilationContext.Model.FindEntityType(entityType);

                    if(entity != null)
                    {
                        var pk = entity.FindPrimaryKey().Properties.Select(x => x.Name).FirstOrDefault();
                        if (!string.IsNullOrWhiteSpace(pk))
                        {
                            return query.OrderBy(pk);
                        }
                    }
                }

                return query;
            }
        }

        private static IQueryable<TSource> ResolveQuery<TSource>(IQueryable<TSource> source) where TSource : class
        {
            var result = source;

            if (System.Web.HttpContext.Current != null)
            {
                var ctxOffset = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "offset");
                var ctxSize = System.Web.HttpContext.Current.Request.Headers.Where(x => x.Key == "size");

                if (ctxOffset != null && ctxOffset.Any() && ctxSize != null && ctxSize.Any())
                {
                    result = source.Skip(int.Parse(ctxOffset.Single().Value)).Take(int.Parse(ctxSize.Single().Value));
                }
            }

            return result;
        }
    }
}
