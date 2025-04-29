using System;

namespace mid_assignment.Domain.Entities;

public class Category
{
    public Guid CategoryId { get; set; }
    public required string Name { get; set; }
    public ICollection<Book>? Books { get; set; }
}
