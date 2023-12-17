using System;
using System.Collections.Generic;

namespace TechBlog.Models;

public partial class Contact
{
    public int ContactId { get; set; }

    public string? FullName { get; set; }

    public string? Gmail { get; set; }

    public string? PhoneNumber { get; set; }

    public string? ContactDesc { get; set; }

    public string? ContactDetail { get; set; }

    public int? Status { get; set; }

    public bool? IsDelete { get; set; }
}
