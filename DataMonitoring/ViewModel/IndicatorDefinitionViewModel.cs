using System.Collections.Generic;
using DataMonitoring.Model;

namespace DataMonitoring.ViewModel
{
    public class IndicatorDefinitionViewModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public IndicatorType Type { get; set; }

        public int RefreshTime { get; set; }

        public int DelayForDelete { get; set; }

        public IndicatorCalculatedViewModel IndicatorCalculated { get; set; }

        public List<QueryConnectorViewModel> QueryConnectors { get; set; }

        public List<TitleLocalizationViewModel> TitleLocalizations { get; set; }

        public long? TimeManagementId { get; set; }

        public string ConnectorTitleListToDisplayed { get; set; }
    }

    public class QueryConnectorViewModel
    {
        public long ConnectorId { get; set; }
        public string Query { get; set; }
    }

    public class IndicatorCalculatedViewModel
    {
        public long IndicatorOneId { get; set; }
        public long IndicatorTwoId { get; set; }
    }
}
