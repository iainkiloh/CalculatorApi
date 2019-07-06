using CalculatorApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculatorApi.Infrastructure.Services
{
    /// <summary>
    /// ICalculator implementation - takes in an operation and executes it
    /// </summary>
    public class Calculator : ICalculator
    {
        public Task<int> Calculate(Func<List<int>, int> operation, List<int> numbers)
        {
            return Task.FromResult(operation(numbers));
        }

    }
}
