using Bot.Models;

namespace Bot.BusinessLogic.Interfaces
{
    public interface ITeacherBySectionService
    {
        IEnumerable<TeacherBySection> Gets(string name);
    }
}
