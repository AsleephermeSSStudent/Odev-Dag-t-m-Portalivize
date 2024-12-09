using OdevDagitim.Models;

namespace OdevDagitim.Repositories
{
    public class ClassRepository : GenericRepository<Class>
    {
        public ClassRepository(AppDbContext context) : base(context, context.Classes)
        {
        }
    }
} 