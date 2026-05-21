using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RupLoader
{

    public class Scheduler
    {

        private List<Tuple<int, int>> monthlyTriggerLst()
        {
            List<Tuple<int, int>> lst = new List<Tuple<int, int>>();
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                SchedulerJob job = context.SchedulerJob.Where(a => a.JobType == 1).FirstOrDefault();
                if (job == null)
                    return null;


                List<string> schedHoursStr = job.DJobHours.Split(',').ToList();
                if (schedHoursStr != null)
                {
                    foreach (string s in schedHoursStr)
                    {
                        DateTime t;
                        if (DateTime.TryParse(s, out t))
                        {
                            Tuple<int, int> item = new Tuple<int, int>(t.Hour, t.Minute);
                            lst.Add(item);


                        }
                    }
                }
            }
            return lst;
        }


        private List<int> dailyTriggerLst(out DateTime triggerTime)
        {
            List<int> lst = new List<int>();
           
            using (RupIntegratorEntities context = new RupIntegratorEntities())
            {
                SchedulerJob job = context.SchedulerJob.Where(a => a.JobType == 2).FirstOrDefault();
                if (job == null)
                {
                    triggerTime  = new DateTime(); // any datetime
                    return null;
                }


                List<string> schedDaysStr = job.MJobDays.Split(',').ToList();
                if (schedDaysStr != null)
                {
                    foreach (string s in schedDaysStr)
                    {
                        int i;
                        if (int.TryParse(s, out i))
                        {
                            
                            lst.Add(i);


                        }
                    }
                }
                triggerTime = job.MJobHour.Value;
            }

            return lst;
        }

        public void clearAllJobs()
        {
            IList<JobKey> jobLst;
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            var groupMatcher = GroupMatcher<JobKey>.GroupContains("RupLoader");
            jobLst = scheduler.GetJobKeys(groupMatcher).ToList<JobKey>();
            scheduler.DeleteJobs(jobLst);
         }

        public void StartMonthlyJob()
        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<MonthlyJob>().Build();
            List<Tuple<int, int>> lst = monthlyTriggerLst();
            if (lst == null || lst.Count == 0)
            {
                Utils.LogWriter("Brak zadań do uruchomieniaa w trybie dziennym");
                return;
            }
            int iNo = 0; 
            foreach (Tuple<int, int> item in lst)
            {
                var trigg = TriggerBuilder.Create()
                .WithIdentity("MonthlyJob"+ (++iNo).ToString(),"RupLoader" )
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(item.Item1, item.Item2))
                .Build();
                scheduler.ScheduleJob(job, trigg);
            }
        }

        public void StartDailyJob()

        {

            DateTime schedTime = new DateTime();
            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();

            IJobDetail job = JobBuilder.Create<MonthlyJob>().Build();
            List<int> lst = dailyTriggerLst(out schedTime);
            
            if (lst == null || lst.Count == 0)
            {
                Utils.LogWriter("Brak zadań do uruchomienia w trybie miesięcznym");
                return;
            }
            lst.Sort();
            int iNo = 0;
            string s = string.Join(",", lst.Select(n => n.ToString()).ToArray());
            if (!string.IsNullOrWhiteSpace(s))
            {
                ITrigger trigg = TriggerBuilder.Create()
                .ForJob(job)
               .WithIdentity("DailyJob", "RupLoader")
               .WithCronSchedule("0 " + schedTime.Minute.ToString() + " " + schedTime.Hour.ToString() + " " + s + " * ?")
               .Build();
                scheduler.ScheduleJob(job, trigg);
            } 
           
           

        }

        public void StartDailyJobNow()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<DailyJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity("DailyJob", "RupLoader")
               .StartNow()
               .WithPriority(1)
               .Build();
            scheduler.ScheduleJob(job, trigger);

        }

        public void StartMonthlyJobNow()

        {

            IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();
            scheduler.Start();
            IJobDetail job = JobBuilder.Create<MonthlyJob>().Build();
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity("MonthlyJob", "RupLoader")
               .StartNow()
               .WithPriority(1)
               .Build();
            scheduler.ScheduleJob(job, trigger);

        }

      


    }

}
