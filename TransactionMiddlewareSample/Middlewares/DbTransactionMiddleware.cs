using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Data;
using System.Threading.Tasks;
using TransactionMiddlewareSample.Data;

namespace TransactionMiddlewareSample.Middlewares
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TransactionAttribute : Attribute
    {
    }

    public class DbTransactionMiddleware
    {
        private readonly RequestDelegate _next;

        public DbTransactionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext, SqlConnectionProvider connectionProvider)
        {
            // For HTTP GET opening transaction is not required
            if (httpContext.Request.Method.Equals("GET", StringComparison.CurrentCultureIgnoreCase))
            {
                await _next(httpContext);
                return;
            }

            // If action is not decorated with TransactionAttribute then skip opening transaction
            var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            var attribute = endpoint?.Metadata.GetMetadata<TransactionAttribute>();
            if (attribute == null)
            {
                await _next(httpContext);
                return;
            }

            IDbTransaction transaction = null;

            try
            {
                transaction = connectionProvider.CreateTransaction();

                await _next(httpContext);

                transaction.Commit();
            }
            finally
            {
                transaction?.Dispose();
            }
        }
    }

    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseDbTransaction(this IApplicationBuilder app)
            => app.UseMiddleware<DbTransactionMiddleware>();
    }
}