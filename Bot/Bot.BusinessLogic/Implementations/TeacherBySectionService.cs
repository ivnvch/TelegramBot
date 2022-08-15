using Bot.BusinessLogic.Interfaces;
using Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Bot.BusinessLogic.Implementations
{
    public class TeacherBySectionService : ITeacherBySectionService
    {
        DataContext _context;
        public TeacherBySectionService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<TeacherBySection> Gets(string name)
        {
            _context.Database.ExecuteSqlRaw(@"ALTER VIEW View_TeacherBySection AS
                SELECT s.Name AS SectionName, t.FullName AS TeacherFullName, t.MobilePhone AS TeacherMobilePhone 
                FROM Sections s
                JOIN Teachers t ON s.Id = t.SectionId");
            _context.SaveChanges();

            return _context._TeacherBySections.Where(x => x.SectionName == name).ToList();

        }

    }
}
