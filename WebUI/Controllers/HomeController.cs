using ServiceClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using TestExecution.Models;

namespace TestExecution.Controllers
{
    public class HomeController : Controller
    {
        private bool IsAPICallRunning
        {
            get
            {
                using (var apiCallClient = new ApiCallClient())
                {
                    var isRunning = apiCallClient.IsRunning();
                    return isRunning;
                }
            }
        }
        //private static Telemetry telemetry = new Telemetry();
        //private static readonly PTLogAnalizer.PTLogAnalizer ptLogAnalizer = new PTLogAnalizer.PTLogAnalizer();
        private static readonly string consoleOutput = "Console Output";

        public ActionResult Index(string id)
        {
            //ViewBag.Message = "Это вызов частичного представления из обычного";

            switch (id)
            {
                case "StopAPICalls":
                    StopAPICalls();
                    break;
                default:
                    break;
            }


            
            if (IsAPICallRunning)
            {
                ViewBag.RunAPICallsDisabled = "disabled";
            }
            else
            {
                ViewBag.StopAPICallsDisabled = "disabled";
            }

            //ViewBag.CpuUsage = telemetry.GetCurrentCpuUsage();
            //ViewBag.AvailableRAM = telemetry.GetAvailableRAM();
            //ViewBag.PtLog = telemetry.getPtLog();

            return View();
        }

        [HttpPost]
        public ActionResult RunAPICalls(ApiCallHomeModel apiCall)
        {
            RunAPICalls(apiCall.UnitTotalAmmount, apiCall.LineAmmount, apiCall.ApiCallInterval);

            //ViewBag.RunAPICallsDisabled = "disabled";

            //var appContext = HttpContext.Current.Session[Constants.ApplicationContextKey] as ApplicationContext;

            //var lines = new List<ApiCallLine>();

            /*
            for (int i = 0; i < apiCallRunner.Lines.Count; i++)
            {
                var requests = apiCallRunner.Lines[i];
                var failedRequestAmmount = requests.Where(e => !e.IsSuccseed).Count();

                lines.Add(new ApiCallLine()
                {
                    UnitNumber = $"unit {i + 1}",
                    TotalRequestAmmount = requests.Count,
                    FailedRequestAmmount = failedRequestAmmount,
                    Succeed = failedRequestAmmount == 0 ? "Passed" : "Failed"
                });
            }

            apiCall.ApiCallLines = lines;
            */

            //return View("Index", lines);
            //return View("Index", apiCall);
            return View("Index");
            //return Partial();
        }

        /*
        public ActionResult Partial()
        {
            var lines = new List<ApiCallLine>();

            for (int i = 0; i < apiCallRunner.Lines.Count; i++)
            {
                var requests = apiCallRunner.Lines[i];
                var failedRequestAmmount = requests.Where(e => !e.IsSuccseed).Count();

                lines.Add(new ApiCallLine()
                {
                    LineName = $"Line {i + 1}",
                    TotalRequestAmmount = requests.Count,
                    FailedRequestAmmount = failedRequestAmmount,
                    IsSucceed = failedRequestAmmount == 0
                });
            }

            return View(lines);
        }
        */
        static int i = 0;

        //public ActionResult YourPartial(YourModel model) //that's if you need the model
        //public ActionResult YourPartial() //that's if you need the model
        //public ActionResult GetData(FlyDataModel flyData) //that's if you need the model
        public ActionResult FlyDataPartial(FlyDataModel flyData)
        {
            TelemetryMain telemetry;

            using (var apiCallClient = new ApiCallClient())
            {
                telemetry = apiCallClient.GetTelemetry();
            }

            //return View(model);
            if (Session["performanceTestExecutionId"] != null)
            {
                var performanceTestExecutionId = Convert.ToInt32(Session["performanceTestExecutionId"]);
                ApiCallMain mainReport;

                using (var apiCallClient = new ApiCallClient())
                {
                    mainReport = apiCallClient.GetMainReport(performanceTestExecutionId);
                }

                //TODO: fix it
                return View(new FlyDataModel()
                {
                    CpuUsage = telemetry.CpuUsage,
                    AvailableRAM = telemetry.AvailableRAM,
                    ShowApiCallDetails = true,
                    TotalUnitsExecuted = mainReport.TotalUnitsExecuted,
                    ApiCallAverageTime = mainReport.ApiCallAverageTime.TotalSeconds > 0 ? mainReport.ApiCallAverageTime.TotalSeconds : 0,
                    UnitProcessingAverageTime = mainReport.UnitProcessingAverageTime.TotalSeconds > 0 && mainReport.UnitProcessingAverageTime.TotalSeconds < 1000 ? mainReport.UnitProcessingAverageTime.TotalSeconds : 0

                });
            }
            else
            {
                return View(new FlyDataModel()
                {
                    ShowApiCallDetails = false,
                    CpuUsage = telemetry.CpuUsage,
                    AvailableRAM = telemetry.AvailableRAM
                });
            }
        }

        public ActionResult ShowApiCalls()
        {
            var lines = new List<ApiCallLine>();

            /*
            for (int i = 0; i < apiCallRunner.Lines.Count; i++)
            {
                var requests = apiCallRunner.Lines[i];
                var failedRequestAmmount = requests.Where(e => !e.IsSuccseed).Count();

                lines.Add(new ApiCallLine()
                {
                    LineName = $"Line {i + 1}",
                    TotalRequestAmmount = requests.Count,
                    FailedRequestAmmount = failedRequestAmmount,
                    IsSucceed = failedRequestAmmount == 0
                });
            }
            */

            return View(lines);
        }

        public ActionResult ShowPtLog()
        {

            //ViewData["PtLogs"] = ptLogAnalizer.Records;

            return View();
        }

        public ActionResult ShowReport()
        {

            ViewBag.Report = "Please wait for the report";

            return View();
        }

        public string ConsoleOutput()
        {
            return consoleOutput;
        }

        private void RunAPICalls(int unitsTotal, int lineAmmount, int apiCallInterval)
        {
            if (IsAPICallRunning)
                return;
            using (var apiCallClient = new ApiCallClient())
            {
                //if (apiCallClient.IsRunning())
                //throw new Exception("API Calls are Running");

                //telemetry.WriteLog();
                //ptLogAnalizer.WriteLog(DateTime.Now, PTLogAnalizer.MonitoredScope.Error | PTLogAnalizer.MonitoredScope.Info);

                var performanceTestExecutionId = apiCallClient.Run(unitsTotal, lineAmmount, apiCallInterval);
                Session["performanceTestExecutionId"] = performanceTestExecutionId;
            }
        }

        private void StopAPICalls()
        {
            if (!IsAPICallRunning)
            {
                return;
            }

            using (var apiCallClient = new ApiCallClient())
            {
                apiCallClient.Stop();
            }

            //telemetry.StopWriteLog();
            //ptLogAnalizer.StopWriteLog();
        }
    }
}