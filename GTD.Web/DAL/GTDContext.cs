using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using GTD.Models;

namespace GTD.DAL
{
    public class GTDContext:DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Context> Contexts { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}