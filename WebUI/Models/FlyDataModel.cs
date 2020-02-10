using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestExecution.Models
{
    public class FlyDataModel
    {
        public float CpuUsage { get; set; }
        public float AvailableRAM { get; set; }

        public bool ShowApiCallDetails { get; set; }
        public int TotalUnitsExecuted { get; set; }
        public double ApiCallAverageTime { get; set; }
        public double UnitProcessingAverageTime { get; set; }
    }
}