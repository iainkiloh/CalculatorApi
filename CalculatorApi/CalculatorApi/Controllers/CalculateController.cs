using CalculatorApi.Enumerations;
using CalculatorApi.Filters;
using CalculatorApi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalculatorApi.Controllers
{
    [UnhandledExceptionFilter]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/calculate")]
    public class CalculateController : ControllerBase
    {
        private ICalculator _calculator;
        public CalculateController(ICalculator calcuator)
        {
            _calculator = calcuator;
        }

       
        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> Get([FromQuery]string numbers, char delimiter, CalculationType calcType) // Get([FromQuery] string number, char delimiter, CalculationType calcType)
        {
            
            List<int> numbersList;
            //convert to list of int32 - throw BadRequest error if conversion fails
            try
            {
                numbersList = numbers.Split(delimiter).Select(Int32.Parse).ToList();
            }
            catch
            {
                //valid but not 
                return BadRequest("Unable to proces the request, ensure you supply a list of delimited numbers, and the correct delimiter");
            }

            //verify at least 2 numbers in the list
            if(numbersList.Count < 2)
            {
                return BadRequest("at least 2 numbers are required");
            }

            //perform the calculation based on the requested operation
            int result = 0;
            switch(calcType) {
                case CalculationType.Add:
                    result = await _calculator.Calculate((x) => x.Sum(), numbersList);
                    break;
                case CalculationType.Multiply:
                    result = await _calculator.Calculate((x) => x.Aggregate((a,b) => a * b), numbersList);
                    break;
                case CalculationType.Subtract:
                    result = await _calculator.Calculate((x) => x.Aggregate((a, b) => a - b), numbersList);
                    break;
            }
            
            return Ok(result);
        }

        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS");
            return Ok();
        }


    }
}
