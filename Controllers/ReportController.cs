using System.Security.Claims;
using BiometricClockingAPI.Data;
using BiometricClockingAPI.DTOs.Report;
using BiometricClockingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiometricClockingAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReportController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ReportController(ApplicationDbContext context)
    {
        _context = context;
    }

    // POST: api/Report
    // Employee or HR creates a report for themselves.
    [HttpPost]
    public async Task<IActionResult> CreateReport(
        [FromBody] CreateReportDto request)
    {
        var employeeId = GetCurrentEmployeeId();

        if (employeeId is null)
        {
            return Unauthorized(new
            {
                message = "The employee ID was not found in the token."
            });
        }

        var employeeExists = await _context.Employees
            .AnyAsync(employee =>
                employee.EmployeeId == employeeId.Value);

        if (!employeeExists)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        var report = new Report
        {
            Title = request.Title.Trim(),
            Description = request.Description.Trim(),
            EmployeeId = employeeId.Value,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _context.Reports.Add(report);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetReportById),
            new { id = report.ReportId },
            new
            {
                report.ReportId,
                report.Title,
                report.EmployeeId,
                report.Description,
                report.Status,
                report.CreatedAt
            }
        );
    }

    // GET: api/Report/my
    // Employee views their own reports.
    [HttpGet("my")]
    public async Task<IActionResult> GetMyReports()
    {
        var employeeId = GetCurrentEmployeeId();

        if (employeeId is null)
        {
            return Unauthorized(new
            {
                message = "The employee ID was not found in the token."
            });
        }

        var reports = await _context.Reports
            .AsNoTracking()
            .Where(report =>
                report.EmployeeId == employeeId.Value)
            .OrderByDescending(report =>
                report.CreatedAt)
            .Select(report => new
            {
                report.ReportId,
                report.Title,
                report.EmployeeId,
                report.Description,
                report.Status,
                report.CreatedAt
            })
            .ToListAsync();

        return Ok(reports);
    }

    // GET: api/Report/5
    // Employee can view their own report.
    // HR can view any report.
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetReportById(int id)
    {
        var employeeId = GetCurrentEmployeeId();

        if (employeeId is null)
        {
            return Unauthorized(new
            {
                message = "The employee ID was not found in the token."
            });
        }

        var report = await _context.Reports
            .AsNoTracking()
            .FirstOrDefaultAsync(report =>
                report.ReportId == id);

        if (report is null)
        {
            return NotFound(new
            {
                message = "Report not found."
            });
        }

        var isHr = User.IsInRole("HR");

        if (!isHr && report.EmployeeId != employeeId.Value)
        {
            return Forbid();
        }

        return Ok(new
        {
            report.ReportId,
            report.Title,
            report.EmployeeId,
            report.Description,
            report.Status,
            report.CreatedAt
        });
    }

    // GET: api/Report
    // HR views all reports.
    [Authorize(Roles = "HR")]
    [HttpGet]
    public async Task<IActionResult> GetAllReports()
    {
        var reports = await _context.Reports
            .AsNoTracking()
            .Join(
                _context.Employees,
                report => report.EmployeeId,
                employee => employee.EmployeeId,
                (report, employee) => new
                {
                    report.ReportId,
                    report.Title,
                    report.EmployeeId,
                    EmployeeName =
                        employee.Name + " " + employee.Surname,
                    employee.EmailAddress,
                    report.Description,
                    report.Status,
                    report.CreatedAt
                }
            )
            .OrderByDescending(report =>
                report.CreatedAt)
            .ToListAsync();

        return Ok(reports);
    }

    // GET: api/Report/employee/2
    // HR views reports submitted by one employee.
    [Authorize(Roles = "HR")]
    [HttpGet("employee/{employeeId:int}")]
    public async Task<IActionResult> GetReportsByEmployee(
        int employeeId)
    {
        var employeeExists = await _context.Employees
            .AnyAsync(employee =>
                employee.EmployeeId == employeeId);

        if (!employeeExists)
        {
            return NotFound(new
            {
                message = "Employee not found."
            });
        }

        var reports = await _context.Reports
            .AsNoTracking()
            .Where(report =>
                report.EmployeeId == employeeId)
            .OrderByDescending(report =>
                report.CreatedAt)
            .Select(report => new
            {
                report.ReportId,
                report.Title,
                report.EmployeeId,
                report.Description,
                report.Status,
                report.CreatedAt
            })
            .ToListAsync();

        return Ok(reports);
    }

    // PUT: api/Report/5/status
    // HR updates the report status.
    [Authorize(Roles = "HR")]
    [HttpPut("{id:int}/status")]
    public async Task<IActionResult> UpdateStatus(
        int id,
        [FromBody] UpdateReportStatusDto request)
    {
        var report = await _context.Reports
            .FindAsync(id);

        if (report is null)
        {
            return NotFound(new
            {
                message = "Report not found."
            });
        }

        report.Status = request.Status;

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Report status updated successfully.",
            report.ReportId,
            report.Status
        });
    }

    // DELETE: api/Report/5
    // HR deletes a report.
    [Authorize(Roles = "HR")]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteReport(int id)
    {
        var report = await _context.Reports
            .FindAsync(id);

        if (report is null)
        {
            return NotFound(new
            {
                message = "Report not found."
            });
        }

        _context.Reports.Remove(report);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Report deleted successfully."
        });
    }

    private int? GetCurrentEmployeeId()
    {
        var employeeIdClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return int.TryParse(
            employeeIdClaim,
            out var employeeId)
                ? employeeId
                : null;
    }
}