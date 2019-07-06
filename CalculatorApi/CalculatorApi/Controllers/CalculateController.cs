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
    /// <summary>
    /// class scoped Attributes to ensure any unhandled exceptions are caught by our exception filter
    /// declare version number and base route for controller class
    /// </summary>
    [UnhandledExceptionFilter]
    [ApiController] //required for SwaggerUI
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/calculate")]
    public class CalculateController : ControllerBase
    {
        private readonly ICalculator _calculator;

        /// <summary>
        /// constructor with ICalculator implementation injection
        /// </summary>
        /// <param name="calcuator"></param>
        public CalculateController(ICalculator calcuator)
        {
            _calculator = calcuator;
        }

         /// <summary>
         /// Route which accepts list of delimited Integers, the delimiter, and the Mathematical operation
         /// to be performed
         /// </summary>
         /// <param name="numbers"></param>
         /// <param name="delimiter"></param>
         /// <param name="calcType"></param>
         /// <returns></returns> 
        [HttpGet]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet]
        [HttpHead]
        public async Task<IActionResult> Get([FromQuery]string numbers, char delimiter, CalculationType calcType)
        {
            //declare list of ints which we will use as input for the ICalculator's 'Calculate' method
            List<int> numbersList;
            //convert to list of int32 - throw BadRequest error if conversion fails
            try
            {
                numbersList = numbers.Split(delimiter).Select(Int32.Parse).ToList();
            }
            catch
            {   //unable to parse the numbers input - throw BadRequest 400 error
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

        /// <summary>
        /// Available actions on this controller
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        public IActionResult GetOptions()
        {
            Response.Headers.Add("Allow", "GET, HEAD, OPTIONS");
            return Ok();
        }

    }
}
