using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestExecution.Models
{
    public class ApiCallLine
    {
        public string UnitNumber { get; set; }
        public string Succeed { get; set; }
        public int FailedRequestAmmount { get; set; }
        public int TotalRequestAmmount { get; set; }
    }
}