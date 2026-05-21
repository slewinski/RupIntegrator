using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls.UI;

namespace KnsMigrator
{
    public partial class MigrForm 
    {
        private BindingSource trBindingSource = new BindingSource();
        

        private void InitTransfer()
        {
            this.trBindingSource.DataSource =   KnsKsiegi.ToList();
            this.rgvKsiegi.DataSource = this.RodzajPrzedmiotuDataSource; //.Mains;

            GridViewComboBoxColumn rodzajColumn = new GridViewComboBoxColumn();
            rodzajColumn.Name = "Rodzajprzedmiotu";
            rodzajColumn.HeaderText = "Rodzaj przedmiotu";
            rodzajColumn.DataSource = thecontext.SAPOpisPrzedmiotu.ToList();
            rodzajColumn.ValueMember = "Symbol";
            rodzajColumn.DisplayMember = "Opis";
            rodzajColumn.FieldName = "RodzajPrzedmiotu";
            rodzajColumn.FilteringMode = GridViewFilteringMode.DisplayMember;
            rodzajColumn.Width = 70;
            this.rgvKsiegi.Columns.Insert(rgvKsiegi.ColumnCount, rodzajColumn);

        }
    }

}