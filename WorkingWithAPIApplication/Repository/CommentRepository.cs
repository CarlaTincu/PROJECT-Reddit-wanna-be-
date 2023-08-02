using WorkingWithAPIApplication.Context;
using WorkingWithAPIApplication.Contracts;

namespace WorkingWithAPIApplication.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly DapperContext _context;
        public CommentRepository(DapperContext context)
        {
            _context = context;
        }
    }
}
