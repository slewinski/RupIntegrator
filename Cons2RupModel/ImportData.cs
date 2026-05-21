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
        Prepared,
        InTransfer,
        Submited,
        Pending,
        FailedImport,
        FailedFiinal,
        Done
    }

    public enum ConsJobStatus
    { 
        New,
        OnGoing,
        Ended

    }
}
