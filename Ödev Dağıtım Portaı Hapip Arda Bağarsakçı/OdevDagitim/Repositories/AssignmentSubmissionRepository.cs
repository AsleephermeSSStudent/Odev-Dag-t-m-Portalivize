using OdevDagitim.Models;
using Microsoft.EntityFrameworkCore;

namespace OdevDagitim.Repositories
{
    public class AssignmentSubmissionRepository : GenericRepository<AssignmentSubmission>
    {
        public AssignmentSubmissionRepository(AppDbContext context) : base(context, context.AssignmentSubmissions)
        {
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByUserAsync(int userId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.Assignment)
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.Created)
                .ToListAsync();
        }

        public async Task<IEnumerable<AssignmentSubmission>> GetSubmissionsByAssignmentAsync(int assignmentId)
        {
            return await _context.AssignmentSubmissions
                .Include(s => s.User)
                .Where(s => s.AssignmentId == assignmentId)
                .OrderByDescending(s => s.Created)
                .ToListAsync();
        }
    }
} 