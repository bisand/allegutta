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