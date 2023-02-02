using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202302012036)]
public class CreateInstrumentsTable : Migration
{
    public override void Up()
    {
        Create.Table("Instruments")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("Symbol").AsString()
            .WithColumn("Currency").AsString()
            .WithColumn("FinancialCurrency").AsString()
            .WithColumn("ShortName").AsString()
            .WithColumn("LongName").AsString()
            .WithColumn("Exchange").AsString()
            .WithColumn("ExchangeFullName").AsString()
            .WithColumn("InstrumentType").AsString()
            .WithColumn("AvgAnalystRating").AsString()
            ;
    }

    public override void Down()
    {
        Delete.Table("Instruments");
    }
}
