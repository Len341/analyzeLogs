using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyzeLogs
{
    class Program
    {
        static int updates = 0;
        static void Main(string[] args)
        {
            string[] logs = getLogs(Directory.GetCurrentDirectory() + "\\Logs");
            string updateText = "";
            List<double> executionTimes = new List<double>();
            Dictionary<DateTime, Tuple<double, int>> averageExecutionByDate = new Dictionary<DateTime, Tuple<double, int>>();
            Dictionary<int, Tuple<double, int>> averageExecutionByHour = new Dictionary<int, Tuple<double, int>>();
            Dictionary<DateTime, Tuple<int, int>> updatesByDate = new Dictionary<DateTime, Tuple<int, int>>();
            Dictionary<int, Tuple<int, int>> updatesByHour = new Dictionary<int, Tuple<int, int>>();


            updateText = processLogs(logs, updateText, executionTimes);

            StringBuilder builder = new StringBuilder();
            builder.Append(data.maxExecutionTime.ToString() + "," + data.minExecutionTime + "," +
                data.averageExecutionTime.ToString().Replace(',', '.') + "," + data.totalUpdates.ToString());
            builder.AppendLine().AppendLine();
            foreach(var item in data.averageExecutionTimeByDay)
            {
                if(averageExecutionByDate.ContainsKey(item.day))
                {
                    averageExecutionByDate[item.day] = Tuple.Create(averageExecutionByDate[item.day].Item1 + item.executionTime,
                        averageExecutionByDate[item.day].Item2 + 1);
                }
                else
                {
                    averageExecutionByDate[item.day] = Tuple.Create(item.executionTime, 1);
                }
            }
            foreach(var item in data.averageExecutionTimeByHour)
            {
                if (averageExecutionByHour.ContainsKey(item.hour))
                {
                    averageExecutionByHour[item.hour] = Tuple.Create(averageExecutionByHour[item.hour].Item1 + item.executionTime,
                        averageExecutionByHour[item.hour].Item2 + 1);
                }
                else
                {
                    averageExecutionByHour[item.hour] = Tuple.Create(item.executionTime, 1);
                }
            }
            foreach(var item in data.numberOfUpdatesByDay)
            {
                if (updatesByDate.ContainsKey(item.day))
                {
                    updatesByDate[item.day] = Tuple.Create(updatesByDate[item.day].Item1 + item.updates, updatesByDate[item.day].Item2 + 1);
                }
                else
                {
                    updatesByDate[item.day] = Tuple.Create(item.updates, 1);
                }
            }
            foreach(var item in data.avgNumberOfUpdatesByHour)
            {
                if (updatesByHour.ContainsKey(item.hour))
                {
                    updatesByHour[item.hour] = Tuple.Create(updatesByHour[item.hour].Item1 + item.updates, updatesByHour[item.hour].Item2 + 1);
                }
                else
                {
                    updatesByHour[item.hour] = Tuple.Create(item.updates, 1);
                }
            }

            data.maxExecutionTime = executionTimes.Max();
            data.minExecutionTime = executionTimes.Min();
            data.averageExecutionTime = (executionTimes.Sum()) / executionTimes.Count;

            Console.WriteLine(averageExecutionByDate);
            Console.WriteLine(averageExecutionByHour);

            Console.WriteLine(data.numberOfUpdatesByDay);
            Console.WriteLine(data.avgNumberOfUpdatesByHour);

            //totalUpdates = data.totalUpdates;
            Console.ReadKey();
        }

        private static string processLogs(string[] logs, string updateText, List<double> executionTimes)
        {
            foreach (string log in logs)
            {
                string text = File.ReadAllText(log);
                if (log == logs.First())
                {
                    updateText = text;
                }
                updateText = getUpdates(updateText);

                DateTime.TryParse(text.Substring(text.IndexOf('[') + 1, 19), out DateTime starttime);
                DateTime.TryParse(text.Substring(text.LastIndexOf(']') - 19, 19), out DateTime endtime);
                double executionTime = endtime.Subtract(starttime).TotalSeconds;
                executionTimes.Add(executionTime);

                createClasses(starttime, executionTime);
            }
            return updateText;
        }

        private static void createClasses(DateTime starttime, double executionTime)
        {
            data.averageExecutionTimeByHour.Add(new executionTimeByHour()
            {
                executionTime = executionTime,
                hour = starttime.Hour
            });
            data.averageExecutionTimeByDay.Add(new executionTimeByDay()
            {
                day = starttime.Date,
                executionTime = executionTime
            });
            data.avgNumberOfUpdatesByHour.Add(new updatesByHour()
            {
                hour = starttime.Hour,
                updates = updates
            });
            data.numberOfUpdatesByDay.Add(new updatesByDay()
            {
                day = starttime.Date,
                updates = updates
            });
        }

        public static string getUpdates(string updateText)
        {
            if (updateText.IndexOf("Keywords remaining after filter:") > 0)
            {
                string firstUpdates = updateText.Substring(updateText.IndexOf("Keywords remaining after filter:"),
                    updateText.Length - updateText.IndexOf("Keywords remaining after filter:"));
                updates = Convert.ToInt32(firstUpdates.Substring(32, firstUpdates.IndexOf("\n") - 32).Trim());
                updateText = firstUpdates.Substring(32);
                data.totalUpdates += updates;
            }
            else
            {
                updates = 0;
            }

            return updateText;
        }

        protected static string[] getLogs(string path)
        {
            List<string> files = new List<string>();
            foreach(string dir in Directory.GetDirectories(path))
            {
                foreach(string file in Directory.GetFiles(dir))
                {
                    if(file.Contains("logs-short.txt"))
                    {
                        files.Add(file);
                    }
                }
            }
            return files.ToArray();
        }
    }
}
