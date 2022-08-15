using AutoMapper;
using Bot.BusinessLogic.Interfaces;
using Bot.Common.Mapper;
using Bot.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bot.BusinessLogic.Implementations
{
    public class RunningTimeService : IRunningTimeService
    {
        DataContext _context;
        IMapper _mapper;
       // public  List<RunningTimeDTO> OutputData { get; set; }
       // List<RunningTimeDTO> IRunningTimeService.OutputData { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public RunningTimeService(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            //OutputData = outputData;
        }


        public IEnumerable<RunningTimeDTO> Gets(string name)
        {
            var runnings = _context._RunningTimeSections.Include(x => x.Section).Where(x => x.Section.Name == name).Distinct().ToList();
            var result = _mapper.Map<IEnumerable<RunningTimeDTO>>(runnings);
            return result;
        }
    }
}
