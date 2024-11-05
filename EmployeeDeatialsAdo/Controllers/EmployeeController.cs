using EmployeeDeatialsAdo.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace EmployeeDeatialsAdo.Controllers
{
    [ApiController]
    [Route("EmployeeDetailsADo")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployee _employee;

        public EmployeeController(IEmployee employee)
        {

            _employee = employee;

        }

        [HttpGet]
        public ActionResult GetEmployee([Required] int id)
        {
            var result = _employee.GetEmployee(id);
            return Ok(result);
        }

        [HttpPost]
        public ActionResult insertEmployee([Required] Employee employee)
        {
            var result = _employee.InsertEmployee(employee);
            return Ok(result);
        }

        [ HttpDelete]
        public ActionResult DeleteEmployee([Required] int id)
        {
            var result = _employee.DeleteEmployee(id);
            return Ok(result);
        }

        [HttpPut]
        public ActionResult PutEmployee([Required] Employee employee)
        {
            var result = _employee.UpdateEmployee(employee);
            return Ok(result);
        }
    }
}
