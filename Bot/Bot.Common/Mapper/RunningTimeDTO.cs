using AutoMapper;
using Bot.Models;

namespace Bot.Common.Mapper
{
   // [AutoMap(typeof(RunningTimeSection))]
    public class RunningTimeDTO
    {
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DayOfWeek { get; set; }
        public string Location { get; set; }
        public List<RunningTimeDTO> Logs { get; set; }
    }
}
