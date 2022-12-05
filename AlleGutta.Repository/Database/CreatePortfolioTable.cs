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

[Migration(202212050700)]
public class ModifyPortfolioTable : Migration
{
    public override void Up()
    {
        Alter.Table("Portfolio")
            .AlterColumn("Cash").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("Ath").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("Equity").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("CostValue").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("MarketValue").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("MarketValuePrev").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("MarketValueMax").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("MarketValueMin").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeTodayTotal").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeTodayPercent").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeTotal").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeTotalPercent").AsDecimal().WithDefaultValue(0.0)
            ;
    }

    public override void Down()
    {
        Alter.Table("Portfolio")
            .AlterColumn("Cash").AsDecimal()
            .AlterColumn("Ath").AsDecimal()
            .AlterColumn("Equity").AsDecimal()
            .AlterColumn("CostValue").AsDecimal()
            .AlterColumn("MarketValue").AsDecimal()
            .AlterColumn("MarketValuePrev").AsDecimal()
            .AlterColumn("MarketValueMax").AsDecimal()
            .AlterColumn("MarketValueMin").AsDecimal()
            .AlterColumn("ChangeTodayTotal").AsDecimal()
            .AlterColumn("ChangeTodayPercent").AsDecimal()
            .AlterColumn("ChangeTotal").AsDecimal()
            .AlterColumn("ChangeTotalPercent").AsDecimal()
            ;
    }
}
