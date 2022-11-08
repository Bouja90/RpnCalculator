using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreWebApplicationWithSwagger.Controllers
{
    /// <summary>
    /// RPN Calculator
    /// </summary>
    [ApiController]
    public class RpnCalculatorController : ControllerBase
    {
        public static readonly string DIVISION_IN_URL = "%2F";
        private static Dictionary<Guid, Stack<double>> dictionary = new Dictionary<Guid, Stack<double>>();
        private static Dictionary<string, Func<double, double, double?>> operatorsDictionnary = new Dictionary<string, Func<double, double, double?>>
        {
            {"+" , (a,b) => a+b},
            {"-" , (a,b) => a-b},
            {"*" , (a,b) => a*b},
            {DIVISION_IN_URL , (a,b) => {if (b != 0) return a/b; else return null; } } // "/" is detected in the uri as "%2F"
        };

        /// <summary>
        ///  List all operators
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/op")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<IEnumerable<string>> GetOperands()
        {
            return Ok(operatorsDictionnary.Keys.Select(s =>
            {
                if (s == DIVISION_IN_URL)
                    return "/";
                return s;
            }).ToList());
        }

        /// <summary>
        /// Apply an operator to a stack
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/op/{op}/stack/{stack_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost]
        public ActionResult<Stack<double>> ApplyOperandToStack(string op, Guid stack_id)
        {
            Stack<double> stack;
            if (!operatorsDictionnary.ContainsKey(op) || !dictionary.TryGetValue(stack_id, out stack))
                return NotFound();

            if (dictionary[stack_id].Count < 2)
                return BadRequest();

            double operand1 = stack.Pop();
            double operand2 = stack.Pop();
            double? result = operatorsDictionnary[op](operand2, operand1);
            if (result == null)
                return BadRequest();

            stack.Push((double)result);
            return Ok(stack);
        }

        /// <summary>
        /// Create a new stack
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/stack")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [HttpPost]
        public ActionResult<Guid> CreateStack()
        {
            Guid guid = Guid.NewGuid();
            var stack = new Stack<double>();
            dictionary.Add(guid, stack);
            return CreatedAtAction(nameof(CreateStack), guid, guid);
        }

        /// <summary>
        ///  List all stacks
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/stack")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [HttpGet]
        public ActionResult<IEnumerable<Stack<double>>> GetStacks()
        {
            var allValues = dictionary.Values.ToList();
            if (allValues.Count() == 0)
                return NoContent();
            else
                return Ok(allValues);
        }

        /// <summary>
        /// Push a new value to a stack
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/stack/{stack_id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpPost]
        public ActionResult<int> PushIntoStack(Guid stack_id, double value)
        {
            Stack<double> stack;
            if (!dictionary.TryGetValue(stack_id, out stack))
                return NotFound();

            stack.Push(value);
            return Ok(value);
        }

        /// <summary>
        ///  Get a stack
        /// </summary>
        /// <returns></returns>
        [Route("/rpn/stack/{stack_id}")]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Stack<double>> GetStack([FromRoute] Guid stack_id)
        {
            Stack<double> stack;
            if (dictionary.TryGetValue(stack_id, out stack))
                return Ok(stack);

            return NotFound();
        }

        /// <summary>
        /// Delete a stack
        /// </summary>
        /// <returns>
        [Route("/rpn/stack/{stack_id}")]
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteStack([FromRoute] Guid stack_id)
        {
            Stack<double> stack;
            if (dictionary.TryGetValue(stack_id, out stack))
            {
                dictionary.Remove(stack_id);
                return Ok();
            }

            return NotFound();
        }
    }
}
