using OdevDagitim.Models;
using Microsoft.EntityFrameworkCore;

namespace OdevDagitim.Repositories
{
    public class AssignmentRepository : GenericRepository<Assignment>
    {
        public AssignmentRepository(AppDbContext context) : base(context, context.Assignments)
        {
        }


        public async Task<List<Assignment>> GetMyList(int id)
        {
            return await _context.Assignments.
                Include(a=>a.Teacher).Include(a=>a.Submissions).Include(a=>a.Class)
                .Where(x => x.TeacherId == id).ToListAsync();
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsWithDetailsAsync()
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Class)
                .OrderByDescending(a => a.Created)
                .ToListAsync();
        }

        public async Task<Assignment> GetAssignmentWithDetailsAsync(int id)
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Include(a => a.Class)
                .Include(a => a.Submissions)
                    .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Assignment>> GetAssignmentsByClassAsync(int classId)
        {
            return await _context.Assignments
                .Include(a => a.Teacher)
                .Where(a => a.ClassId == classId)
                .OrderByDescending(a => a.Created)
                .ToListAsync();
        }
    }
} 