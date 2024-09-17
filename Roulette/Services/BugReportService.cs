using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Models;

namespace Roulette.Services
{
    public class BugReportService
    {
        private readonly ApplicationDbContext _context;

        public BugReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<BugReport>> GetAllBugReportsAsync()
        {
            return await _context.BugReports.ToListAsync();
        }

        public async Task<BugReport?> GetBugReportByIdAsync(int id)
        {
            return await _context.BugReports.FindAsync(id);
        }

        public async Task CreateBugReportAsync(BugReport bugReport)
        {
            _context.BugReports.Add(bugReport);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBugReportAsync(BugReport bugReport)
        {
            _context.BugReports.Update(bugReport);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBugReportAsync(int id)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport != null)
            {
                _context.BugReports.Remove(bugReport);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateBugReportStatusAsync(int id, BugReportStatus newStatus)
        {
            var bugReport = await _context.BugReports.FindAsync(id);
            if (bugReport != null)
            {
                bugReport.Status = newStatus;
                bugReport.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}
