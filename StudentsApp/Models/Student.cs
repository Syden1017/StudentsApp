using System;
using System.Collections.Generic;

namespace StudentsApp.Models;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; }

    public string Address { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;
}
