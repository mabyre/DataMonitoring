using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DataMonitoring.DAL.Migrations
{
    public partial class _01_initial_script : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ColorClasses",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    TxtClassName = table.Column<string>(maxLength: 100, nullable: false),
                    BgClassName = table.Column<string>(maxLength: 100, nullable: true),
                    HexColorCode = table.Column<string>(maxLength: 7, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorClasses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Connectors",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true),
                    TimeZone = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    BaseUrl = table.Column<string>(maxLength: 100, nullable: true),
                    AcceptHeader = table.Column<string>(maxLength: 50, nullable: true),
                    AutorisationType = table.Column<int>(nullable: true),
                    AccessTokenUrl = table.Column<string>(maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(maxLength: 100, nullable: true),
                    ClientSecret = table.Column<string>(maxLength: 100, nullable: true),
                    GrantType = table.Column<int>(nullable: true),
                    HttpMethod = table.Column<string>(maxLength: 10, nullable: true),
                    HostName = table.Column<string>(maxLength: 30, nullable: true),
                    DatabaseName = table.Column<string>(maxLength: 30, nullable: true),
                    UseIntegratedSecurity = table.Column<bool>(nullable: true),
                    UserName = table.Column<string>(maxLength: 30, nullable: true),
                    Password = table.Column<string>(maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Connectors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Dashboards",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(nullable: true),
                    TitleDisplayed = table.Column<bool>(nullable: false),
                    TitleColorName = table.Column<string>(maxLength: 50, nullable: false),
                    CurrentTimeManagementDisplayed = table.Column<bool>(nullable: false),
                    Version = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dashboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Styles",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    Code = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Styles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TimeManagements",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeManagements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DashboardLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocalizationCode = table.Column<string>(maxLength: 10, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    DashboardId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DashboardLocalizations_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SharedDashboards",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(nullable: true),
                    CodeLangue = table.Column<string>(nullable: true),
                    TimeZone = table.Column<string>(nullable: true),
                    Skin = table.Column<string>(nullable: true),
                    IsTestMode = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: true),
                    DashboardId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharedDashboards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharedDashboards_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorDefinitions",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    RefreshTime = table.Column<int>(nullable: false),
                    DelayForDelete = table.Column<int>(nullable: false),
                    LastRefreshUtc = table.Column<DateTime>(nullable: true),
                    TimeManagementId = table.Column<long>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    IndicatorDefinitionId1 = table.Column<long>(nullable: true),
                    IndicatorDefinitionId2 = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorDefinitions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorDefinitions_IndicatorDefinitions_IndicatorDefinitionId1",
                        column: x => x.IndicatorDefinitionId1,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorDefinitions_IndicatorDefinitions_IndicatorDefinitionId2",
                        column: x => x.IndicatorDefinitionId2,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorDefinitions_TimeManagements_TimeManagementId",
                        column: x => x.TimeManagementId,
                        principalTable: "TimeManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SlipperyTimes",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TimeBack = table.Column<int>(nullable: false),
                    UnitOfTime = table.Column<int>(nullable: false),
                    TimeManagementId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlipperyTimes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlipperyTimes_TimeManagements_TimeManagementId",
                        column: x => x.TimeManagementId,
                        principalTable: "TimeManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeRanges",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    StartTimeUtc = table.Column<DateTime>(nullable: false),
                    EndTimeUtc = table.Column<DateTime>(nullable: true),
                    TimeManagementId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeRanges_TimeManagements_TimeManagementId",
                        column: x => x.TimeManagementId,
                        principalTable: "TimeManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Widgets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Title = table.Column<string>(maxLength: 200, nullable: true),
                    TitleColorName = table.Column<string>(maxLength: 50, nullable: false),
                    RefreshTime = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    TimeManagementId = table.Column<long>(nullable: true),
                    TitleDisplayed = table.Column<bool>(nullable: false),
                    CurrentTimeManagementDisplayed = table.Column<bool>(nullable: false),
                    LastRefreshTimeIndicator = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Widgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Widgets_TimeManagements_TimeManagementId",
                        column: x => x.TimeManagementId,
                        principalTable: "TimeManagements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocalizationCode = table.Column<string>(maxLength: 10, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    IndicatorDefinitionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorLocalizations_IndicatorDefinitions_IndicatorDefinitionId",
                        column: x => x.IndicatorDefinitionId,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorQueries",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Query = table.Column<string>(maxLength: 3000, nullable: true),
                    ConnectorId = table.Column<long>(nullable: false),
                    IndicatorDefinitionId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorQueries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorQueries_Connectors_ConnectorId",
                        column: x => x.ConnectorId,
                        principalTable: "Connectors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorQueries_IndicatorDefinitions_IndicatorDefinitionId",
                        column: x => x.IndicatorDefinitionId,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorValues",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IndicatorDefinitionId = table.Column<long>(nullable: false),
                    DateUtc = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Group1 = table.Column<string>(nullable: true),
                    Group2 = table.Column<string>(nullable: true),
                    Group3 = table.Column<string>(nullable: true),
                    Group4 = table.Column<string>(nullable: true),
                    Group5 = table.Column<string>(nullable: true),
                    ChartValue = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    TableValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorValues_IndicatorDefinitions_IndicatorDefinitionId",
                        column: x => x.IndicatorDefinitionId,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DashboardWidgets",
                columns: table => new
                {
                    DashboardId = table.Column<long>(nullable: false),
                    WidgetId = table.Column<long>(nullable: false),
                    Column = table.Column<int>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DashboardWidgets", x => new { x.DashboardId, x.WidgetId });
                    table.ForeignKey(
                        name: "FK_DashboardWidgets_Dashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "Dashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DashboardWidgets_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorWidgets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    WidgetId = table.Column<long>(nullable: false),
                    TitleIndicatorDisplayed = table.Column<bool>(nullable: false),
                    TitleIndicatorColor = table.Column<string>(maxLength: 50, nullable: true),
                    TargetValue = table.Column<decimal>(type: "decimal(10,4)", nullable: true),
                    IndicatorDefinitionId = table.Column<long>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    DisplayAxeX = table.Column<bool>(nullable: true),
                    DisplayAxeY = table.Column<bool>(nullable: true),
                    DisplayDataAxeY = table.Column<bool>(nullable: true),
                    TextDataAxeYColor = table.Column<string>(maxLength: 50, nullable: true),
                    DisplayGridLinesAxeY = table.Column<bool>(nullable: true),
                    LabelColumnCode = table.Column<string>(maxLength: 60, nullable: true),
                    LabelColorText = table.Column<string>(maxLength: 50, nullable: true),
                    LabelFontSize = table.Column<int>(nullable: true),
                    DataColumnCode = table.Column<string>(maxLength: 60, nullable: true),
                    DisplayDataLabelInBar = table.Column<bool>(nullable: true),
                    DataLabelInBarColor = table.Column<string>(maxLength: 50, nullable: true),
                    FontSizeDataLabel = table.Column<int>(nullable: true),
                    DecimalMask = table.Column<string>(maxLength: 60, nullable: true),
                    AddTargetBar = table.Column<bool>(nullable: true),
                    TargetBarColor = table.Column<string>(maxLength: 50, nullable: true),
                    BarColor = table.Column<string>(maxLength: 50, nullable: true),
                    SetSumDataToTarget = table.Column<bool>(nullable: true),
                    AddBarStackedToTarget = table.Column<bool>(nullable: true),
                    BarColorStackedToTarget = table.Column<string>(maxLength: 60, nullable: true),
                    AxeXDisplayed = table.Column<bool>(nullable: true),
                    AxeXColor = table.Column<string>(maxLength: 50, nullable: true),
                    AxeYDisplayed = table.Column<bool>(nullable: true),
                    AxeYDataDisplayed = table.Column<bool>(nullable: true),
                    AxeYColor = table.Column<string>(maxLength: 50, nullable: true),
                    AxeFontSize = table.Column<int>(nullable: true),
                    IndicatorChartWidget_DecimalMask = table.Column<string>(maxLength: 60, nullable: true),
                    ChartTargetDisplayed = table.Column<bool>(nullable: true),
                    ChartTargetColor = table.Column<string>(maxLength: 50, nullable: true),
                    ChartAverageDisplayed = table.Column<bool>(nullable: true),
                    ChartAverageColor = table.Column<string>(maxLength: 50, nullable: true),
                    ChartDataColor = table.Column<string>(maxLength: 50, nullable: true),
                    ChartDataFill = table.Column<bool>(nullable: true),
                    Group1 = table.Column<string>(nullable: true),
                    Group2 = table.Column<string>(nullable: true),
                    Group3 = table.Column<string>(nullable: true),
                    Group4 = table.Column<string>(nullable: true),
                    Group5 = table.Column<string>(nullable: true),
                    TargetDisplayed = table.Column<bool>(nullable: true),
                    IndicatorGaugeWidget_Group1 = table.Column<string>(nullable: true),
                    IndicatorGaugeWidget_Group2 = table.Column<string>(nullable: true),
                    IndicatorGaugeWidget_Group3 = table.Column<string>(nullable: true),
                    IndicatorGaugeWidget_Group4 = table.Column<string>(nullable: true),
                    IndicatorGaugeWidget_Group5 = table.Column<string>(nullable: true),
                    HeaderDisplayed = table.Column<bool>(nullable: true),
                    Sequence = table.Column<int>(nullable: true),
                    RowStyleWhenEqualValue = table.Column<string>(maxLength: 100, nullable: true),
                    ColumnCode = table.Column<string>(maxLength: 60, nullable: true),
                    EqualsValue = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorWidgets_IndicatorDefinitions_IndicatorDefinitionId",
                        column: x => x.IndicatorDefinitionId,
                        principalTable: "IndicatorDefinitions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IndicatorWidgets_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "WidgetLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocalizationCode = table.Column<string>(maxLength: 10, nullable: false),
                    Title = table.Column<string>(maxLength: 200, nullable: false),
                    WidgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WidgetLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WidgetLocalizations_Widgets_WidgetId",
                        column: x => x.WidgetId,
                        principalTable: "Widgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BarLabelWidget",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    Sequence = table.Column<int>(nullable: false),
                    LabelTextColor = table.Column<string>(maxLength: 50, nullable: true),
                    UseLabelColorForBar = table.Column<bool>(nullable: false),
                    IndicatorBarWidgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarLabelWidget", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarLabelWidget_IndicatorWidgets_IndicatorBarWidgetId",
                        column: x => x.IndicatorBarWidgetId,
                        principalTable: "IndicatorWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IndicatorBarWidgetColumn",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 60, nullable: true),
                    Filtered = table.Column<bool>(nullable: false),
                    FilteredValue = table.Column<string>(maxLength: 60, nullable: true),
                    IsNumericFormat = table.Column<bool>(nullable: false),
                    IndicatorBarWidgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndicatorBarWidgetColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IndicatorBarWidgetColumn_IndicatorWidgets_IndicatorBarWidgetId",
                        column: x => x.IndicatorBarWidgetId,
                        principalTable: "IndicatorWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TableWidgetColumns",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    NameDisplayed = table.Column<bool>(nullable: false),
                    Code = table.Column<string>(maxLength: 60, nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    Displayed = table.Column<bool>(nullable: false),
                    TextHeaderColor = table.Column<string>(maxLength: 50, nullable: false),
                    TextBodyColor = table.Column<string>(maxLength: 50, nullable: false),
                    ColumnStyle = table.Column<int>(nullable: false),
                    DecimalMask = table.Column<string>(nullable: true),
                    BoldHeader = table.Column<bool>(nullable: false),
                    BoldBody = table.Column<bool>(nullable: false),
                    AlignStyle = table.Column<int>(nullable: false),
                    CellStyleWhenLowerValue = table.Column<string>(maxLength: 100, nullable: true),
                    LowerValue = table.Column<string>(maxLength: 60, nullable: true),
                    LowerColumnCode = table.Column<string>(maxLength: 60, nullable: true),
                    CellStyleWhenHigherValue = table.Column<string>(maxLength: 100, nullable: true),
                    HigherValue = table.Column<string>(maxLength: 60, nullable: true),
                    HigherColumnCode = table.Column<string>(maxLength: 60, nullable: true),
                    CellStyleWhenEqualValue1 = table.Column<string>(maxLength: 100, nullable: true),
                    EqualsValue1 = table.Column<string>(maxLength: 60, nullable: true),
                    EqualsColumnCode1 = table.Column<string>(maxLength: 60, nullable: true),
                    CellStyleWhenEqualValue2 = table.Column<string>(maxLength: 100, nullable: true),
                    EqualsValue2 = table.Column<string>(maxLength: 60, nullable: true),
                    EqualsColumnCode2 = table.Column<string>(maxLength: 60, nullable: true),
                    CellStyleWhenEqualValue3 = table.Column<string>(maxLength: 100, nullable: true),
                    EqualsValue3 = table.Column<string>(maxLength: 60, nullable: true),
                    EqualsColumnCode3 = table.Column<string>(maxLength: 60, nullable: true),
                    IndicatorTableWidgetId = table.Column<long>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    PartialValueColumn = table.Column<string>(nullable: true),
                    TotalValueColumn = table.Column<string>(nullable: true),
                    Filtered = table.Column<bool>(nullable: true),
                    FilteredValue = table.Column<string>(maxLength: 60, nullable: true),
                    IsNumericFormat = table.Column<bool>(nullable: true),
                    TranspositionColumn = table.Column<bool>(nullable: true),
                    TranspositionValue = table.Column<bool>(nullable: true),
                    TranspositionRow = table.Column<bool>(nullable: true),
                    DisplayModel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableWidgetColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableWidgetColumns_IndicatorWidgets_IndicatorTableWidgetId",
                        column: x => x.IndicatorTableWidgetId,
                        principalTable: "IndicatorWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TargetIndicatorChartWidgets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    StartDateUtc = table.Column<DateTime>(nullable: false),
                    StartTargetValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    EndDateUtc = table.Column<DateTime>(nullable: false),
                    EndTargetValue = table.Column<decimal>(type: "decimal(10,4)", nullable: false),
                    IndicatorChartWidgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TargetIndicatorChartWidgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TargetIndicatorChartWidgets_IndicatorWidgets_IndicatorChartWidgetId",
                        column: x => x.IndicatorChartWidgetId,
                        principalTable: "IndicatorWidgets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BarLabelWidgetLocalization",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocalizationCode = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    BarLabelWidgetId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarLabelWidgetLocalization", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BarLabelWidgetLocalization_BarLabelWidget_BarLabelWidgetId",
                        column: x => x.BarLabelWidgetId,
                        principalTable: "BarLabelWidget",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TableWidgetColumnLocalizations",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    LocalizationCode = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(maxLength: 60, nullable: false),
                    TableWidgetColumnId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableWidgetColumnLocalizations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TableWidgetColumnLocalizations_TableWidgetColumns_TableWidgetColumnId",
                        column: x => x.TableWidgetColumnId,
                        principalTable: "TableWidgetColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BarLabelWidget_IndicatorBarWidgetId",
                table: "BarLabelWidget",
                column: "IndicatorBarWidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BarLabelWidgetLocalization_BarLabelWidgetId",
                table: "BarLabelWidgetLocalization",
                column: "BarLabelWidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardLocalizations_DashboardId",
                table: "DashboardLocalizations",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_DashboardWidgets_WidgetId",
                table: "DashboardWidgets",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorBarWidgetColumn_IndicatorBarWidgetId",
                table: "IndicatorBarWidgetColumn",
                column: "IndicatorBarWidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorDefinitions_IndicatorDefinitionId1",
                table: "IndicatorDefinitions",
                column: "IndicatorDefinitionId1");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorDefinitions_IndicatorDefinitionId2",
                table: "IndicatorDefinitions",
                column: "IndicatorDefinitionId2");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorDefinitions_TimeManagementId",
                table: "IndicatorDefinitions",
                column: "TimeManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorLocalizations_IndicatorDefinitionId",
                table: "IndicatorLocalizations",
                column: "IndicatorDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorQueries_ConnectorId",
                table: "IndicatorQueries",
                column: "ConnectorId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorQueries_IndicatorDefinitionId",
                table: "IndicatorQueries",
                column: "IndicatorDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorValues_IndicatorDefinitionId_DateUtc",
                table: "IndicatorValues",
                columns: new[] { "IndicatorDefinitionId", "DateUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorWidgets_IndicatorDefinitionId",
                table: "IndicatorWidgets",
                column: "IndicatorDefinitionId");

            migrationBuilder.CreateIndex(
                name: "IX_IndicatorWidgets_WidgetId",
                table: "IndicatorWidgets",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_SharedDashboards_DashboardId",
                table: "SharedDashboards",
                column: "DashboardId");

            migrationBuilder.CreateIndex(
                name: "IX_SlipperyTimes_TimeManagementId",
                table: "SlipperyTimes",
                column: "TimeManagementId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TableWidgetColumnLocalizations_TableWidgetColumnId",
                table: "TableWidgetColumnLocalizations",
                column: "TableWidgetColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_TableWidgetColumns_IndicatorTableWidgetId",
                table: "TableWidgetColumns",
                column: "IndicatorTableWidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetIndicatorChartWidgets_IndicatorChartWidgetId",
                table: "TargetIndicatorChartWidgets",
                column: "IndicatorChartWidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeRanges_TimeManagementId",
                table: "TimeRanges",
                column: "TimeManagementId");

            migrationBuilder.CreateIndex(
                name: "IX_WidgetLocalizations_WidgetId",
                table: "WidgetLocalizations",
                column: "WidgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Widgets_TimeManagementId",
                table: "Widgets",
                column: "TimeManagementId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BarLabelWidgetLocalization");

            migrationBuilder.DropTable(
                name: "ColorClasses");

            migrationBuilder.DropTable(
                name: "DashboardLocalizations");

            migrationBuilder.DropTable(
                name: "DashboardWidgets");

            migrationBuilder.DropTable(
                name: "IndicatorBarWidgetColumn");

            migrationBuilder.DropTable(
                name: "IndicatorLocalizations");

            migrationBuilder.DropTable(
                name: "IndicatorQueries");

            migrationBuilder.DropTable(
                name: "IndicatorValues");

            migrationBuilder.DropTable(
                name: "SharedDashboards");

            migrationBuilder.DropTable(
                name: "SlipperyTimes");

            migrationBuilder.DropTable(
                name: "Styles");

            migrationBuilder.DropTable(
                name: "TableWidgetColumnLocalizations");

            migrationBuilder.DropTable(
                name: "TargetIndicatorChartWidgets");

            migrationBuilder.DropTable(
                name: "TimeRanges");

            migrationBuilder.DropTable(
                name: "WidgetLocalizations");

            migrationBuilder.DropTable(
                name: "BarLabelWidget");

            migrationBuilder.DropTable(
                name: "Connectors");

            migrationBuilder.DropTable(
                name: "Dashboards");

            migrationBuilder.DropTable(
                name: "TableWidgetColumns");

            migrationBuilder.DropTable(
                name: "IndicatorWidgets");

            migrationBuilder.DropTable(
                name: "IndicatorDefinitions");

            migrationBuilder.DropTable(
                name: "Widgets");

            migrationBuilder.DropTable(
                name: "TimeManagements");
        }
    }
}
