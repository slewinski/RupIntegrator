using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace RupLoader
{
    public class MonthlyJob : IJob
        {


            public void Execute(IJobExecutionContext context)
            {
            JobManager job = new JobManager();
            job.ExecJob(1);

              
            }
        }

  


    public class DailyJob : IJob
    {


        public void Execute(IJobExecutionContext context)
        {
            JobManager job = new JobManager();
            job.ExecJob(2);

        }
    }

}
