using System;
using System.Collections.Generic;
using System.Text;
using DataMonitoring.Model;

namespace DataMonitoring.Test
{
    public static class DataMonitoringModel
    {
        public static Widget NewWidgetUniqueTable()
        {
            return new Widget
            {
                Id = 1,
                Type = WidgetType.Table,
                RefreshTime = 1,
                LastRefreshTimeIndicator = false,
                Title = "Test widget",
                TitleDisplayed = true,
                TitleColorName = "Black",
                CurrentTimeManagementDisplayed = false,
                TimeManagementId = 1,
            };
        }

        public static IndicatorTableWidget NewIndicatorTableWidget()
        {
            return new IndicatorTableWidget
            {
                Id = 1,
                Widget = NewWidgetUniqueTable(),
                HeaderDisplayed = true,
                Sequence = 1,
                TitleIndicatorDisplayed = false,
                WidgetId = 1,
                TableWidgetColumns = NewListTableWidgetColumns()
            };
        }

        public static ICollection<TableWidgetColumn> NewListTableWidgetColumns()
        {
            var list = new List<TableWidgetColumn>();

            list.Add
            (
                new IndicatorTableWidgetColumn
                {
                    Id = 1,
                    Code = "ZONE",
                    Displayed = false,
                    Filtered = true,
                    FilteredValue = "1",
                    Sequence = 1,
                    Name = "Zone",
                }
            );

            list.Add
            ( 
                new IndicatorTableWidgetColumn
                {
                    Id = 2,
                    Code = "OUTILS",
                    Displayed = false,
                    Filtered = false,
                    FilteredValue = "",
                    Sequence = 2,
                    Name = "Outils",
                } 
            );

            list.Add
            ( 
                new IndicatorTableWidgetColumn
                {
                    Id = 3,
                    Code = "CAT",
                    Displayed = true,
                    Filtered = false,
                    FilteredValue = "",
                    Sequence = 3,
                    Name = "Categorie",
                    TranspositionRow = false
                } 
            );

            list.Add
            ( 
                new IndicatorTableWidgetColumn
                {
                    Id = 4,
                    Code = "REG1",
                    Displayed = true,
                    Filtered = false,
                    FilteredValue = "",
                    Sequence = 4,
                    Name = "Regroupement",
                    TranspositionColumn = false,
                } 
            );

            list.Add
            ( 
                new IndicatorTableWidgetColumn
                {
                    Id = 5,
                    Code = "valeur",
                    Displayed = true,
                    Filtered = false,
                    FilteredValue = "",
                    Sequence = 5,
                    Name = "Valeur",
                    IsNumericFormat = true,
                    TranspositionValue = false,
                } 
            );

            return list;
        }

    }
}
