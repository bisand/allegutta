using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202212042120)]
public class CreatePortfolioTable : Migration
{
    public override void Up()
    {
        Create.Table("Portfolio")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Name").AsString()
            .WithColumn("Cash").AsDecimal()
            .WithColumn("Ath").AsDecimal()
            .WithColumn("Equity").AsDecimal()
            .WithColumn("CostValue").AsDecimal()
            .WithColumn("MarketValue").AsDecimal()
            .WithColumn("MarketValuePrev").AsDecimal()
            .WithColumn("MarketValueMax").AsDecimal()
            .WithColumn("MarketValueMin").AsDecimal()
            .WithColumn("ChangeTodayTotal").AsDecimal()
            .WithColumn("ChangeTodayPercent").AsDecimal()
            .WithColumn("ChangeTotal").AsDecimal()
            .WithColumn("ChangeTotalPercent").AsDecimal()
            ;
    }

    public override void Down()
    {
        Delete.Table("Portfolio");
    }
}