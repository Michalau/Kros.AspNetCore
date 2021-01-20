﻿using FluentValidation;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace Kros.ProblemDetails.Extensions
{
    /// <summary>
    /// Hellang problem details service extensions.
    /// </summary>
    public static class ProblemDetailsExtensions
    {
        /// <summary>
        /// Registers Hellang problem details to IoC container.
        /// </summary>
        /// <param name="services">IoC container.</param>
        /// <param name="environment">Current environment.</param>
        /// <param name="configAction">Additional action for configuring Hellang problem details.</param>
        public static IServiceCollection AddCustomProblemDetails(
            this IServiceCollection services,
            IWebHostEnvironment environment,
            Action<ProblemDetailsOptions> configAction = null)
            =>  services.AddProblemDetails(p =>
            {
                p.IncludeExceptionDetails = (context, ex) => IncludeExceptionDetails(environment, ex);
                p.SourceCodeLineCount = 0;

                configAction?.Invoke(p);
            });

        /// <summary>
        /// Map <see cref="ValidationException"/> to <see cref="ValidationProblemDetails"/>
        /// </summary>
        /// <param name="options">Problem details options</param>
        public static void MapFluentValidationException(this ProblemDetailsOptions options)
            => options.Map<ValidationException>((ex) => new ValidationProblemDetails(ex.Errors, StatusCodes.Status400BadRequest));

        private static bool IncludeExceptionDetails(IWebHostEnvironment environment, Exception ex)
            => environment.IsTestOrDevelopment() && !IsExceptionWithoutExceptionDetails(ex) ? true : false;

        private static bool IsExceptionWithoutExceptionDetails(Exception ex)
            => ex.GetType() == typeof(ValidationException);
    }
}
