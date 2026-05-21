using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace RupLoader
{
    public partial class SchedulerForm : Form
    {
        private RupIntegratorEntities context;
        // private List<DateTime> schedHours= new List<DateTime>();
        public class schedHoursClass
        {
            private DateTime _time;
            public schedHoursClass()
            {
            }
            public schedHoursClass(DateTime t)
            {
                this._time = t;
            }
            public DateTime ScheduleTime
            {
                get
                {
                    return this._time;
                }
                set {
                    this._time = value;

                }

            }

        }

        public class schedDaysClass
        {
            private uint _day;
            public schedDaysClass()
            {
            }
            public schedDaysClass(uint d)
            {
                this._day = d;
            }
            public uint DayOfaMonth
            {
                get
                {
                    return this._day;
                }
                set
                {
                    this._day = value;

                }

            }

        }

        private List<schedHoursClass> schedHours = new List<schedHoursClass>();
        private List<schedDaysClass> schedDays = new List<schedDaysClass>();

        private SchedulerJob job2;
        private SchedulerJob job1;
        private void loadContent()
        {
            List<SchedulerJob> jobs = context.SchedulerJob.ToList();
            int jobidM = 0 , jobidD = 0 ;

            foreach (SchedulerJob j in jobs)
            {
                if (j.JobType == 1) // jeśli opcja dzienna
                {
                    job1 = j;
                    jobidD = j.Id;
                    List<string> schedHoursStr = j.DJobHours.Split(',').ToList();
                    if (schedHoursStr != null)
                    {
                        foreach (string s in schedHoursStr)
                        {
                            DateTime t;
                            if (DateTime.TryParse(s, out t))
                                schedHours.Add(new schedHoursClass(t));
                        }

                    }

                }
                else if  ( j.JobType == 2  )  // miesięczny ckl - odbieramy 
                {
                    job2 = j;
                    jobidM = j.Id;
                    List<string> schedDaysStr = j.MJobDays.Split(',').ToList();
                    if (schedDaysStr != null)
                    {
                        foreach (string s in schedDaysStr)
                        {
                            uint i;
                            if (UInt32.TryParse(s, out i))
                                schedDays.Add(new schedDaysClass(i));
                        }

                    }
                   
                    rdtpGodzina.Value = j.MJobHour.Value;


                }

             }
            if (job1 == null)
            {
                job1 = new SchedulerJob();
                job1.JobType = 1;
                context.SchedulerJob.AddObject(job1);
            }
            if (job2 == null)
            {
                job2 = new SchedulerJob();
                job2.JobType = 2;
                context.SchedulerJob.AddObject(job2);
            }


                rgvDailyJobs.DataSource =  context.SchedulerItem.Where(c => c.SchedulerJobId == jobidD).ToList();
                rgvMonthlyJobs.DataSource = context.SchedulerItem.Where(c => c.SchedulerJobId == jobidM).ToList();
                rgvDaysofMonth.DataSource = schedDays;
                 rgvSchedules.DataSource = schedHours;




        }

        private void saveContent()
        {
            string hours = string.Empty;
            string days = string.Empty;

            job1 = context.SchedulerJob.Where(a => a.JobType == 1).FirstOrDefault();
            if (job1 == null)
            {
                job1 = new SchedulerJob();
                job1.JobType = 1;
                context.SchedulerJob.AddObject(job1);
            }
            foreach (schedHoursClass s in schedHours)
            {
                if (!string.IsNullOrWhiteSpace(hours))
                    hours += ",";
                hours += s.ScheduleTime.ToString("HH:mm");

            }
            job1.DJobHours = hours;

            job2 = context.SchedulerJob.Where(a => a.JobType == 2).FirstOrDefault();
            if (job2 == null)
            {
                job2 = new SchedulerJob();
                job2.JobType = 2;
                context.SchedulerJob.AddObject(job2);
            }
            foreach (schedDaysClass s in schedDays)
            {
                if (!string.IsNullOrWhiteSpace(days))
                    days += ",";
               days += s.DayOfaMonth.ToString();

            }
            job2.MJobDays = days;
            job2.MJobHour =  new DateTime(2000,1,1) + new TimeSpan(0, rdtpGodzina.Value.Hour, rdtpGodzina.Value.Minute,0);


            context.SaveChanges();


        }



        public SchedulerForm()
        {
            InitializeComponent();
            context = new RupIntegratorEntities();
            loadContent();
        }

        private void rbStart_Click(object sender, EventArgs e)
        {

            Scheduler sc = new Scheduler();
            sc.StartMonthlyJobNow();
            
           
        }

        private void rbSaveMonthScheduler_Click(object sender, EventArgs e)
        {
            saveContent();
        }

        private void SchedulerForm_Load(object sender, EventArgs e)
        {

        }
        private void AddItem(SchedulerJob theJob)
        {
            AddJobItem aj = new AddJobItem();
            if (aj.ShowDialog() == DialogResult.OK)
            {
                SchedulerItem si = new SchedulerItem();
                si.Arguments = aj.tbArgs.Text;
                si.RL_KonfigId = (int)aj.rddlJobItem.SelectedValue;
                si.SchedulerJob = theJob;
                this.context.SchedulerItem.AddObject(si);
                this.context.SaveChanges();

            }
            
        }


        private void DeleteItem(RadGridView rgv)
        {
            if (rgv.SelectedRows.Count > 0)
            {
                if (MessageBox.Show("Czy na pewno chcesz usunąć wybrane zadanie ?", "Potwierdź", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                    return;
                context.SchedulerItem.DeleteObject(rgv.SelectedRows[0].DataBoundItem as SchedulerItem);
                rgv.SelectedRows[0].Delete();
              
            }

        }


          
        private void rbAddDaily_Click(object sender, EventArgs e)
        {
            AddItem(job1);
            rgvDailyJobs.DataSource = context.SchedulerItem.Where(c => c.SchedulerJob.JobType == 1).ToList();
            

        }

        private void rbAddMonthly_Click(object sender, EventArgs e)
        {
            AddItem(job2);
            rgvMonthlyJobs.DataSource = context.SchedulerItem.Where(c => c.SchedulerJob.JobType == 2).ToList();

        }

        private void rbDellDaily_Click(object sender, EventArgs e)
        {
            
            DeleteItem(rgvDailyJobs);

        }

        private void rbDelMonthly_Click(object sender, EventArgs e)
        {
            DeleteItem(rgvMonthlyJobs);
        }

        private void rbStart2_Click(object sender, EventArgs e)
        {
            Scheduler sc = new Scheduler();
            sc.StartDailyJobNow();
        }

        private void rbLoadJobs_Click(object sender, EventArgs e)
        {
            Scheduler sc = new Scheduler();
            sc.clearAllJobs(); // czyszczenie poprzednich zadań,
            sc.StartDailyJob();
            sc.StartMonthlyJob();

        }
    }
}
