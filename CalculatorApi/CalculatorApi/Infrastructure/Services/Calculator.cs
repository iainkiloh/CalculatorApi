using CalculatorApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculatorApi.Infrastructure.Services
{
    public class Calculator : ICalculator
    {
        public Task<int> Calculate(Func<List<int>, int> operation, List<int> numbers)
        {
            return Task.FromResult(operation(numbers));
        }

    }
}
