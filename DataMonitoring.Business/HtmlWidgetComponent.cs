// 
//  Créé les chaînes Html pour le contenu des Widgets
//  Très spécifique de SmartAdmin, par exemple text-align-left
// se trouve dans \wwwroot\css\smartadmin.css
//
using DataMonitoring.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace DataMonitoring.Business
{
    public static class HtmlWidgetComponent
    {
        public static string ScriptBegin()
        {
            return "<script>";
        }

        public static string ScriptEnd()
        {
            return "</script>";
        }

        public static string TableBegin()
        {
            return "<table class='table font-md'>";
        }

        public static string TableEnd()
        {
            return "</table>";
        }

        public static string TableLineBegin()
        {
            return "<tr>";
        }

        public static string TableLineEnd()
        {
            return "</tr>";
        }

        public static string GetMessageLine(string message, int nbColumn, ColorHtml color)
        {
            var classDef = color != null && !string.IsNullOrEmpty(color.TxtClassName) ? color.TxtClassName : string.Empty;

            return string.IsNullOrEmpty(classDef)
                ? $"<td colspan={nbColumn}>{message}</td>"
                : $"<td colspan={nbColumn} class='{classDef}'>{message}</td>";
        }

        public static string TableColumnBegin( int colspan = 0 )
        {
            return colspan == 0 ? "<td>" : $"<td colspan={colspan}>";
        }

        public static string TableColumnEnd()
        {
            return "</td>";
        }

        public static string TableColumn
            (
                string text, 
                string specificRowStyle, 
                ColumnStyle columnStyle, 
                string specificColumnStyle, 
                string decimalMask, 
                ColorHtml color, 
                bool bold, 
                AlignStyle alignStyle, 
                bool bottom = false
            )
        {
            var txt = "&nbsp;";
            string htlmContent = string.Empty;

            if (!string.IsNullOrEmpty(text))
            {
                if (decimal.TryParse(text, out var val))
                {
                    txt = val.ToString(decimalMask);
                    txt = txt.Replace(",", ".");
                }
                else
                {
                    txt = text;
                }
            }

            var styleDef = string.Empty;

            if (!string.IsNullOrEmpty(specificColumnStyle))
            {
                styleDef = specificColumnStyle;
            }
            else
            {
                if (string.IsNullOrEmpty(specificRowStyle))
                {
                    styleDef = CreateHtmlColumnByStyle(columnStyle, color, bold);
                }
                else
                {
                    styleDef = columnStyle == ColumnStyle.Text
                        ? specificRowStyle
                        : CreateHtmlColumnByStyle( columnStyle, color, bold );
                }
            }

            var classDef = color != null && !string.IsNullOrEmpty(color.TxtClassName) ? color.TxtClassName : string.Empty;
            classDef += $" {GetTextAlignClass(alignStyle)}";

            // _BRY_ Progess bar un peu plus "sympa"
            string style = string.Empty;
            if ( columnStyle == ColumnStyle.ProgressBar )
            {
                style = bottom ? "style='vertical-align:bottom;width:58%'" : "style='vertical-align:middle;width:58%'";
            }
            else
            {
                style = bottom ? "style='vertical-align:bottom'" : "style='vertical-align:middle'";
            }

            htlmContent = string.IsNullOrEmpty( classDef )
                ? $"<td {style}>{styleDef.Replace( "%%value%%", txt )}</td>"
                : $"<td class='{classDef}' {style}>{styleDef.Replace( "%%value%%", txt )}</td>";

            return htlmContent;
        }

        public static string GetTextAlignClass(AlignStyle align)
        {
            switch (align)
            {
                case AlignStyle.Left:
                    return "text-align-left";
                case AlignStyle.Center:
                    return "text-align-center";
                case AlignStyle.Right:
                    return "text-align-right";
                default:
                    throw new ArgumentOutOfRangeException(nameof(align), align, null);
            }
        }

        public static string MyTextAlignClass( AlignStyle align )
        {
            switch ( align )
            {
                case AlignStyle.Left:
                    return "text-align:left";
                case AlignStyle.Center:
                    return "text-align:center";
                case AlignStyle.Right:
                    return "text-align:right";
                default:
                    throw new ArgumentOutOfRangeException( nameof( align ), align, null );
            }
        }

        private static string CreateHtmlColumnByStyle( ColumnStyle style, ColorHtml color, bool bold )
        {
            switch ( style )
            {
                case ColumnStyle.Text:
                    return bold ? "<strong>%%value%%</strong>" : "%%value%%";

                case ColumnStyle.ProgressBar:
                    var classProgressBar = color != null && !string.IsNullOrEmpty( color.BgClassName ) ? color.BgClassName : "bg-color-blue";

                    var styleProgressBar = "<div class='progress' style='margin:0px;padding:0px'>";
                    // data-transitiongoal='25' saprogressbar='' ne marche pas ...
                    styleProgressBar += $"<div class='progress-bar {classProgressBar}' role='progressbar' style='width:%%value%%%'>%%value%%%</div>";
                    styleProgressBar += "</div>";

                    return styleProgressBar;

                case ColumnStyle.Badge:

                    var styleBadge = bold ? "<strong>%%value%%</strong>" : "%%value%%";
                    var classBadge = "bg-color-blue";

                    if ( color != null && !string.IsNullOrEmpty( color.BgClassName ) )
                    {
                        classBadge = color.BgClassName;
                    }

                    return $"<span class='badge {classBadge} font-sm' style='color:#000'>{styleBadge}</span>";

                default:
                    throw new ArgumentOutOfRangeException( nameof( style ), style, null );
            }
        }

        public static string HtmlTitleH3( string title, ColorHtml colorHtml, AlignStyle align )
        {
            var content = string.Empty;
            var className = string.Empty;
            var style = string.Empty;

            if ( string.IsNullOrEmpty( colorHtml.TxtClassName ) == false )
            {
                className += $" class='{colorHtml.TxtClassName}'";
            }

            if ( string.IsNullOrEmpty( colorHtml.HexColorCode ) == false )
            {
                style = $" style='color:{colorHtml.HexColorCode}'";
            }
            style += $";{MyTextAlignClass( align )}";

            content = $"<h3 {className} {style}'>{title}</h3>";

            return content;
        }

        public static string HtmlTitleH1(string title, ColorHtml color, AlignStyle align)
        {
            string htmlContent = string.Empty;
            var classDef = color != null && !string.IsNullOrEmpty(color.TxtClassName) ? color.TxtClassName : string.Empty;
            classDef += $" {GetTextAlignClass(align)}";

            htmlContent = string.IsNullOrEmpty( classDef )
                ? $"<h1>{title}</h1>"
                : $"<h1 class='{classDef}'>{title}</h1>";

            return htmlContent;
        }

        public static string DivRowBegin()
        {
            return "<div class='row'>";
        }

        public static string DivBegin()
        {
            return "<div>";
        }

        public static string DivByCol(int column)
        {
            return $"<div class='col-sm-{column}'>";
        }

        public static string DivEnd()
        {
            return "</div>";
        }

        public static string ChartName(string chartName)
        {
            return $"<div style='text-align:center;'><canvas id='{chartName}' class='widget-canvas'></canvas></div>";
        }

        public static string BarChartScript(string chartName, string position, string data, string options, string plugin)
        {
            var chartVarName = string.IsNullOrEmpty(position) ? "barchart" : $"barchart{position}";

            var content = $"if ({chartVarName}) {{ {chartVarName}.destroy(); }}";
            content += $"var ctx=document.getElementById('{chartName}').getContext('2d');";
            content += $"var {chartVarName} = new Chart(ctx, {{type:'bar',data:{{{data}}},options:{{{options}}},plugins:[{{{plugin}}}]}});";

            return content;
        }

        public static string Labels(List<string> labels)
        {
            return $"labels: [{ListDataToString(labels)}],";
        }

        public static string BarLineData(string datasetLabel, List<string> listData, string color)
        {
            var content = "{";

            content += $"label: '{datasetLabel}',";
            content += $"data:[{ListDataToNumeric( listData)}],";

            content += "datalabels:{display:false},";
            content += $"borderColor:'{color}',borderWidth: 5,borderDash:[10, 10],";
            content += "fill:false,radius:0,pointStyle:'dash',type:'line'";

            content += "},";

            return content;
        }

        public static string BarData(string datasetLabel, List<string> listData, List<string> colors, string stackName = "")
        {
            var content = "{";

            content += $"label: '{datasetLabel}',";
            content += $"data:[{ListDataToNumeric( listData)}],";

            content += "datalabels:{display:false},";
            content += $"backgroundColor:[{ListDataToString(colors)}],";

            if (!string.IsNullOrEmpty(stackName))
            {
                content += $"stack: '{stackName}'";
            }

            content += "},";

            return content;
        }

        public static string BarData(string datasetLabel, List<string> listData, string color, string stackName = "")
        {
            var content = "{";

            content += $"label:'{datasetLabel}',";
            content += $"data:[{ListDataToNumeric(listData)}],";

            content += "datalabels:{display:false},";
            content += $"backgroundColor:'{color}',";

            if (!string.IsNullOrEmpty(stackName))
            {
                content += $"stack: '{stackName}'";
            }

            content += "},";

            return content;
        }

        public static string BarOption(bool displayAxeX, bool displayAxeY, bool displayGridLineY, bool displayDataAxeY, string dataAxeYColor, string labelColor, int labelFontSize, bool isStackedAxeY)
        {
            var content = "layout:{padding:{left: 0,right: 0,top: 20,bottom: 50}},tooltips: false,";
            content += "title: { display: false },legend: { display: false },";

            content += "scales:{";
            content += $"xAxes: [{{display: {BoolValue(displayAxeX)},";
            content += $"ticks:{{display: false,fontSize: {labelFontSize},fontColor: '{labelColor}'}},gridLines:{{display: false}}}}],";
            content += $"yAxes: [{{display: {BoolValue(displayAxeY)},stacked: {BoolValue(isStackedAxeY)},";
            content += $"ticks:{{display: {BoolValue(displayDataAxeY)},fontSize: {labelFontSize},fontColor: '{dataAxeYColor}'}},";
            content += $"gridLines:{{display: {BoolValue(displayGridLineY)}}}}}]";

            content += "}";

            return content;
        }

        public static string PluginRegisterScript(string afterDatasetsUpdate, string afterDraw)
        {
            var content = "Chart.plugins.register({";

            content += afterDatasetsUpdate;
            content += afterDraw;

            content += "});";

            return content;
        }

        public static string AfterDatasetUpdateScript(List<int> dataSetIndex, int targetIndex, int width, int widthTarget)
        {
            var content = "afterDatasetsUpdate: function(chart) {";

            foreach (var index in dataSetIndex)
            {
                content += $"Chart.helpers.each(chart.getDatasetMeta({index}).data, function(rectangle, index) {{";
                content += $"if (index == {targetIndex}){{rectangle._view.width = rectangle._model.width = {widthTarget};}}";
                content += $"else {{rectangle._view.width = rectangle._model.width = {width};}}";
                content += "});";
            }

            content += "},";

            return content;
        }

        public static string AfterDraw(List<string> labelBars, bool displayDataLabel, string dataLabelColor, int dataLabelFontSize, int targetIndex, string dataLabelTargetColor, int labelFontSize, List<string> labelColors)
        {
            var content = "afterDraw: function(chartInstance) {var ctx = chartInstance.chart.ctx;";
            content += "var meta = chartInstance.chart.getDatasetMeta(0);";
            content += "var yScale = chartInstance.chart.scales[meta.yAxisID];";

            if (displayDataLabel)
            {
                content += $"ctx.font='{dataLabelFontSize}px Arial';ctx.textBaseline='middle';";
                content += "chartInstance.data.datasets.forEach(function(dataset) {";

                foreach (var label in labelBars)
                {
                    content += DataLabelToScript( label, dataLabelColor, targetIndex, dataLabelTargetColor);
                }

                content += "});";
            }

            content += $"ctx.font='{labelFontSize}px Arial';";
            content += "ctx.textBaseline='middle';ctx.textAlign='center';";
            content += "Chart.helpers.each(meta.data, function(bar, index) {";
            content += "const label = chartInstance.chart.config.data.labels[index];";

            content += "if (label==='TARGET'){";
            content += $"ctx.fillStyle ='{dataLabelTargetColor}';";
            content += "ctx.textAlign ='left';ctx.fillText(label, bar._model.x + 20, bar._model.y + 20);}";
            content += "else {";

            var index = 0;
            foreach (var color in labelColors)
            {
                content += $"if (index==={index}){{";
                content += $"ctx.fillStyle = '{color}';";
                content += "ctx.fillText(label, bar._model.x, yScale.getPixelForValue(yScale.min) + 20);}";
                index++;
            }

            content += "}";

            content += "});";

            content += "},";  // afterDraw: function(chartInstance)

            return content;
        }

        private static string DataLabelToScript(string labelBar, string dataLabelColor, int targetIndex, string dataLabelTargetColor)
        {
            var content = $"if (dataset.label=='{labelBar}'){{";

            content += "for (var i = 0; i < dataset.data.length; i++){";
            content += "var model=dataset._meta[Object.keys(dataset._meta)[0]].data[i]._model;";
            content += $"if (i==={targetIndex}){{";
            content += $"ctx.textAlign='left';ctx.fillStyle='{dataLabelTargetColor}';";
            content += "ctx.fillText(dataset.data[i], model.x + 20, model.y + 60);";
            content += "}else{ctx.textAlign='center';";
            content += $"ctx.fillStyle = '{dataLabelColor}';";
            content += "ctx.fillText(dataset.data[i], model.x, yScale.getPixelForValue(yScale.min) / 2);}}}";

            return content;
        }

        private static string ListDataToNumeric(List<string> listData)
        {
            var content = string.Empty;
            var index = 0;

            foreach (var data in listData)
            {
                if (index == 0)
                {
                    content += $"{data}";
                }
                else
                {
                    content += $",{data}";
                }

                index++;
            }

            return content;
        }

        private static string ListDataToString(List<string> listData)
        {
            var content = string.Empty;
            var index = 0;

            foreach (var data in listData)
            {
                if (index == 0)
                {
                    content += $"'{data}'";
                }
                else
                {
                    content += $",'{data}'";
                }

                index++;
            }

            return content;
        }

        public static string BoolValue(bool value)
        {
            return value ? "true" : "false";
        }

        public static string CreateVariableData(IEnumerable<JToken> data, string decimalMask, TimeZoneInfo timeZoneInfo, bool localDate)
        {
            var content = "var result = [";
            var index = 0;

            foreach (var line in data)
            {
                var dateUtc = line.SelectToken("DateUtc").Value<DateTime>();
                var date = dateUtc;

                if (localDate)
                {
                    date = timeZoneInfo != null
                        ? TimeZoneInfo.ConvertTimeFromUtc(dateUtc, timeZoneInfo)
                        : dateUtc.ToLocalTime();
                }
                
                var valeur = line.SelectToken("Value").Value<decimal>().ToString(decimalMask);
                valeur = valeur.Replace(",", ".");

                if (index != 0) { content += ","; }

                content += $"{{ x: '{date.ToString("yyyy-MM-dd HH:mm")}', y: {valeur}}}";
                index++;
            }

            content += "];";

            content += "var labels = result.map(function(el) { return el.x; });";
            content += "var data = result.map(function(el) { return el.y; });";

            return content;
        }

        public static string getChartConfig(string labels, string datasets, string options, string plugin)
        {
            return $"var config={{type:'line',data:{{labels:{labels},datasets:[{datasets}]}},options:{{{options}}},plugins:[{{{plugin}}}]}};";
        }

        public static string ScriptChart(string chartName, string position)
        {
            var chartVarName = string.IsNullOrEmpty(position) ? "myChart" : $"myChart{position}";

            var content = $"if ({chartVarName}) {{ {chartVarName}.destroy(); }}";
            content += $"var ctx=document.getElementById('{chartName}').getContext('2d');";
            content += $"var {chartVarName} = new Chart(ctx,config)";

            return content;
        }

        public static string ScriptChartOptions(bool displayLegend, bool displayAxeX, bool displayAxeY, int fontSize,
            string colorAxeX, string colorAxeY, bool displayDataAxeY, DateTime? dateMin, DateTime? dateMax,
            int yMinValue)
        {
            var content = $"title: {{display:false}},legend:{{display:{BoolValue(displayLegend)}}},";

            content += "scales:{";
            content += $"xAxes: [{{display:{BoolValue(displayAxeX)},type:'time',";
            content += $"time:{{unit: 'hour',displayFormats:{{hour: 'HH:mm'}}";
            if (dateMin.HasValue) content += $",min: '{dateMin.Value.ToString("yyyy-MM-dd HH:mm")}'";
            if (dateMax.HasValue)
            {
                content += $",max: '{dateMax.Value.ToString("yyyy-MM-dd HH:mm")}'";
            }
            content += "},";
            content += $"ticks:{{fontColor: '{colorAxeX}',fontSize: {fontSize},}},";
            content += "gridLines:{display:false} }],";

            content += $"yAxes: [{{display:{BoolValue(displayAxeY)},gridLines:{{display:false}},";
            content += $"ticks:{{display: {BoolValue(displayDataAxeY)},beginAtZero: true, min:{yMinValue}, fontColor: '{colorAxeY}',fontSize: {fontSize}}},}}]";
            content += "}";

            return content;
        }

        public static string ScriptChartData(string label, string data, bool fill, string color,int borderSize, bool isDashed)
        {
            var content = $"{{label: '{label}',data: {data},borderWidth: {borderSize},fill: {BoolValue(fill)},backgroundColor: '{color}',";
            //if (isDashed) content += "borderDash:[10,10],";
            //content += "borderDash:[10,10],";
            //content += $"pointBackgroundColor: 'rgba(220,220,220,1)',";
            content += $"pointStyle: 'circle',";
            content += $"pointBackgroundColor: '#ff0000',";
            content += $"pointBorderColor: '#fff',";

            content += $"borderColor: '{color}',"; //,radius: 0,}},";

            content += $"}},";

            return content;
        }

        public static string ScriptChartDataset(DateTime date, decimal value, string decimalMask)
        {
            var valeur = value.ToString(decimalMask);
            valeur = valeur.Replace(",", ".");
            var content = $"{{x: '{date.ToString("yyyy-MM-dd HH:mm")}', y: {valeur}}},";

            return content;
        }

        public static string ScriptChartDataset(string data)
        {
            return $"[{data}]";
        }

        public static string ScriptPluginChart(int fontsize)
        {
            var content = "afterDraw: function(chartInstance) {var ctx = chartInstance.chart.ctx;";

            content += $"ctx.font = '{fontsize}px Arial';ctx.textBaseline = 'top';";
            content += "var meta = chartInstance.chart.getDatasetMeta(0);";
            content += "var xScale = chartInstance.chart.scales[meta.xAxisID];";
            content += "chartInstance.data.datasets.forEach(function(dataset) {";
           
            content += "if (dataset.label == 'TARGET'){";
            content += "ctx.textAlign = 'left';ctx.fillStyle = dataset.borderColor;";
            content += "ctx.fillText(dataset.label, xScale.getPixelForValue(xScale.max) - 70, 0);}";

            content += "if (dataset.label == 'AVERAGE'){";
            content += "ctx.textAlign = 'left';ctx.fillStyle = dataset.borderColor; "; 
            content += "ctx.fillText(dataset.label, xScale.getPixelForValue(xScale.max) - 160, 0);}";

            content += "})}";

            return content;
        }

        public static string GaugeOption(int targetValue, string targetColor, 
            string range1Color, int range1MinValue, int range1MaxValue, 
            bool displayRange2, string range2Color, int range2MinValue, int range2MaxValue,
            bool displayRange3, string range3Color, int range3MinValue, int range3MaxValue)
        {
            var gaugeColor = "#FFFFFF";
            var content = "var opts = {angle: 0, lineWidth: 0.1, radiusScale: 0.9, pointer:";
            content += "{length: 0.5, strokeWidth: 0.08, ";
            content += $"color: '{gaugeColor}', textColor: '{gaugeColor}'";
            content += ", font: '60px Arial', },";

            content += "staticZones: [";
            content += $"{{ strokeStyle: '{range1Color}', min: {range1MinValue}, max: {range1MaxValue}}},";
            if (displayRange2)
            {
                content += $"{{ strokeStyle: '{range2Color}', min: {range2MinValue}, max: {range2MaxValue}}},";
            }
            if (displayRange3)
            {
                content += $"{{ strokeStyle: '{range3Color}', min: {range3MinValue}, max: {range3MaxValue}}},";
            }
            content += "],";

            content += "staticLabels:{font: '28px sans-serif',";

            var valTarget = targetValue != 0 ? targetValue.ToString() : string.Empty;

            content += $"labels: [{valTarget}], colors: ['{targetColor}'], infos: [''],";
            content += "color: '#FFFFFF', fractionDigits: 0},";
            
            content += "limitMax: true, limitMin: true, strokeColor: '#EEEEEE', generateGradient: true, };";
            
            return content;
        }

        public static string ScriptGauge(string chartName, string position, int valueMin, int valueMax, int value)
        {
            var _chartName = string.IsNullOrEmpty(position) ? "gauge" : $"gauge{position}";

            var content = $"if ({_chartName}) {{ {_chartName} = null; }}";
            content += $"var target = document.getElementById('{chartName}');";
            content += $"var {_chartName} = new Gauge(target).setOptions(opts);";
            content += $"{_chartName}.minValue = {valueMin};";
            content += $"{_chartName}.maxValue = {valueMax};";
            content += $"{_chartName}.animationSpeed = 15;";
            content += $"{_chartName}.set({value});";

            return content;
        }
    }
}
