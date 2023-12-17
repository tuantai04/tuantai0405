using System;
using System.Collections.Generic;

namespace TechBlog.Models;

public partial class Comment
{
    public int CommentId { get; set; }

    public int? PostId { get; set; }

    public string? CommentContent { get; set; }

    public int? CommentLevels { get; set; }

    public int? CommentParrentId { get; set; }

    public DateTime? CommentDate { get; set; }

    public string? FullName { get; set; }

    public virtual Post? Post { get; set; }
}
