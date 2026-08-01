using BiometricClockingAPI.Data;
using BiometricClockingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee([FromBody] Employee employee)
        {
            // Hash password before saving
            if (!string.IsNullOrWhiteSpace(employee.Password))
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(employee.Password);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(SearchEmployee), new { id = employee.EmployeeId }, employee);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();
            return Ok(employees);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchEmployeeByQuery([FromQuery] int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid employee id");
            }

            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> SearchEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound("Employee not found");
            }

            return Ok(employee);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditEmployee(int id, [FromBody] Employee updatedEmployee)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            employee.Name = updatedEmployee.Name;
            employee.Surname = updatedEmployee.Surname;
            employee.Role = updatedEmployee.Role;
            // If a new password was provided, hash it; otherwise keep current password
            if (!string.IsNullOrWhiteSpace(updatedEmployee.Password))
            {
                employee.Password = BCrypt.Net.BCrypt.HashPassword(updatedEmployee.Password);
            }
            employee.EmailAddress = updatedEmployee.EmailAddress;

            await _context.SaveChangesAsync();
            return Ok("Employee updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _context.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            return Ok("Employee deleted successfully");
        }
    }
}
