using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
//using System.Timers;
using System.IO;
using Telerik.WinControls.UI;
using System.Windows.Forms;

namespace RupLoader
{
    class FileListener
    {
         private BackgroundWorker worker;
      
         public FileListener()
    {
        worker = new BackgroundWorker();
        worker.DoWork += worker_DoWork;
        System.Timers.Timer  timer = new System.Timers.Timer(2000);
        timer.Elapsed += timer_Elapsed;
        timer.Start();
    }

         void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        if(!worker.IsBusy)
            worker.RunWorkerAsync();
    }
    private Control FindControl(Control parent, string name)
    {
        // Check the parent.
        if (parent.Name == name) return parent;

        // Recursively search the parent's children.
        foreach (Control ctl in parent.Controls)
        {
            Control found = FindControl(ctl, name);
            if (found != null) return found;
        }

        // If we still haven't found it, it's not here.
        return null;
    }
    void worker_DoWork(object sender, DoWorkEventArgs e)
    {
        if (File.Exists(RunMode.CmdFileName))
        {
            string value = File.ReadAllText(RunMode.CmdFileName);
            RadGridView rgv = FindControl(RunMode.wndHandler,"rgvWyciag") as RadGridView;
            if (rgv != null)
            {
                RunMode.data = value;
                PaymentService prsv = new PaymentService();
                prsv.AttachCmdDataSource(rgv);
                File.Delete(RunMode.CmdFileName);
                prsv.ParseTytul(rgv);
                //prsv.InitPozostaloGrid(rgv);
            }
        }
    }
    }
}
