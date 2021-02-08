using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzeLogs
{
    public static class data
    {
        public static double averageExecutionTime { get; set; }
        public static double maxExecutionTime { get; set; }
        public static double minExecutionTime { get; set; }
        public static int totalUpdates { get; set; } = 0;
        public static List<executionTimeByDay> averageExecutionTimeByDay { get; set; } = new List<executionTimeByDay>();
        public static List<executionTimeByHour> averageExecutionTimeByHour { get; set; } = new List<executionTimeByHour>();
        public static List<updatesByDay> numberOfUpdatesByDay { get; set; } = new List<updatesByDay>();
        public static List<updatesByHour> avgNumberOfUpdatesByHour { get; set; } = new List<updatesByHour>();
    }
}
