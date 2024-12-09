using OdevDagitim.Models;
using Microsoft.EntityFrameworkCore;

namespace OdevDagitim.Repositories
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(AppDbContext context) : base(context, context.Users)
        {
        }

        public async Task<IEnumerable<User>> GetTeachersAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Teacher")
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetUsersWithClassAsync()
        {
            return await _context.Users.Include(u => u.Class).ToListAsync();
        }
    }
}
