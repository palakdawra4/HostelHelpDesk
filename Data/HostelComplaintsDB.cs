using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class HostelComplaintsDB : DbContext
    {
        public HostelComplaintsDB(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Student> Students { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Caretaker> Caretakers { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Hostel> Hostels { get; set; }
        public DbSet<Room> Rooms { get; set; }  
        public DbSet<TimeSlot> TimeSlots { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /*modelBuilder.Entity<Hostel>()
                .HasOne(c => c.Caretaker)
                .WithOne(h => h.Hostel)
                .HasForeignKey<Caretaker>(h => h.HostelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Hostel)
                .WithMany(h => h.Students)
                .HasForeignKey(s => s.HostelId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Student)
                .WithMany(h => h.Complaints)
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Caretaker>()
                .HasOne(c => c.Hostel)
                .WithOne(h => h.Caretaker)
                .HasForeignKey<Hostel>(h => h.CaretakerId)
                .OnDelete(DeleteBehavior.Restrict);*/

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Room)
                .WithMany()
                .HasForeignKey(c => c.RoomId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Room>()
                .HasOne(c => c.Hostel) 
                .WithMany()
                .HasForeignKey(c=>c.HostelId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(s => s.Hostel)
                .WithMany()
                .HasForeignKey(s => s.HostelId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Worker)
                .WithMany()
                .HasForeignKey(c => c.WorkerId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Caretaker)
                .WithMany()
                .HasForeignKey(c => c.CaretakerId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<Hostel>()
                .HasIndex(x => x.HostelName)
                .IsUnique();

            modelBuilder.Entity<Worker>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Complaint>()
                .HasIndex(x => x.Complaint_No)
                .IsUnique();

            modelBuilder.Entity<Caretaker>()
                .HasIndex(x => x.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(x => x.RollNo)
                .IsUnique();
        }
    }
}
