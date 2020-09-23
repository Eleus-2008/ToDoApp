using Microsoft.EntityFrameworkCore;

namespace ToDoApp.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }
        
        public ApplicationContext()
        {
            Database.EnsureCreated();
        }   
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source = ToDoApp.db");
        }
    }
}