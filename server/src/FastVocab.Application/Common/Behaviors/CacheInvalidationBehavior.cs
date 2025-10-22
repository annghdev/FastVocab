﻿using FastVocab.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastVocab.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that invalidate caches for ICacheInvalidatorRequest requests
/// </summary>
public class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly ICacheService _cacheService;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CacheInvalidationBehavior(ICacheService cacheService, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        if (request is not ICacheInvalidatorRequest invalidator)
        {
            return response;
        }

        // Trường hợp response là Result hoặc Result<T>, chỉ hủy cache khi thành công
        bool success = true;

        var type = typeof(TResponse);
        if (type.IsGenericType && type.GetGenericTypeDefinition().Name.StartsWith("Result"))
        {
            var isSuccessProp = type.GetProperty("IsSuccess");
            if (isSuccessProp != null)
                success = (bool)(isSuccessProp.GetValue(response) ?? false);
        }
        else if (type.Name == "Result")
        {
            var isSuccessProp = type.GetProperty("IsSuccess");
            if (isSuccessProp != null)
                success = (bool)(isSuccessProp.GetValue(response) ?? false);
        }

        if (success)
        {
            foreach (var key in invalidator.CacheKeysToInvalidate)
            {
                _logger.LogInformation("remove cached for key: {key}", key);
                await _cacheService.RemoveAsync(key, cancellationToken);
            }
        }

        return response;
    }
}
