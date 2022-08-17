using Bot.Common.Mapper;

namespace Bot.BusinessLogic.Interfaces
{
    public interface ISectionService
    {
        IEnumerable<SectionDTO> Gets();
        string Title { get; }
        SectionDTO Get(string Names);
        SectionDTO DTO(string title);
    }
}
