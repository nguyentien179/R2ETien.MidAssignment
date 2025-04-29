using System;
using mid_assignment.Domain.Enum;

namespace mid_assignment.Application.Common.Models;

public class SortDescriptor
{
    public string? PropertyName { get; set; }
    public SortDirection Direction { get; set; } = SortDirection.ASCENDING;
}
