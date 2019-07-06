using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CalculatorApi.Interfaces
{
    public interface ICalculator
    {
        Task<int> Calculate(Func<List<int>, int> operation, List<int> numbers);
    }
}
