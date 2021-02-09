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
        static int totalUpdates = 0;
        public static data data = new data();
        public static List<double> executionTimes = new List<double>();
        static void Main(string[] args)
        {
            StringBuilder builder = getData();

            Console.WriteLine(builder.ToString());
            Console.ReadKey();
        }

        private static StringBuilder getData()
        {
            string[] logs = getLogs(Directory.GetCurrentDirectory() + "\\Logs");
            string updateText = "";
            Dictionary<string, Tuple<double, int>> averageExecutionByDate = new Dictionary<string, Tuple<double, int>>();
            Dictionary<int, Tuple<double, int>> averageExecutionByHour = new Dictionary<int, Tuple<double, int>>();
            Dictionary<string, Tuple<int>> updatesByDate = new Dictionary<string, Tuple<int>>();
            Dictionary<int, Tuple<int, int>> updatesByHour = new Dictionary<int, Tuple<int, int>>();
            updateText = processLogs(logs, updateText);

            getAverages(averageExecutionByDate, averageExecutionByHour, updatesByDate, updatesByHour);

            data.maxExecutionTime = executionTimes.Max();
            data.minExecutionTime = executionTimes.Min();
            data.averageExecutionTime = (executionTimes.Sum()) / executionTimes.Count;

            StringBuilder builder = new StringBuilder();
            builder.Append("max Executiontime, min Executiontime, average Executiontime, total Updates made\n");
            builder.Append(data.maxExecutionTime.ToString() + "," + data.minExecutionTime + "," +
                data.averageExecutionTime.ToString().Replace(',', '.') + "," + totalUpdates.ToString());
            builder.AppendLine().AppendLine();

            generateStringData(averageExecutionByDate, averageExecutionByHour, updatesByDate, updatesByHour, builder);
            return builder;
        }

        private static void generateStringData(Dictionary<string, Tuple<double, int>> averageExecutionByDate,
            Dictionary<int, Tuple<double, int>> averageExecutionByHour, Dictionary<string, Tuple<int>> updatesByDate, Dictionary<int, Tuple<int, int>> updatesByHour, StringBuilder builder)
        {
            builder.Append("Date, Average execution time for this day, updates made this day\n");
            foreach (var item in averageExecutionByDate)
            {
                builder.Append(item.Key.ToString() + "," + (item.Value.Item1 / item.Value.Item2).ToString().Replace(',','.'));
                foreach (var updateItem in updatesByDate)
                {
                    if (updateItem.Key == item.Key)
                    {
                        builder.Append("," + updateItem.Value.Item1);
                    }
                }
                builder.AppendLine();
            }

            builder.AppendLine().AppendLine();

            builder.Append("The following hour ranges includes averages for all days").AppendLine().AppendLine();

            builder.Append("Hour, Average execution time for this hour, Average Updates made this hour\n");
            foreach (var item in averageExecutionByHour)
            {
                if (item.Key == 24)
                {
                    builder.Append(item.Key + " - 1" + "," + (item.Value.Item1 / item.Value.Item2).ToString().Replace(',','.'));
                }
                else
                {
                    builder.Append(item.Key + " - " + (item.Key + 1).ToString() + "," + (item.Value.Item1 / item.Value.Item2).ToString().Replace(',', '.'));
                }
                foreach (var updatehour in updatesByHour)
                {
                    if (updatehour.Key == item.Key)
                    {
                        builder.Append("," + (updatehour.Value.Item1 / updatehour.Value.Item2).ToString().Replace(',', '.'));
                    }
                }
                builder.AppendLine();
            }
        }

        private static void getAverages(Dictionary<string, Tuple<double, int>> averageExecutionByDate, Dictionary<int, Tuple<double, int>> averageExecutionByHour, Dictionary<string, Tuple<int>> updatesByDate, Dictionary<int, Tuple<int, int>> updatesByHour)
        {
            foreach (var item in data.averageExecutionTimeByDay)
            {
                if (averageExecutionByDate.ContainsKey(item.day))
                {
                    averageExecutionByDate[item.day] = Tuple.Create(averageExecutionByDate[item.day].Item1 + item.executionTime,
                        averageExecutionByDate[item.day].Item2 + 1);
                }
                else
                {
                    averageExecutionByDate[item.day] = Tuple.Create(item.executionTime, 1);
                }
            }
            foreach (var item in data.averageExecutionTimeByHour)
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
            foreach (var item in data.numberOfUpdatesByDay)
            {
                if (updatesByDate.ContainsKey(item.day))
                {
                    updatesByDate[item.day] = Tuple.Create(updatesByDate[item.day].Item1 + item.updates);
                }
                else
                {
                    updatesByDate[item.day] = Tuple.Create(item.updates);
                }
                totalUpdates += item.updates;
            }
            foreach (var item in data.avgNumberOfUpdatesByHour)
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
        }

        private static string processLogs(string[] logs, string updateText)
        {
            foreach (string log in logs)
            {
                string text = File.ReadAllText(log);
                string date = log.Substring(log.LastIndexOf(@"Logs\") + 5, 19);
                date = date.Split(' ')[0]+ date.Split(' ')[1].Replace("-",":");
                date = date.Insert(10, " ");
                updateText = text;
                getUpdates(updateText);
                DateTime starttime = DateTime.Parse(date, System.Globalization.CultureInfo.InvariantCulture);
                DateTime endtime = DateTime.Parse(text.Substring(text.LastIndexOf(']') - 19, 19), System.Globalization.CultureInfo.InvariantCulture);
                double executionTime = endtime.Subtract(starttime).TotalSeconds;
                if(starttime.Hour == 23 || starttime.Hour == 0)
                {
                    if(endtime.ToString().Split(' ')[1].Split(':')[0].Contains("12"))
                    {
                        string newEndTime = (endtime.ToString().Split(' ')[0] + 
                            endtime.ToString().Split(' ')[1].Replace("12", "00")).Insert(10, " ");
                        DateTime.TryParse(newEndTime, System.Globalization.CultureInfo.InvariantCulture, 
                            System.Globalization.DateTimeStyles.AdjustToUniversal, out endtime);

                    }else if(endtime.ToString().Split(' ')[1].Split(':')[0].Contains("11"))
                    {
                        string newEndTime = (endtime.ToString().Split(' ')[0] + 
                            endtime.ToString().Split(' ')[1].Replace("11", "23")).Insert(10, " ");
                        DateTime.TryParse(newEndTime, System.Globalization.CultureInfo.InvariantCulture, 
                            System.Globalization.DateTimeStyles.AdjustToUniversal, out endtime);
                    }
                    
                    executionTime = endtime.Subtract(starttime).TotalSeconds;
                }
                if (executionTime < 0)
                {
                    endtime = endtime.AddHours(12);
                    executionTime = endtime.Subtract(starttime).TotalSeconds;
                }
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
                day = starttime.Date.ToString("dd/MM/yyyy"),
                executionTime = executionTime
            });
            data.avgNumberOfUpdatesByHour.Add(new updatesByHour()
            {
                hour = starttime.Hour,
                updates = updates
            });
            data.numberOfUpdatesByDay.Add(new updatesByDay()
            {
                day = starttime.Date.ToString("dd/MM/yyyy"),
                updates = updates
            });
        }

        public static void getUpdates(string updateText)
        {
            updates = 0;
            while (updateText.IndexOf("Keywords remaining after filter:") > 0)
            {
                string firstUpdates = updateText.Substring(updateText.IndexOf("Keywords remaining after filter:"),
                    updateText.Length - updateText.IndexOf("Keywords remaining after filter:"));
                updates = Convert.ToInt32(firstUpdates.Substring(32, firstUpdates.IndexOf("\n") - 32).Trim());
                updateText = firstUpdates.Substring(32);
            }
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
