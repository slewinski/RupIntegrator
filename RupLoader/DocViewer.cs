using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace RupLoader
{
    public partial class DocViewer : Form
    {
        public string RtfDoc { get; set; } 
       

        public DocViewer()
        {
            InitializeComponent();
            ContextMenu contextMenu = new System.Windows.Forms.ContextMenu();
            MenuItem menuItem = new MenuItem("Wytnij");
            menuItem.Enabled = false;
            menuItem.Click += new EventHandler(CutAction);
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Kopiuj");
            menuItem.Click += new EventHandler(CopyAction);
            contextMenu.MenuItems.Add(menuItem);
            menuItem = new MenuItem("Wklej");
            menuItem.Enabled = false;
            menuItem.Click += new EventHandler(PasteAction);
            contextMenu.MenuItems.Add(menuItem);

            rtStandard.ContextMenu = contextMenu;
            
        }

        private void DocViewer_Load(object sender, EventArgs e)
        {
            if (RtfDoc != null && RtfDoc.Length > 0)
            {
           
                rtStandard.Rtf = RtfDoc;
            
            }
        }
        void CutAction(object sender, EventArgs e)
        {
            rtStandard.Cut();
        }

        void CopyAction(object sender, EventArgs e)
        {
            rtStandard.Copy(); //Clipboard.SetText(rtStandard.SelectedText);
        }
        void PasteAction(object sender, EventArgs e)
        {
            if (Clipboard.ContainsText())
            {
                rtStandard.Paste();
            }
        }  
      
       
    }
}
