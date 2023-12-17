using System;
using System.Collections.Generic;

namespace TechBlog.Models;

public partial class Post
{
    public int PostId { get; set; }

    public string? PostName { get; set; }

    public string? PostTitle { get; set; }

    public string? PostSlug { get; set; }

    public string? PostDetail { get; set; }

    public int? CategoryId { get; set; }

    public string? PostImage { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDelete { get; set; }

    public DateTime? CreatedDate { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? ModifiedDate { get; set; }

    public int? ModifiedBy { get; set; }

    public int? LikeNumber { get; set; }

    public int? ViewNumber { get; set; }

    public bool? IsHot { get; set; }

    public int? PostStatus { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual User? ModifiedByNavigation { get; set; }
}
