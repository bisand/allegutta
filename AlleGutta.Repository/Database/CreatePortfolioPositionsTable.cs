using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202212042143)]
public class CreatePortfolioPositionsTable : Migration
{
    public override void Up()
    {
        Create.Table("PortfolioPositions")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("PortfolioId").AsInt32().ForeignKey("Portfolio", "Id")
            .WithColumn("Symbol").AsString()
            .WithColumn("Shares").AsInt32().WithDefaultValue(0)
            .WithColumn("AvgPrice").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Name").AsString()
            .WithColumn("LastPrice").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeToday").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeTodayPercent").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("PrevClose").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("CostValue").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("CurrentValue").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Return").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ReturnPercent").AsDecimal().WithDefaultValue(0.0)
            ;
    }

    public override void Down()
    {
        Delete.Table("PortfolioPositions");
    }
}
