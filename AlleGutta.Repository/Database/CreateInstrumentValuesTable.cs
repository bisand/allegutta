using FluentMigrator;

namespace AlleGutta.Repository.Database;

[Migration(202302020754)]
public class CreateInstrumentValuesTable : Migration
{
    public override void Up()
    {
        Create.Table("InstrumentValues")
            .WithColumn("Id").AsInt32().PrimaryKey().Identity()
            .WithColumn("InstrumentId").AsInt32().ForeignKey("Instruments", "Id")
            .WithColumn("Open").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("High").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Low").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Close").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Volume").AsInt32().WithDefaultValue(0)
            .WithColumn("AdjClose").AsDecimal().WithDefaultValue(0.0)
            .WithColumn("Date").AsDateTime()
            ;
    }

    public override void Down()
    {
        Delete.Table("Instrument");
    }
}
