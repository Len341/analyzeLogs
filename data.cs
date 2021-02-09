using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzeLogs
{
    public class data
    {
        public double averageExecutionTime { get; set; }
        public double maxExecutionTime { get; set; }
        public double minExecutionTime { get; set; }
        public int totalUpdates { get; set; }
        public List<executionTimeByDay> averageExecutionTimeByDay { get; set; } = new List<executionTimeByDay>();
        public List<executionTimeByHour> averageExecutionTimeByHour { get; set; } = new List<executionTimeByHour>();
        public List<updatesByDay> numberOfUpdatesByDay { get; set; } = new List<updatesByDay>();
        public List<updatesByHour> avgNumberOfUpdatesByHour { get; set; } = new List<updatesByHour>();
    }
}
