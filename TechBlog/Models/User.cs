using System;
using System.Collections.Generic;

namespace TechBlog.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? FullName { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Address { get; set; }

    public int? RoleId { get; set; }

    public string? Avatar { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public virtual ICollection<Post> PostCreatedByNavigations { get; set; } = new List<Post>();

    public virtual ICollection<Post> PostModifiedByNavigations { get; set; } = new List<Post>();
}
