using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using TeamGlobal.Infrastructure;
using TeamGlobal.Infrastructure.Model;
using TeamGlobal.Infrastructure.Services;
using TeamGlobal.Infrastructure.ViewModel;
using TeamGlobal.Web.Helper;

namespace TeamGlobal.Web.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IUnitOfWork unitOfWork;

        public HomeController()
        {
            unitOfWork = new UnitOfWork();
        }

        public ActionResult Index(string fromDate, string toDate, string origin, string destination)
        {
            string ApiName = WebConfigurationManager.AppSettings["WebApi"];

            LocationViewModel location = new LocationViewModel();

            if (!string.IsNullOrEmpty(fromDate) &&
                !string.IsNullOrEmpty(toDate) &&
                !string.IsNullOrEmpty(origin) &&
                !string.IsNullOrEmpty(destination))
            {
                location.DestinationId = destination;
                location.OriginId = origin;
                location.FromDate = fromDate;
                location.ToDate = toDate;
            }

            return View(location);
        }

        public JsonResult SourceLocation(string sourceLocation)
        {
            var data = unitOfWork.Repository<Location>().GetAll()
                .Where(x => x.NAME.ToLower().StartsWith(sourceLocation.ToLower()))
                .Select(x => new { Name = (string.IsNullOrEmpty(x.CODE) ? "" : x.CODE) + ", " + (string.IsNullOrEmpty(x.NAME) ? "" : x.NAME), Value = x.CODE, Country = x.CountryName })
                 .Distinct().Take(10).ToList();

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DestinationLocation(string destinationLocation)
        {
            var data = unitOfWork.Repository<Location>().GetAll()
             .Where(x => x.NAME.ToLower().StartsWith(destinationLocation.ToLower()))
                .Select(x => new { Name = (string.IsNullOrEmpty(x.CODE) ? "" : x.CODE) + ", " + (string.IsNullOrEmpty(x.NAME) ? "" : x.NAME), Value = x.CODE, Country = x.CountryName })
                .Distinct().Take(10).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> GetSearchResult(string fromDate, string toDate, string fromSource, string toDestination)
        {
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.FromSource = fromSource;
            ViewBag.ToDestination = toDestination;

            DisplayViewModel displayViewModel = new DisplayViewModel();

            displayViewModel.FromDate = fromDate;
            displayViewModel.ToDate = toDate;
            displayViewModel.FromSource = fromSource;
            displayViewModel.ToDestination = toDestination;

            List<SearchResultViewModel> resultList = new List<SearchResultViewModel>();

            await TryMergePort(fromDate, toDate, fromSource, toDestination, displayViewModel);

            return PartialView("_GetSearchResult", displayViewModel);
        }

        private async Task TryMergePort(string fromDate, string toDate, string fromSource, string toDestination, DisplayViewModel displayViewModel)
        {
            try
            {
                var source = unitOfWork.Repository<Location>().Get(x => string.Equals(x.CODE, fromSource, StringComparison.InvariantCultureIgnoreCase));

                bool isFromIndia = false;

                if (string.Equals(source.CountryName, "INDIA", StringComparison.InvariantCultureIgnoreCase))
                {
                    isFromIndia = true;
                }

                displayViewModel.IsFromIndia = isFromIndia;

                await GetApiResult(fromDate, toDate, fromSource, toDestination, displayViewModel.SearchResultViewModel);

                if (displayViewModel.SearchResultViewModel.Count != 0 && isFromIndia)
                {
                    foreach (var result in displayViewModel.SearchResultViewModel)
                    {
                        result.IsFromIndia = true;
                    }
                }

                if (displayViewModel.SearchResultViewModel != null && displayViewModel.SearchResultViewModel.Count == 0)
                {
                    var locationA = unitOfWork.Repository<Location>().Get(x => x.CODE.Equals(fromSource, StringComparison.InvariantCultureIgnoreCase));
                    var locationC = unitOfWork.Repository<Location>().Get(x => x.CODE.Equals(toDestination, StringComparison.InvariantCultureIgnoreCase));

                    /* For checking pourpose */
                    //var portList = unitOfWork.Repository<PortList>()
                    //       .GetMany(x => x.OriginCode.Equals(fromSource, StringComparison.InvariantCultureIgnoreCase)
                    //                   && x.DestinationCode.Equals(toDestination, StringComparison.InvariantCultureIgnoreCase));
                    //var protListOrder = portList.OrderBy(x => x.RoutingCode);
                    //var selectedPort = protListOrder.FirstOrDefault();

                    var protInfo = unitOfWork.Repository<PortList>()
                        .GetMany(x => x.OriginCode.Equals(fromSource, StringComparison.InvariantCultureIgnoreCase)
                                    && x.DestinationCode.Equals(toDestination, StringComparison.InvariantCultureIgnoreCase))
                                    .OrderBy(x => x.PreferenceOrder)
                                    .FirstOrDefault();

                    if (protInfo != null)
                    {
                        List<SearchResultViewModel> resultListAtoB = new List<SearchResultViewModel>();
                        List<SearchResultViewModel> resultListBtoC = new List<SearchResultViewModel>();

                        var locationB = unitOfWork.Repository<Location>().Get(x => x.CODE.ToUpper().Contains(protInfo.RoutingCode.ToUpper()));
                        //routeInfo[0] = string.Format("{0}, {1}", locationB.NAME ?? "", locationB.CountryName ?? "");
                        //routeInfo[0] = string.Format("{0}, {1}", locationB.NAME ?? "", locationB.CountryName ?? "");
                        displayViewModel.RoutedBy = locationB.CODE;
                        await GetApiResult(fromDate, toDate, fromSource, protInfo.RoutingCode, resultListAtoB);
                        await GetApiResult(fromDate, toDate, protInfo.RoutingCode, toDestination, resultListBtoC);

                        // As Per discuss wwith sony
                        //if (resultListBtoC.Any())
                        //{
                        //    var BtoC = resultListBtoC.FirstOrDefault();

                        //    int.TryParse(BtoC.TransitTimeCutOffOriginCFStoPortOfDischarge, out int TransitTimeCutOffOriginCFStoPortOfDischarge);
                        //    int.TryParse(BtoC.TransitTimePortofLoadingtoPortofDischarge, out int TransitTimePortofLoadingtoPortofDischarge);
                        //    int.TryParse(BtoC.TransitTimePortofLoadingtoCFSDestination, out int TransitTimePortofLoadingtoCFSDestination);
                        //    int.TryParse(BtoC.TransitTimeCutOffOriginCFStoCFSDestination, out int TransitTimeCutOffOriginCFStoCFSDestination);

                        //    foreach (var item in resultListAtoB)
                        //    {
                        //        if (int.TryParse(item.TransitTimeCutOffOriginCFStoCFSDestination, out int resultAValue))
                        //        {
                        //            item.TransitTimeCutOffOriginCFStoCFSDestination = Convert.ToString(resultAValue + TransitTimeCutOffOriginCFStoCFSDestination);
                        //        }

                        //        if (int.TryParse(item.TransitTimeCutOffOriginCFStoPortOfDischarge, out resultAValue))
                        //        {
                        //            item.TransitTimeCutOffOriginCFStoPortOfDischarge = Convert.ToString(resultAValue + TransitTimeCutOffOriginCFStoPortOfDischarge);
                        //        }

                        //        if (int.TryParse(item.TransitTimePortofLoadingtoPortofDischarge, out resultAValue))
                        //        {
                        //            item.TransitTimePortofLoadingtoPortofDischarge = Convert.ToString(resultAValue + TransitTimePortofLoadingtoPortofDischarge);
                        //        }

                        //        if (int.TryParse(item.TransitTimePortofLoadingtoCFSDestination, out resultAValue))
                        //        {
                        //            item.TransitTimePortofLoadingtoCFSDestination = Convert.ToString(resultAValue + TransitTimePortofLoadingtoCFSDestination);
                        //        }
                        //    }
                        //}

                        //routeInfo[0] = "true";
                        //routeInfo[1] = Convert.ToString(resultListAtoB.Count);
                        displayViewModel.SearchResultViewModel.AddRange(resultListAtoB);

                        displayViewModel.IsMergeResut = true;

                        if (resultListAtoB.Count != 0 && isFromIndia)
                        {
                            foreach (var result in resultListAtoB)
                            {
                                result.IsFromIndia = true;
                                result.IsRouteFromAtoB = true;
                            }
                        }
                        displayViewModel.SearchResultViewModel.AddRange(resultListBtoC);
                    }
                }
            }
            catch (Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
            }
        }

        private async Task GetApiResult(string fromDate, string toDate, string fromSource, string toDestination, List<SearchResultViewModel> resultList)
        {
            RestHelper restHelper = new RestHelper(fromDate, toDate, fromSource, toDestination);

            var result = await restHelper.GetObjects<ResponceObject>();

            if (result != null && result.ScheduleResponse != null && result.ScheduleResponse.ScheduleResponseDetails.Count > 0)
            {
                foreach (var item in result.ScheduleResponse.ScheduleResponseDetails)
                {
                    var search = new SearchResultViewModel
                    {
                        DirectTransship = "Direct",
                        VesselVoyage = string.Format("{0} / {1} {2}", item.VesselName ?? "", item.Voyage ?? "", item.CarrierSCAC ?? ""),
                        ETD = Convert.ToDateTime(item.ETD).ToOrdinal(),
                        ETA = Convert.ToDateTime(item.ETA).ToOrdinal(),
                        CutOffDate = item.CutOffDateTime.GetDateTimeFormateWithIST(),

                        TransitTimeCutOffOriginCFStoPortOfDischarge = Convert.ToString(item.TransitTimeCFSToCFS),
                        TransitTimePortofLoadingtoPortofDischarge = Convert.ToString(item.TransitTimePortToPort),
                        TransitTimePortofLoadingtoCFSDestination = Convert.ToString(item.TransitTimePortToPort),
                        TransitTimeCutOffOriginCFStoCFSDestination = Convert.ToString(item.TransitTimeCFSToCFS)
                    };

                    var portNameCountry = unitOfWork.Repository<Location>().GetMany(x => x.CODE.ToLower().Equals(item.RoutingVia.ToLower()))
                        .Select(x => new
                        {
                            PortName = (x.NAME + ", " + x.CountryName)
                        });

                    search.PortOfLoading = portNameCountry.Select(x => x.PortName).FirstOrDefault();
                    resultList.Add(search);
                }
            }
        }

        public async Task<ActionResult> ExportExcel(string fromDate, string toDate, string fromSource, string toDestination)
        {
            if (string.IsNullOrEmpty(fromDate) ||
                string.IsNullOrEmpty(toDate) ||
                string.IsNullOrEmpty(fromSource) ||
                string.IsNullOrEmpty(toDestination))
            {
                throw new ArgumentNullException();
            }
            try
            {
                //List<SearchResultViewModel> resultList = new List<SearchResultViewModel>();
                DisplayViewModel displayViewModel = new DisplayViewModel();

                await TryMergePort(fromDate, toDate, fromSource, toDestination, displayViewModel);

                var OriginObject = unitOfWork.Repository<Location>().Get(x => x.CODE == fromSource);
                var Origin = OriginObject.NAME + ", " + OriginObject.CountryName;

                var DestinationObject = unitOfWork.Repository<Location>().Get(x => x.CODE == toDestination);
                var Destination = DestinationObject.NAME + ", " + DestinationObject.CountryName;

                ExcelHelper excelHelper = new ExcelHelper(fromDate, toDate, Origin, Destination);
                string path = excelHelper.GetContext(displayViewModel.SearchResultViewModel);

                return File(path, "application/vnd.ms-excel", "Team Global.xlsx");
            }
            catch (Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
                return null;
            }
        }

        public ActionResult DummyCall()
        {
            return null;
        }

        /// <summary>
        /// "fromDate": fromDate,
        /// "toDate": toDate,
        /// "fromOrigin": fromOrigin,
        /// "toDestination": toDestination,
        ///  "origin": origin,
        /// "destination": destination,
        /// </summary>
        /// <returns></returns>
        //public ActionResult QueryForm(string fromDate, string toDate, string fromOrigin, string toDestination, string origin, string destination)
        public ActionResult QueryForm(QueryViewModel queryViewModel)
        {
            if (ModelState.IsValid)
                return PartialView("_QueryForm", queryViewModel);

            return null;
        }

        [HttpPost]
        public async Task SubmitQuery(QueryViewModel queryViewModel)
        {
            try
            {
                string htmlbody = "<html>";
                htmlbody += "<head>";
                htmlbody += "<style>";
                htmlbody += "table,th,td {border: 1px solid black;    border-collapse: collapse;}";
                htmlbody += "th,td {padding: 3px;text-align: left;text-align: center;}";
                htmlbody += "hr {margin-top : 50px;margin-bottom : 50px;}";
                htmlbody += ".x {text-align: left;padding-left: 30px;}";
                htmlbody += "</style>";
                htmlbody += "</head>";
                htmlbody += "<body>";

                string customerInfo = string.Empty;
                customerInfo += "<h2>Customer Info:</h2>";
                customerInfo += "<table style='width:1000px'>";
                customerInfo += "<tr><th>First Name </th><th>Last Name </th><th>Email</th><th>Phone</th></tr>";
                customerInfo += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", queryViewModel.FirstName ?? "", queryViewModel.LastName ?? "", queryViewModel.Email ?? "", queryViewModel.Phone ?? "");
                customerInfo += "</table>";
                customerInfo += "<hr/>";
                htmlbody += customerInfo;

                string travelInfo = string.Empty;
                travelInfo += "<h2>Travel Info:</h2>";
                travelInfo += "<table style='width:1000px'>";
                travelInfo += "<tr><th>Origin</th><th>Origin Date</th><th>Destination</th><th>Destination Date</th></tr>";
                travelInfo += string.Format("<tr><td>{0}</td><td>{1}</td><td>{2}</td><td>{3}</td></tr>", queryViewModel.Origin ?? "", queryViewModel.FromDate ?? "", queryViewModel.Destination ?? "", queryViewModel.ToDate ?? "");
                travelInfo += "</table>";
                travelInfo += "<hr/>";

                htmlbody += travelInfo;

                string scheduleInfo = string.Empty;

                string shipInfo = PrintLine(105);

                //List<SearchResultViewModel> searchResultViewModel = new List<SearchResultViewModel>();
                DisplayViewModel displayViewModel = new DisplayViewModel();
                try
                {
                    await TryMergePort(queryViewModel.FromDate, queryViewModel.ToDate, queryViewModel.FromOrigin, queryViewModel.ToDestination, displayViewModel);

                    var searchResult = displayViewModel.SearchResultViewModel.Where(x => x.CutOffDate == queryViewModel.SelectedValue).FirstOrDefault();
                    if (searchResult != null)
                    {
                        scheduleInfo += "<h2>Schedule Info:</h2>";
                        scheduleInfo += "<table style='width:800px'>";
                        scheduleInfo += string.Format("<tr><td class='x'>Direct/Transship</td><td>{0}</td></tr>", searchResult.DirectTransship ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Vessel/Voyage</td><td>{0}</td></tr>", searchResult.VesselVoyage ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Cut-off Date/Time</td><td>{0}</td></tr>", searchResult.CutOffDate ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>ETD</td><td>{0}</td></tr>", searchResult.ETD ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>ETA CFS</td><td>{0}</td></tr>", searchResult.ETA ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Port of Loading</td><td>{0}</td></tr>", searchResult.PortOfLoading ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Transit Time Cut - Off Origin CFS to Port Of Discharge</td><td>{0}</td></tr>", searchResult.TransitTimeCutOffOriginCFStoPortOfDischarge ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Transit Time Port of Loading to Port of Discharge</td><td>{0}</td></tr>", searchResult.TransitTimePortofLoadingtoPortofDischarge ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Transit Time Port of Loading to CFS Destination</td><td>{0}</td></tr>", searchResult.TransitTimePortofLoadingtoCFSDestination ?? "");
                        scheduleInfo += string.Format("<tr><td class='x'>Transit Time Cut-Off Origin CFS to CFS Destination</td><td>{0}</td></tr>", searchResult.TransitTimeCutOffOriginCFStoCFSDestination ?? "");
                        scheduleInfo += "</table>";
                        scheduleInfo += "<hr/>";
                    }
                    else
                    {
                        foreach (var search in displayViewModel.SearchResultViewModel)
                        {
                            scheduleInfo += "<h2>Schedule Info:</h2>";
                            scheduleInfo += "<table style='width:800px'>";
                            scheduleInfo += string.Format("<tr><td class='x'>Direct/Transship</td><td>{0}</td></tr>", search.DirectTransship ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Vessel/Voyage</td><td>{0}</td></tr>", search.VesselVoyage ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Cut-off Date/Time</td><td>{0}</td></tr>", search.CutOffDate ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>ETD</td><td>{0}</td></tr>", search.ETD ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>ETA CFS</td><td>{0}</td></tr>", search.ETA ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Port of Loading</td><td>{0}</td></tr>", search.PortOfLoading ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Transit Time Cut - Off Origin CFS to Port Of Discharge</td><td>{0}</td></tr>", search.TransitTimeCutOffOriginCFStoPortOfDischarge ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Transit Time Port of Loading to Port of Discharge</td><td>{0}</td></tr>", search.TransitTimePortofLoadingtoPortofDischarge ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Transit Time Port of Loading to CFS Destination</td><td>{0}</td></tr>", search.TransitTimePortofLoadingtoCFSDestination ?? "");
                            scheduleInfo += string.Format("<tr><td class='x'>Transit Time Cut-Off Origin CFS to CFS Destination</td><td>{0}</td></tr>", search.TransitTimeCutOffOriginCFStoCFSDestination ?? "");
                            scheduleInfo += "</table>";
                            scheduleInfo += "<hr/>";
                        }
                    }

                    htmlbody += scheduleInfo;

                    htmlbody += "</body>";
                    htmlbody += "</html>";
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception.Message);
                    Console.WriteLine(exception.StackTrace);
                }

                var to = ConfigurationManager.AppSettings["ToAddress"];
                var subject = ConfigurationManager.AppSettings["Subject"];
                ISmtpHelper smtpHelper = new SmtpHelper(to);

                smtpHelper.SetSubject(subject);

                smtpHelper.SetBody(htmlbody);
                smtpHelper.SendMail();
            }
            catch (Exception exception)
            {
                log.Debug(exception.Message);
                log.Debug(exception.StackTrace);
                throw;
            }

            //shipInfo += PrintRow(new string[] { queryViewModel.Origin, queryViewModel.Destination });
        }

        private string PrintLine(int length)
        {
            return new string('-', length) + "\n";
        }

        private string PrintRow(int length, params string[] columns)
        {
            int width = (length - columns.Length) / columns.Length;
            string row = "|";

            foreach (string column in columns)
            {
                row += AlignCentre(column, width) + "|";
            }

            return row + "\n";
        }

        private string AlignCentre(string text, int width)
        {
            text = text.Length > width ? text.Substring(0, width - 3) + "..." : text;

            if (string.IsNullOrEmpty(text))
            {
                return new string(' ', width);
            }
            else
            {
                return text.PadRight(width - (width - text.Length) / 2).PadLeft(width);
            }
        }

        public ActionResult GetLoactionInfo(string sourceCode, string destinationCode)
        {
            List<Location> locationList = new List<Location>();

            if (!string.IsNullOrEmpty(sourceCode))
            {
                var location = unitOfWork.Repository<Location>().Get(x => x.CODE.Equals(sourceCode, StringComparison.InvariantCultureIgnoreCase));
                locationList.Add(location);
            }

            if (!string.IsNullOrEmpty(destinationCode))
            {
                var location = unitOfWork.Repository<Location>().Get(x => x.CODE.Equals(destinationCode, StringComparison.InvariantCultureIgnoreCase));
                locationList.Add(location);
            }

            return Json(locationList, JsonRequestBehavior.AllowGet);
        }
    }
}