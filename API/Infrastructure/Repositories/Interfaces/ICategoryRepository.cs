using System;
using mid_assignment.Domain.Entities;

namespace mid_assignment.Infrastructure.Repositories.Interfaces;

public interface ICategoryRepository : IRepository<Category>
{
    Task<bool> CategoryNameExistAsync(string categoryName);
}
