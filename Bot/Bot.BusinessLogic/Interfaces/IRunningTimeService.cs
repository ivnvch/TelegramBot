using Bot.Common.Mapper;

namespace Bot.BusinessLogic.Interfaces
{
    public interface IRunningTimeService
    {
        public IEnumerable<RunningTimeDTO> Gets(string name);
        //List<RunningTimeDTO> OutputData {get; set;}
    }
}
