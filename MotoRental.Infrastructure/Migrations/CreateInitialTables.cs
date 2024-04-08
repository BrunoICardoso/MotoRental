using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentMigrator;

namespace MotoRental.Infrastructure.Migrations
{

    [Migration(2024040601)]
    public class CreateInitialTables : Migration
    {
        public override void Up()
        {
            #region Auth
            Execute.Sql("CREATE SCHEMA IF NOT EXISTS auth");

            Create.Table("Role")
             .InSchema("auth")
             .WithColumn("IdRole").AsInt32().PrimaryKey().Identity()
             .WithColumn("Name").AsString();

            Insert.IntoTable("Role").InSchema("auth")
                  .Row(new { Name = "Admin" })
                  .Row(new { Name = "Entregador" });


            Create.Table("User")
                .InSchema("auth")
                .WithColumn("IdUser").AsInt32().PrimaryKey().Identity()
                .WithColumn("Username").AsString()
                .WithColumn("PasswordHash").AsString()
                .WithColumn("Active").AsBoolean();

            Create.Table("UserRole")
                  .InSchema("auth")
                  .WithColumn("IdUser").AsInt32()
                  .WithColumn("IdRole").AsInt32();
    

            Create.PrimaryKey()
                 .OnTable("UserRole").WithSchema("auth")
                 .Columns("IdUser", "IdRole");


            Create.ForeignKey()
                  .FromTable("UserRole").InSchema("auth").ForeignColumn("IdUser")
                  .ToTable("User").InSchema("auth").PrimaryColumn("IdUser");

            Create.ForeignKey()
                  .FromTable("UserRole").InSchema("auth").ForeignColumn("IdRole")
                  .ToTable("Role").InSchema("auth").PrimaryColumn("IdRole");

            #endregion

            #region Core

            Execute.Sql("CREATE SCHEMA IF NOT EXISTS core");

            Create.Table("CNHType")
                  .InSchema("core")
                  .WithColumn("IdCNHType").AsInt32().PrimaryKey().Identity()
                  .WithColumn("Name").AsString();

            Insert.IntoTable("CNHType").InSchema("core")
                                       .Row(new { Name = "A" })
                                       .Row(new { Name = "B" })
                                       .Row(new { Name = "AB" });


            Create.Table("Moto")
                .InSchema("core")
                .WithColumn("IdMoto").AsInt32().PrimaryKey().Identity()
                .WithColumn("Year").AsInt32()
                .WithColumn("Model").AsString()
                .WithColumn("LicensePlate").AsString(7).Unique()
                .WithColumn("Active").AsBoolean();


            Create.Table("DeliveryPerson")
                .InSchema("core")
                .WithColumn("IdDeliveryPerson").AsInt32().PrimaryKey().Identity()
                .WithColumn("IdUser").AsInt32().Unique()
                .WithColumn("Name").AsString()
                .WithColumn("CNPJ").AsString(14).Unique()
                .WithColumn("BirthDate").AsDate()
                .WithColumn("CNHNumber").AsString().Unique()
                .WithColumn("IdCNHType").AsInt32()
                .WithColumn("CNHImagePath").AsString().Nullable()
                .WithColumn("Active").AsBoolean();


            Create.Table("OrderStatus")
                .InSchema("core")
                .WithColumn("IdOrderStatus").AsInt32().PrimaryKey().Identity()
                .WithColumn("Status").AsString();

            Insert.IntoTable("OrderStatus").InSchema("core")
                                          .Row(new { Status = "Disponível" })
                                          .Row(new { Status = "Aceito" })
                                          .Row(new { Status = "Entregue" });

            Create.Table("Order")
                .InSchema("core")
                .WithColumn("IdOrder").AsInt32().PrimaryKey().Identity()
                .WithColumn("CreationDate").AsDateTime()
                .WithColumn("RaceValue").AsDecimal(18, 2)
                .WithColumn("IdOrderStatus").AsInt32()
                .WithColumn("IdDeliveryPerson").AsInt32().Nullable();


            Create.Table("RentalPlan")
                   .InSchema("core")
                   .WithColumn("IdRentalPlan").AsInt32().PrimaryKey().Identity()
                   .WithColumn("DurationDays").AsInt32()
                   .WithColumn("DailyCost").AsDecimal(18, 2)
                   .WithColumn("PenaltyRate").AsDecimal(18, 2)
                   .WithColumn("AdditionalDailyCost").AsDecimal(18, 2).WithDefaultValue(50.00);


            Insert.IntoTable("RentalPlan").InSchema("core")
                .Row(new { DurationDays = 7, DailyCost = 30.00, PenaltyRate = 20.00 })
                .Row(new { DurationDays = 15, DailyCost = 28.00, PenaltyRate = 40.00 })
                .Row(new { DurationDays = 30, DailyCost = 22.00, PenaltyRate = 60.00 });



            Create.Table("Rental")
                .InSchema("core")
                .WithColumn("IdRental").AsInt32().PrimaryKey().Identity()
                .WithColumn("IdDeliveryPerson").AsInt32()
                .WithColumn("IdMoto").AsInt32()
                .WithColumn("StartDate").AsDate()
                .WithColumn("EndDate").AsDate().Nullable()
                .WithColumn("ExpectedEndDate").AsDate()
                .WithColumn("DailyCost").AsDecimal(18, 2)
                .WithColumn("TotalCost").AsDecimal(18, 2).Nullable()
                .WithColumn("IdRentalPlan").AsInt32();

            Create.Table("Notification")
                 .InSchema("core")
                 .WithColumn("IdNotification").AsInt32().PrimaryKey().Identity()
                 .WithColumn("IdDeliveryPerson").AsInt32()
                 .WithColumn("IdOrder").AsInt32()
                 .WithColumn("NotifiedAt").AsDateTime();

            Create.ForeignKey()
                  .FromTable("Notification").InSchema("core").ForeignColumn("IdDeliveryPerson")
                  .ToTable("DeliveryPerson").InSchema("core").PrimaryColumn("IdDeliveryPerson");

            Create.ForeignKey()
                  .FromTable("Notification").InSchema("core").ForeignColumn("IdOrder")
                  .ToTable("Order").InSchema("core").PrimaryColumn("IdOrder");

            Create.ForeignKey()
                  .FromTable("DeliveryPerson").InSchema("core").ForeignColumn("IdUser")
                  .ToTable("User").InSchema("auth").PrimaryColumn("IdUser");


            Create.ForeignKey()
                  .FromTable("DeliveryPerson").InSchema("core").ForeignColumn("IdCNHType")
                  .ToTable("CNHType").InSchema("core").PrimaryColumn("IdCNHType");

            Create.ForeignKey()
                  .FromTable("Rental").InSchema("core").ForeignColumn("IdDeliveryPerson")
                  .ToTable("DeliveryPerson").InSchema("core").PrimaryColumn("IdDeliveryPerson");

            Create.ForeignKey()
                  .FromTable("Rental").InSchema("core").ForeignColumn("IdMoto")
                  .ToTable("Moto").InSchema("core").PrimaryColumn("IdMoto");

            Create.ForeignKey()
                  .FromTable("Rental").InSchema("core").ForeignColumn("IdRentalPlan")
                  .ToTable("RentalPlan").InSchema("core").PrimaryColumn("IdRentalPlan");

            Create.ForeignKey()
                  .FromTable("Order").InSchema("core").ForeignColumn("IdOrderStatus")
                  .ToTable("OrderStatus").InSchema("core").PrimaryColumn("IdOrderStatus");

            Create.ForeignKey()
                  .FromTable("Order").InSchema("core").ForeignColumn("IdDeliveryPerson")
                  .ToTable("DeliveryPerson").InSchema("core").PrimaryColumn("IdDeliveryPerson");


            #endregion


        }

        public override void Down()
        {
            
            Delete.Table("Rental").InSchema("core");
            Delete.Table("Order").InSchema("core");
            Delete.Table("DeliveryPerson").InSchema("core");

            
            Delete.Table("Moto").InSchema("core");
            Delete.Table("CNHType").InSchema("core");
            Delete.Table("OrderStatus").InSchema("core");
            Delete.Table("RentalPlan").InSchema("core");
            Delete.Table("Notification").InSchema("core");

            Delete.Table("UserRole").InSchema("auth");
            Delete.Table("User").InSchema("auth");
            Delete.Table("Role").InSchema("auth");

            
            Execute.Sql("DROP SCHEMA IF EXISTS auth CASCADE");
            Execute.Sql("DROP SCHEMA IF EXISTS core CASCADE");
        }
    }
}

