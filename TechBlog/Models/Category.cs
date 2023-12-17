using System;
using System.Collections.Generic;

namespace TechBlog.Models;

public partial class Category
{
    public int CategoryId { get; set; }

    public string? CategoryName { get; set; }

    public string? CategorySlug { get; set; }

    public string? CategoryDesc { get; set; }

    public int? CategoryParrentId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public int? Levels { get; set; }

    public virtual ICollection<Post> Posts { get; set; } = new List<Post>();
}
