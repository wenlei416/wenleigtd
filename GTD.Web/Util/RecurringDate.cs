using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace GTD.Util
{
    public static class RecurringDate
    {
        public static List<DateTime> RecurringJsonToDate(string recurringJson)
        {
            //返回值
            List<DateTime> recurringDateTimes = new List<DateTime>();

            //把json字符串解析为dic，并把对应的值取出来
            Dictionary<string, string> recurringDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(recurringJson);
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
                //对2-29不好使
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

            }
            else if (cweek != 0)
            {
                //周日是0
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
                cycDateTime = startDateTime;
                while (cycDateTime <= endday)
                {
                    recurringDateTimes.Add(cycDateTime);
                    cycDateTime = cycDateTime.AddDays(cday);
                }
            }
            return recurringDateTimes;

        }
    }
}