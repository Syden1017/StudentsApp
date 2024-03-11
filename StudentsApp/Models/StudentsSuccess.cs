using System;
using System.Collections.Generic;

namespace StudentsApp.Models;

public partial class StudentsSuccess
{
    public string StudentId { get; set; } = null!;

    public string SubjectId { get; set; } = null!;

    public int Evaluation { get; set; }

    public DateTime ExamDate { get; set; }
}
