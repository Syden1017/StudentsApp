using System;
using System.Collections.Generic;

namespace StudentsApp.Models;

public partial class Subject
{
    public string SubjectId { get; set; } = null!;

    public string SubjectName { get; set; } = null!;

    public int? TotalHours { get; set; }

    public virtual ICollection<StudentsSuccess> StudentsSuccesses { get; set; } = new List<StudentsSuccess>();
}
