using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace StudentsApp.Models;

public partial class StudentsContext : DbContext
{
    public StudentsContext()
    {
    }

    public StudentsContext(DbContextOptions<StudentsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<StudentsSuccess> StudentsSuccesses { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=Syden1810; Database=Students; Integrated Security=true; Encrypt=false");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK_Student_StudentID");

            entity.ToTable("Student");

            entity.HasIndex(e => e.PhoneNumber, "UQ_Student_PhoneNumber").IsUnique();

            entity.Property(e => e.StudentId)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("StudentID");
            entity.Property(e => e.BirthDate).HasColumnType("date");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MiddleName).HasMaxLength(50);
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
        });

        modelBuilder.Entity<StudentsSuccess>(entity =>
        {
            entity.HasKey(e => new { e.StudentId, e.SubjectId, e.ExamDate });

            entity.ToTable("StudentsSuccess");

            entity.Property(e => e.StudentId)
                .HasMaxLength(6)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("StudentID");
            entity.Property(e => e.SubjectId)
                .HasMaxLength(9)
                .HasColumnName("SubjectID");
            entity.Property(e => e.ExamDate).HasColumnType("date");
            entity.Property(e => e.Evaluation).HasMaxLength(10);

            entity.HasOne(d => d.Student).WithMany(p => p.StudentsSuccesses)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK_StudentsSuccess_StudentID");

            entity.HasOne(d => d.Subject).WithMany(p => p.StudentsSuccesses)
                .HasForeignKey(d => d.SubjectId)
                .HasConstraintName("FK_StudentsSuccess_SubjectName");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.SubjectId).HasName("PK_Subject_SubjectID");

            entity.ToTable("Subject");

            entity.Property(e => e.SubjectId)
                .HasMaxLength(9)
                .HasColumnName("SubjectID");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
