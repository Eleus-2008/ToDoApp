using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ToDoApp.Model.Enums;
using ToDoApp.Model.Models;

namespace ToDoApp.Model
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Task> Tasks { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AccessToken> Tokens { get; set; }

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

            modelBuilder.Entity<ToDoList>()
                .ToTable("ToDoLists");

            modelBuilder.Entity<User>()
                .ToTable("Users");


            modelBuilder.Entity<AccessToken>()
                .ToTable("Tokens")
                .HasOne(t => t.User)
                .WithOne(u => u.Token)
                .HasForeignKey<AccessToken>(t => t.UserId);
        }
    }
}