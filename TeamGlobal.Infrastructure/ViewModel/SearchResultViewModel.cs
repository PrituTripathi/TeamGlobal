using System.Collections.Generic;

namespace TeamGlobal.Infrastructure.ViewModel
{
    public class SearchResultViewModel
    {
        public string DirectTransship { get; set; }
        public string VesselVoyage { get; set; }
        public string CutOffDate { get; set; }
        public string ETD { get; set; }
        public string ETA { get; set; }
        public string PortOfLoading { get; set; }
        public string TransitTimeCutOffOriginCFStoPortOfDischarge { get; set; }
        public string TransitTimePortofLoadingtoPortofDischarge { get; set; }
        public string TransitTimePortofLoadingtoCFSDestination { get; set; }
        public string TransitTimeCutOffOriginCFStoCFSDestination { get; set; }
        public bool IsFromIndia { get; set; }
        public bool IsRouteFromAtoB { get; set; }
    }

    public class DisplayViewModel
    {
        public DisplayViewModel()
        {
            SearchResultViewModel = new List<ViewModel.SearchResultViewModel>();
        }

        public List<SearchResultViewModel> SearchResultViewModel { get; set; }

        public string FromDate { get; set; }
        public string ToDate { get; set; }
        public string FromSource { get; set; }
        public string ToDestination { get; set; }
        public string RoutedBy { get; set; }

        public bool IsFromIndia { get; set; }
        public bool IsMergeResut { get; set; }
       
    }
}