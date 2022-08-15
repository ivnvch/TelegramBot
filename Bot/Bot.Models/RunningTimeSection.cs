using System.ComponentModel.DataAnnotations;

namespace Bot.Models
{
    public class RunningTimeSection
    {
        [Key]
        public int Id { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string DayOfWeek { get; set; }
        public string Location { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }

    }
}
