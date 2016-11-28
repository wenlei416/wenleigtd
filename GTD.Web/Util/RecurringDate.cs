using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GTD.Util
{
    public static class RecurringDate
    {
        /// <summary>
        /// 根据json字符串，生成会新建循环任务的日期，不考虑今天是什么日子
        /// </summary>
        /// <param name="recurringJson"></param>
        /// <returns></returns>
        public static List<DateTime> RecurringJsonToDate(string recurringJson)
        {
            //返回值
            List<DateTime> recurringDateTimes = new List<DateTime>();

            //把json字符串解析为dic，并把对应的值取出来
            Dictionary<string, string> recurringDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(recurringJson);
            int cyear = Convert.ToInt32(recurringDictionary["cyear"]);
            int cmonth = Convert.ToInt32(recurringDictionary["cmonth"]);
            int cweek = Convert.ToInt32(recurringDictionary["cweek"]);
            int cday = Convert.ToInt32(recurringDictionary["cday"]);

            //循环日，表示和按json中表明的循环方法，离这个循环方法的第一天距离多少天
            int cyc = Convert.ToInt32(recurringDictionary["cyc"]);

            DateTime startDateTime = Convert.ToDateTime(recurringDictionary["startday"]);
            DateTime endday = Convert.ToDateTime(recurringDictionary["endday"]);

            //每个可能的循环日
            DateTime cycDateTime;
            //year不等于0，表示按年循环
            if (cyear != 0)
            {
                int y = startDateTime.Year;
                //业务规则，cyc设为366，表示循环2-29
                if (cyc == 366)
                {
                    //构造第一个循环日
                    //这里假设了第一年一定有2-29
                    cycDateTime = new DateTime(startDateTime.Year, 2, 29);
                }
                else
                {
                    //非闰年，则随便找个普通年，计算那年的第cyc天是几月几号
                    DateTime dtBase = new DateTime(2001, 1, 1);
                    DateTime dtDate = dtBase.AddDays(cyc - 1);
                    //构造第一个循环日
                    cycDateTime =
                        new DateTime(startDateTime.Year, dtDate.Month, dtDate.Day);
                }
                //todo 对2-29不好使
                //如果循环日小于结束日，进循环
                while (cycDateTime <= endday)
                {
                    //如果循环日小于开始日期，不能加入返回队列，直接年加1
                    if (cycDateTime < startDateTime)
                    {
                        cycDateTime = cycDateTime.AddYears(cyear);
                    }
                    //如果循环日大于开始日期，加入返回队列，年加1
                    else
                    {
                        recurringDateTimes.Add(cycDateTime);
                        cycDateTime = cycDateTime.AddYears(cyear);
                    }
                }
            }
            else if (cmonth != 0)
            {
                int y = startDateTime.Year;
                int m = startDateTime.Month;
                int d;
                //cyc在30以内，说明按几月几号循环，31表示当月最后一天。
                //对于二月，29，30，31都代表最后一天
                //在30以外，说明按第几周的周几循环100代表第一个周日，205，代表第2个周5，505，代表最后一个周5

                //最后一天的场景
                if (cyc == 31)
                {
                    //通过下个月第一天加-1天，获取本月最后一天
                    cycDateTime = new DateTime(y, m, 1).AddMonths(1).AddDays(-1); //这是第startdate那个月的最后一天，第一个循环
                    while (cycDateTime <= endday)
                    {
                        //如果循环日大于开始日期，加入返回队列，月加1
                        recurringDateTimes.Add(cycDateTime);
                        cycDateTime =
                            new DateTime(cycDateTime.AddMonths(1).Year, cycDateTime.AddMonths(1).Month, 1).AddMonths(1)
                                .AddDays(
                                    -1);
                    }
                }
                //不是最后一天的场景
                //处理2月29和30
                else if (cyc < 100)
                {
                    d = cyc;
                    cycDateTime = new DateTime(y, m, d);

                    while (cycDateTime <= endday)
                    {
                        //如果循环日小于开始日期，不能加入返回队列，月加1
                        if (cycDateTime < startDateTime)
                        {
                            //如果是2月，需要处理29和30两个场景
                            if (cycDateTime.Month == 2 && (cyc == 29 || cyc == 30))
                            {
                                cycDateTime = new DateTime(cycDateTime.Year, 3, d);
                            }
                            else
                            {
                                cycDateTime = cycDateTime.AddMonths(cmonth);
                            }
                        }
                        //如果循环日大于开始日期，加入返回队列，月加1，也要处理了2月场景
                        else
                        {
                            recurringDateTimes.Add(cycDateTime);
                            if (cycDateTime.Month == 2 && (cyc == 29 || cyc == 30))
                            {
                                cycDateTime = new DateTime(cycDateTime.Year, 3, d);
                            }
                            else
                            {
                                cycDateTime = cycDateTime.AddMonths(cmonth);
                            }
                        }
                    }
                }
                //按周指定的场景
                //在30以外，说明按第几周的周几循环100代表第一个周日，205，代表第2个周5，505，代表最后一个周5
                //不是第一周的周几，而是第一个周几
                else if (cyc >= 100)
                {
                    int week = cyc / 100; //第几周，5代表最后一周
                    int weekday = cyc % 100; //周几，周日是0
                    if (week == 5)
                    {
                        cycDateTime = new DateTime(y, m, 1);
                        DateTime lastDayofCycMonth = cycDateTime.AddMonths(1).AddDays(-1);
                        int lastDayofCycMonthWeekDay = (int)lastDayofCycMonth.DayOfWeek;
                        cycDateTime =
                            lastDayofCycMonth.AddDays(weekday - lastDayofCycMonthWeekDay >= 0
                                ? weekday - lastDayofCycMonthWeekDay
                                : weekday - lastDayofCycMonthWeekDay - 7);

                        while (cycDateTime <= endday)
                        {
                            if (cycDateTime >= startDateTime)
                            {
                                recurringDateTimes.Add(cycDateTime);
                            }
                            cycDateTime = cycDateTime.AddMonths(cmonth);
                            lastDayofCycMonth =
                                new DateTime(cycDateTime.AddMonths(1).Year, cycDateTime.AddMonths(1).Month, 1).AddDays(
                                    -1);
                            lastDayofCycMonthWeekDay = (int)lastDayofCycMonth.DayOfWeek;
                            cycDateTime =
                                lastDayofCycMonth.AddDays(weekday - lastDayofCycMonthWeekDay <= 0
                                    ? weekday - lastDayofCycMonthWeekDay
                                    : weekday - lastDayofCycMonthWeekDay - 7);
                        }
                    }
                    else
                    {
                        cycDateTime = new DateTime(y, m, 1);
                        int firstDayofCycMonthWeekDay = (int)cycDateTime.DayOfWeek;
                        cycDateTime =
                            cycDateTime.AddDays(weekday - firstDayofCycMonthWeekDay >= 0
                                ? weekday - firstDayofCycMonthWeekDay + 7 * (week - 1)
                                : weekday - firstDayofCycMonthWeekDay + 7 * week);

                        while (cycDateTime <= endday)
                        {
                            if (cycDateTime >= startDateTime)
                            {
                                recurringDateTimes.Add(cycDateTime);
                            }
                            cycDateTime = new DateTime(cycDateTime.AddMonths(cmonth).Year,
                                cycDateTime.AddMonths(cmonth).Month, 1);
                            firstDayofCycMonthWeekDay = (int)cycDateTime.DayOfWeek;
                            cycDateTime =
                                cycDateTime.AddDays(weekday - firstDayofCycMonthWeekDay >= 0
                                    ? weekday - firstDayofCycMonthWeekDay + 7 * (week - 1)
                                    : weekday - firstDayofCycMonthWeekDay + 7 * week);
                        }
                    }
                }


            }
            else if (cweek != 0)
            {
                //周日是0，算出最近的符合要求的周几，可能是倒退一天
                cycDateTime = startDateTime.AddDays(cyc - (int)startDateTime.DayOfWeek);
                while (cycDateTime <= endday)
                {
                    //如果循环日小于开始日期，不能加入返回队列，直接日加7
                    if (cycDateTime < startDateTime)
                    {
                        cycDateTime = cycDateTime.AddDays(cweek * 7);
                    }
                    //如果循环日大于开始日期，加入返回队列，日加7
                    else
                    {
                        recurringDateTimes.Add(cycDateTime);
                        cycDateTime = cycDateTime.AddDays(cweek * 7);
                    }
                }
            }
            else if (cday != 0)
            {
                //开始日第一天一定在范围内，cyc是没有用的
                cycDateTime = startDateTime;
                while (cycDateTime <= endday)
                {
                    recurringDateTimes.Add(cycDateTime);
                    cycDateTime = cycDateTime.AddDays(cday);
                }
            }
            return recurringDateTimes;

        }


        public static string JsonToString(string recurringJson)
        {
            //{'cyear':'0','cmonth':'0','cweek':'0','cday':'1','startday':'2016-10-1','endday':'2016-10-5','cyc':'100'}
            string recurringDescription = null;
            string repeatDate;

            //把json字符串解析为dic，并把对应的值取出来
            Dictionary<string, string> recurringDictionary =
                JsonConvert.DeserializeObject<Dictionary<string, string>>(recurringJson);
            int cyear = Convert.ToInt32(recurringDictionary["cyear"]);
            int cmonth = Convert.ToInt32(recurringDictionary["cmonth"]);
            int cweek = Convert.ToInt32(recurringDictionary["cweek"]);
            int cday = Convert.ToInt32(recurringDictionary["cday"]);

            //循环日，表示和按json中表明的循环方法，离这个循环方法的第一天距离多少天
            int cyc = Convert.ToInt32(recurringDictionary["cyc"]);

            DateTime startDateTime = Convert.ToDateTime(recurringDictionary["startday"]);
            DateTime endday = Convert.ToDateTime(recurringDictionary["endday"]);

            if (cyear != 0)
            {
                //业务规则，cyc设为366，表示循环2-29
                if (cyc == 366)
                {
                    //构造第一个循环日
                    //这里假设了第一年一定有2-29
                    repeatDate = "2月29日";
                }
                else
                {
                    //非闰年，则随便找个普通年，计算那年的第cyc天是几月几号
                    DateTime dtBase = new DateTime(2001, 1, 1);
                    DateTime dtDate = dtBase.AddDays(cyc - 1);
                    //构造第一个循环日
                    repeatDate = (new DateTime(startDateTime.Year, dtDate.Month, dtDate.Day)).ToString("M月d日");
                }
                recurringDescription = $"每隔{cyear}年的{repeatDate}重复";
            }
            else if (cmonth != 0)
            {
                int m = startDateTime.Month;
                int d = cyc;
                //cyc在30以内，说明按几月几号循环，31表示当月最后一天。
                //对于二月，29，30，31都代表最后一天
                //在30以外，说明按第几周的周几循环100代表第一个周日，205，代表第2个周5，505，代表最后一个周5

                //最后一天的场景
                if (cyc == 31)
                {
                    repeatDate = "最后一天";
                    recurringDescription = $"每隔{cmonth}个月的{repeatDate}重复";
                }
                //不是最后一天的场景
                //处理2月29和30
                else if (cyc < 100)
                {
                    repeatDate = $"{d}日";
                    recurringDescription = $"每隔{cmonth}个月的{repeatDate}重复";

                }
                ////按周指定的场景
                ////在30以外，说明按第几周的周几循环100代表第一个周日，205，代表第2个周5，505，代表最后一个周5
                ////不是第一周的周几，而是第一个周几
                else if (cyc >= 100)
                {
                    int week = cyc / 100;//第几周，5代表最后一周
                    string weekday = (cyc % 100) != 0 ? (cyc % 100).ToString() : "日";
                    //周几，周日是0
                    if (week == 5)
                    {
                        repeatDate = $"最后一周周{weekday}";
                        recurringDescription = $"每隔{cmonth}个月的{repeatDate}重复";
                    }
                    else
                    {
                        repeatDate = $"第{week}周周{weekday}";
                        recurringDescription = $"每隔{cmonth}个月的{repeatDate}重复";
                    }
                }
            }
            else if (cweek != 0)
            {
                //周日是0
                repeatDate = cyc != 0 ? cyc.ToString() : "日";
                recurringDescription = $"每隔{cweek}周的周{repeatDate}重复";
            }
            else if (cday != 0)
            {
                recurringDescription = $"每隔{cday}天重复";
            }

            return recurringDescription;
        }
    }
}
