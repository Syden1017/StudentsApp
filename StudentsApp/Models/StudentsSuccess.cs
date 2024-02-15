using System;

namespace StudentsApp.Models;

public partial class StudentsSuccess
{
    public string StudentId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public string Evaluation { get; set; } = null!;

    public DateTime ExamDate { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
