using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202212042143)]
public class CreatePortfolioPositionsTable : Migration
{
    public override void Up()
    {
        Create.Table("PortfolioPositions")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("PortfolioId").AsInt64().ForeignKey("Portfolio", "Id")
            .WithColumn("Symbol").AsString()
            .WithColumn("Shares").AsInt64()
            .WithColumn("AvgPrice").AsDecimal()
            .WithColumn("Name").AsString()
            .WithColumn("LastPrice").AsDecimal()
            .WithColumn("ChangeToday").AsDecimal()
            .WithColumn("ChangeTodayPercent").AsDecimal()
            .WithColumn("PrevClose").AsDecimal()
            .WithColumn("CostValue").AsDecimal()
            .WithColumn("CurrentValue").AsDecimal()
            .WithColumn("Return").AsDecimal()
            .WithColumn("ReturnPercent").AsDecimal()
            ;
    }

    public override void Down()
    {
        Delete.Table("PortfolioPositions");
    }
}

[Migration(202212050708)]
public class ModifyPortfolioPositionsTable : Migration
{
    public override void Up()
    {
        Alter.Table("PortfolioPositions")
            .AlterColumn("Shares").AsInt64().WithDefaultValue(0)
            .AlterColumn("AvgPrice").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("LastPrice").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeToday").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ChangeTodayPercent").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("PrevClose").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("CostValue").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("CurrentValue").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("Return").AsDecimal().WithDefaultValue(0.0)
            .AlterColumn("ReturnPercent").AsDecimal().WithDefaultValue(0.0)
            ;
    }

    public override void Down()
    {
        Delete.Table("PortfolioPositions");
    }
}