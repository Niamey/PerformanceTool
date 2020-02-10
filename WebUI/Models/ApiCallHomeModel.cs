using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestExecution.Models
{
    public class ApiCallHomeModel
    {
        public int UnitTotalAmmount { get; set; }
        public int LineAmmount { get; set; }
        public int ApiCallInterval { get; set; }

        public int TotalUnitsExecuted { get; set; }
        public double ApiCallAverageTime { get; set; }
        public double UnitProcessingAverageTime { get; set; }
    }
}