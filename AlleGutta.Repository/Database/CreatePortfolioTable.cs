using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202212042120)]
public class CreatePortfolioTable : Migration
{
    public override void Up()
    {
        Create.Table("Portfolio")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("Cash").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Ath").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Equity").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("CostValue").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("MarketValue").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("MarketValuePrev").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("MarketValueMax").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("MarketValueMin").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeTodayTotal").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeTodayPercent").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeTotal").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("ChangeTotalPercent").AsDecimal().WithDefaultValue(0.0)
            ;
    }

    public override void Down()
    {
        Delete.Table("Portfolio");
    }
}
