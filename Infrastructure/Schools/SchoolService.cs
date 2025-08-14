using Application.Features.Schools;
using Domain.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Schools;

public class SchoolService:ISchoolService
{
    private readonly ApplicationDbContext _context;

    public SchoolService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(School school)
    {
        await _context.Schools.AddAsync(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<int> UpdateAsync(School school)
    {
        _context.Schools.Update(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<int> DeleteAsync(School school)
    {
        _context.Schools.Remove(school);
        await _context.SaveChangesAsync();
        return school.Id;
    }

    public async Task<School> GetByIdAsync(int schoolId)
    {
        return await _context.Schools.Where(school => school.Id == schoolId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<School>> GetAllAsync()
    {
        return await _context.Schools.ToListAsync();
    }

    public async Task<School> GetByNameAsync(string name)
    {
        return await _context.Schools.Where(school => school.Name == name)
            .FirstOrDefaultAsync();
    }
}