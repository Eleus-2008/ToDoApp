using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoApp.Model.Enums;

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
            optionsBuilder.UseSqlite(@"Data Source = ToDoApp.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .OwnsOne(e => e.RepeatingConditions, repeatingConditions =>
                {
                    repeatingConditions
                    .Ignore(e => e.RepeatingDaysOfWeek)
                        .Property(e => e.Type)
                        .HasConversion(new EnumToStringConverter<TypeOfRepeatTimeSpan>())
                        .HasColumnType("TEXT");
                })
                .ToTable("Tasks");

            modelBuilder.Entity<ToDoList>().ToTable("ToDoLists");
        }
    }
}