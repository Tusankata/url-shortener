using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UrlShortener.Application.Common.Abstractions;

public interface IExceptionHandler<in TException> : IExceptionHandler
    where TException : Exception
{
    Task HandleAsync(HttpContext context, TException exception);
}
public interface IExceptionHandler
{
    Task HandleAsync(HttpContext context, Exception exception);
}
