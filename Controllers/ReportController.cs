using BiometricClockingAPI.Data;
using BiometricClockingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _context.Reports
                .Include(r => r.Employee)
                .ToListAsync();

            return Ok(reports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetReport(int id)
        {
            var report = await _context.Reports
                .Include(r => r.Employee)
                .FirstOrDefaultAsync(r => r.ReportId == id);

            if (report == null)
            {
                return NotFound("Report not found");
            }

            return Ok(report);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport([FromBody] Report report)
        {
            _context.Reports.Add(report);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReport), new { id = report.ReportId }, report);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateReport(int id, [FromBody] Report updatedReport)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                return NotFound("Report not found");
            }

            report.Title = updatedReport.Title;
            report.EmployeeId = updatedReport.EmployeeId;
            report.Description = updatedReport.Description;

            await _context.SaveChangesAsync();
            return Ok("Report updated successfully");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReport(int id)
        {
            var report = await _context.Reports.FindAsync(id);

            if (report == null)
            {
                return NotFound("Report not found");
            }

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Ok("Report deleted successfully");
        }
    }
}
