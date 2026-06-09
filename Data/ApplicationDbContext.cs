using Microsoft.EntityFrameworkCore;
using Todo.Models;

namespace Todo.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext( DbContextOptions<ApplicationDbContext> options) : base(options)
        {}
        public DbSet<User> Users { get; set; }

        public DbSet<TodoTask> Tasks { get; set; }
    }
}