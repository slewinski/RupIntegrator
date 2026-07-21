using ConsInterfeces.Rup2ConsImportContentSystemData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cons2RupModel
{
    public class ConsImportData 
    {
        public int IdSprawy { get; set;}
        public int IdStrony { get; set;}
        public int IdImportu { get; set; }
        public ConsImportStatus status { get; set; }
        public string HashValue { get; set;}
        public ImportContentSystemDataRequest importContentSystemDataRequest { get; set;}
    }

    public enum ConsImportStatus
    {
        Prepared = 0,
        Pending = 1,
        Done = 2,
        Error = 3,
        Duplicate = 4
    }


    public enum ConsJobStatus
    { 
        New,
        OnGoing,
        Ended

    }
}
