using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace CalculatorApi.Infrastructure
{
    public class UnprocessableObjectResult : ObjectResult
    {
        public UnprocessableObjectResult(ModelStateDictionary modelState) 
            : base(new SerializableError(modelState))
        {
            if (modelState == null) { throw new ArgumentNullException(nameof(modelState)); }
            StatusCode = 422;
        }
    }
}
