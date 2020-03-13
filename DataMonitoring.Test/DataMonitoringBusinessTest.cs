using DataMonitoring.Business;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using DataMonitoring.Model;

namespace DataMonitoring.Test
{
    [TestFixture]
    public class DataMonitoringBusinessTest
    {
        private MonitorBusiness MonitorBusiness { get; set; }
        private TimeManagementBusiness TimeManagementBusiness { get; set; }

        [SetUp]
        public void SetUpEachTest()
        {
            MonitorBusiness = Substitute.ForPartsOf<MonitorBusiness>();
            TimeManagementBusiness = Substitute.ForPartsOf<TimeManagementBusiness>();
        }

        private const string JSON = @"[
                { 'ZONE':'1','OUTILS':'1','CAT':'TOUR','REG1':'TO DO' ,'valeur':110},
                { 'ZONE':'1','OUTILS':'1','CAT':'TOUR','REG1':'DONE'  ,'valeur':110},
                { 'ZONE':'1','OUTILS':'2','CAT':'TOUR','REG1':'TO DO' ,'valeur':110},
                { 'ZONE':'1','OUTILS':'2','CAT':'TOUR','REG1':'DONE'  ,'valeur':110},
                { 'ZONE':'2','OUTILS':'1','CAT':'TOUR','REG1':'DONE'  ,'valeur':5},
                { 'ZONE':'3','OUTILS':'2','CAT':'TOUR','REG1': null   ,'valeur':1}
                ]";

        private const string JSONTranspo = @"[
                { 'CAT':'TOUR','REG1':'TO DO' ,'valeur':110},
                { 'CAT':'TOUR','REG1':'DONE'  ,'valeur':110},
                { 'CAT':'TOUR','REG1':'TO DO' ,'valeur':110},
                { 'CAT':'TOUR','REG1':'DONE'  ,'valeur':110},
                { 'CAT':'TOUR','REG1':'DONE'  ,'valeur':5},
                { 'CAT':'TOUR','REG1': null  ,'valeur':1}
                ]";

        [Test]
        public void FilterDataTest()
        {
            var jsonData = JArray.Parse(JSON);

            var list = DataMonitoringModel.NewIndicatorTableWidget().TableWidgetColumns.OfType<IIndicatorColumn>().ToList();
            var data = MonitorBusiness.FilterData(jsonData, list);

            Assert.AreEqual(4, data.ToList().Count);
        }

        [Test]
        public void FilerColumnTest()
        {
            var jsonData = JArray.Parse(JSON);

            var listColumns = MonitorComponent.GetColumnsNotDisplayed(DataMonitoringModel.NewIndicatorTableWidget());

            var data = MonitorBusiness.FilterDataColumn(jsonData, listColumns);
            Assert.AreEqual(3, data.First().ToObject<JObject>().Properties().Count());
        }

        [Test]
        public void AggregationDataTest()
        {
            var jsonData = JArray.Parse(JSON).ToList();

            var listColumns = MonitorComponent.GetColumnsToAggregate(DataMonitoringModel.NewIndicatorTableWidget());
            var list = DataMonitoringModel.NewIndicatorTableWidget().TableWidgetColumns.OfType<IIndicatorColumn>().ToList();

            var data = MonitorBusiness.Aggregation(jsonData, list, listColumns);

            Assert.AreEqual(3, data.ToList().Count);
        }

        object GetTranspositionColumn()
        {
            return new { RowColumnName = "CAT", ColColumnName = "REG1", DataColumnName = "valeur" };
        }

        [Test]
        public void TranspositionDataTest()
        {
            var jsonData = JArray.Parse(JSONTranspo);

            var data = MonitorBusiness.TranspositionData(jsonData, "CAT", "REG1", "valeur");

            Assert.AreEqual(1, data.ToList().Count);
        }

        [Test]
        public void GetDataAsyncTest()
        {
            TimeManagementBusiness.GetTimeRangeAsync(Arg.Any<long>()).Returns(new TimeRange());
            MonitorBusiness.GetSnapshotValueIndicator(Arg.Any<long>(), Arg.Any<DateTime>()).Returns(JArray.Parse(JSON));

            var data = MonitorBusiness.GetTableWidgetData(DataMonitoringModel.NewIndicatorTableWidget(),new TimeRange());

            Assert.AreEqual(2, data.ToList().Count);
            Assert.AreEqual(3, data.First().ToObject<JObject>().Properties().Count());
            Assert.AreEqual(220, data.First().ToObject<JObject>().Property("valeur").Value.Value<decimal>());
        }

        [Test]
        public void GetDataToTestWidgetTest()
        {
            var data = MonitorBusiness.SimulateDataToTestTableWidget(DataMonitoringModel.NewIndicatorTableWidget());
        }
    }
}