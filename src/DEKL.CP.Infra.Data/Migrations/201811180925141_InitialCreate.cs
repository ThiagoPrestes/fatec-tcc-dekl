namespace DEKL.CP.Infra.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountToPay",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Decimal(nullable: false, storeType: "money"),
                        PaidValue = c.Decimal(storeType: "money"),
                        PaymentDate = c.DateTime(),
                        Description = c.String(maxLength: 60, unicode: false),
                        MaturityDate = c.DateTime(nullable: false),
                        DailyInterest = c.Decimal(nullable: false, precision: 4, scale: 2),
                        Penalty = c.Decimal(nullable: false, precision: 4, scale: 2),
                        MonthlyAccount = c.Boolean(nullable: false),
                        Priority = c.Int(nullable: false),
                        PaymentType = c.Int(nullable: false),
                        DocumentNumber = c.String(nullable: false, maxLength: 80, unicode: false),
                        NumberOfInstallments = c.Short(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Provider", t => t.ProviderId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.Installment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.Decimal(nullable: false, storeType: "money"),
                        PaidValue = c.Decimal(storeType: "money"),
                        MaturityDate = c.DateTime(nullable: false),
                        PaymentDate = c.DateTime(),
                        AccountToPayId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountToPay", t => t.AccountToPayId)
                .Index(t => t.AccountToPayId);
            
            CreateTable(
                "dbo.PaymentSimulator",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PaymentDate = c.DateTime(nullable: false),
                        InternalBankAccountId = c.Int(nullable: false),
                        Observations = c.String(maxLength: 100, unicode: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.InternalBankAccount", t => t.InternalBankAccountId, cascadeDelete: true)
                .Index(t => t.InternalBankAccountId);
            
            CreateTable(
                "dbo.InternalBankAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 20, unicode: false),
                        Name = c.String(nullable: false, maxLength: 80, unicode: false),
                        Balance = c.Decimal(nullable: false, storeType: "money"),
                        BankAgencyId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAgency", t => t.BankAgencyId)
                .Index(t => t.Name, unique: true, name: "UQ_dbo.InternalBankAccount.Number")
                .Index(t => t.BankAgencyId);
            
            CreateTable(
                "dbo.BankAgency",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Short(nullable: false),
                        AddressId = c.Int(nullable: false),
                        ManagerName = c.String(maxLength: 80, unicode: false),
                        PhoneNumber = c.String(maxLength: 20, unicode: false),
                        Email = c.String(maxLength: 80, unicode: false),
                        BankId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.Id)
                .ForeignKey("dbo.Bank", t => t.BankId)
                .Index(t => t.Id)
                .Index(t => t.Number, unique: true, name: "UQ_dbo.BankAgency.Number")
                .Index(t => t.Email, unique: true, name: "UQ_dbo.BankAgency.Email")
                .Index(t => t.BankId);
            
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Street = c.String(nullable: false, maxLength: 60, unicode: false),
                        Number = c.String(maxLength: 20, unicode: false),
                        ZipCode = c.String(nullable: false, maxLength: 100, unicode: false),
                        Complement = c.String(nullable: false, maxLength: 100, unicode: false),
                        Neighborhood = c.String(nullable: false, maxLength: 60, unicode: false),
                        City = c.String(nullable: false, maxLength: 60, unicode: false),
                        StateId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.State", t => t.StateId)
                .Index(t => t.StateId);
            
            CreateTable(
                "dbo.State",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60, unicode: false),
                        Initials = c.String(nullable: false, maxLength: 2, fixedLength: true, unicode: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Bank",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.Short(nullable: false),
                        Name = c.String(nullable: false, maxLength: 60, unicode: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Number, unique: true, name: "UQ_dbo.Bank.Number")
                .Index(t => t.Name, unique: true, name: "UQ_dbo.Bank.Name");
            
            CreateTable(
                "dbo.ProviderBankAccount",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false, maxLength: 20, unicode: false),
                        Name = c.String(nullable: false, maxLength: 80, unicode: false),
                        BankAgencyId = c.Int(nullable: false),
                        ProviderId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.BankAgency", t => t.BankAgencyId, cascadeDelete: true)
                .ForeignKey("dbo.Provider", t => t.ProviderId)
                .Index(t => t.Name, unique: true, name: "UQ_dbo.ProviderBankAccount.Name")
                .Index(t => t.BankAgencyId)
                .Index(t => t.ProviderId);
            
            CreateTable(
                "dbo.BankTransaction",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AccountToPayId = c.Int(nullable: false),
                        InternalBankAccountId = c.Int(nullable: false),
                        ProviderBankAccountId = c.Int(nullable: false),
                        NewBalance = c.Decimal(nullable: false, precision: 4, scale: 2),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AccountToPay", t => t.AccountToPayId, cascadeDelete: true)
                .ForeignKey("dbo.InternalBankAccount", t => t.InternalBankAccountId)
                .ForeignKey("dbo.ProviderBankAccount", t => t.ProviderBankAccountId)
                .Index(t => t.AccountToPayId)
                .Index(t => t.InternalBankAccountId)
                .Index(t => t.ProviderBankAccountId);
            
            CreateTable(
                "dbo.Provider",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PhoneNumber = c.String(maxLength: 20, unicode: false),
                        Email = c.String(maxLength: 80, unicode: false),
                        AddressId = c.Int(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Address", t => t.AddressId, cascadeDelete: true)
                .Index(t => t.Email, unique: true, name: "UQ_dbo.ProviderPhysicalPerson.Email")
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Audit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ApplicationUserId = c.Int(nullable: false),
                        ModuleId = c.Int(nullable: false),
                        Event = c.String(nullable: false, maxLength: 100, unicode: false),
                        DateTime = c.DateTime(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ApplicationUser", t => t.ApplicationUserId)
                .ForeignKey("dbo.Module", t => t.ModuleId)
                .Index(t => t.ApplicationUserId)
                .Index(t => t.ModuleId);
            
            CreateTable(
                "dbo.ApplicationUser",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                        FirstName = c.String(maxLength: 100, unicode: false),
                        LastName = c.String(maxLength: 100, unicode: false),
                        Email = c.String(maxLength: 100, unicode: false),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(maxLength: 100, unicode: false),
                        SecurityStamp = c.String(maxLength: 100, unicode: false),
                        PhoneNumber = c.String(maxLength: 100, unicode: false),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Module",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 60, unicode: false),
                        Description = c.String(nullable: false, maxLength: 80, unicode: false),
                        AddedDate = c.DateTime(nullable: false),
                        ModifiedDate = c.DateTime(),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "UQ_dbo.Module.Name");
            
            CreateTable(
                "dbo.PaymentSimulatorAccountToPay",
                c => new
                    {
                        PaymentSimulator_Id = c.Int(nullable: false),
                        AccountToPay_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.PaymentSimulator_Id, t.AccountToPay_Id })
                .ForeignKey("dbo.PaymentSimulator", t => t.PaymentSimulator_Id, cascadeDelete: true)
                .ForeignKey("dbo.AccountToPay", t => t.AccountToPay_Id, cascadeDelete: true)
                .Index(t => t.PaymentSimulator_Id)
                .Index(t => t.AccountToPay_Id);
            
            CreateTable(
                "dbo.ProviderLegalPerson",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        CorporateName = c.String(nullable: false, maxLength: 100, unicode: false),
                        CNPJ = c.String(nullable: false, maxLength: 14, unicode: false),
                        MunicipalRegistration = c.String(maxLength: 11, unicode: false),
                        StateRegistration = c.String(maxLength: 9, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Provider", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.CNPJ, unique: true, name: "UQ_dbo.ProviderLegalPerson.CNPJ");
            
            CreateTable(
                "dbo.ProviderPhysicalPersonMap",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100, unicode: false),
                        CPF = c.String(nullable: false, maxLength: 11, unicode: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Provider", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.CPF, unique: true, name: "UQ_dbo.ProviderPhysicalPerson.CPF");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ProviderPhysicalPersonMap", "Id", "dbo.Provider");
            DropForeignKey("dbo.ProviderLegalPerson", "Id", "dbo.Provider");
            DropForeignKey("dbo.Audit", "ModuleId", "dbo.Module");
            DropForeignKey("dbo.Audit", "ApplicationUserId", "dbo.ApplicationUser");
            DropForeignKey("dbo.AccountToPay", "ProviderId", "dbo.Provider");
            DropForeignKey("dbo.PaymentSimulator", "InternalBankAccountId", "dbo.InternalBankAccount");
            DropForeignKey("dbo.InternalBankAccount", "BankAgencyId", "dbo.BankAgency");
            DropForeignKey("dbo.ProviderBankAccount", "ProviderId", "dbo.Provider");
            DropForeignKey("dbo.Provider", "AddressId", "dbo.Address");
            DropForeignKey("dbo.BankTransaction", "ProviderBankAccountId", "dbo.ProviderBankAccount");
            DropForeignKey("dbo.BankTransaction", "InternalBankAccountId", "dbo.InternalBankAccount");
            DropForeignKey("dbo.BankTransaction", "AccountToPayId", "dbo.AccountToPay");
            DropForeignKey("dbo.ProviderBankAccount", "BankAgencyId", "dbo.BankAgency");
            DropForeignKey("dbo.BankAgency", "BankId", "dbo.Bank");
            DropForeignKey("dbo.BankAgency", "Id", "dbo.Address");
            DropForeignKey("dbo.Address", "StateId", "dbo.State");
            DropForeignKey("dbo.PaymentSimulatorAccountToPay", "AccountToPay_Id", "dbo.AccountToPay");
            DropForeignKey("dbo.PaymentSimulatorAccountToPay", "PaymentSimulator_Id", "dbo.PaymentSimulator");
            DropForeignKey("dbo.Installment", "AccountToPayId", "dbo.AccountToPay");
            DropIndex("dbo.ProviderPhysicalPersonMap", "UQ_dbo.ProviderPhysicalPerson.CPF");
            DropIndex("dbo.ProviderPhysicalPersonMap", new[] { "Id" });
            DropIndex("dbo.ProviderLegalPerson", "UQ_dbo.ProviderLegalPerson.CNPJ");
            DropIndex("dbo.ProviderLegalPerson", new[] { "Id" });
            DropIndex("dbo.PaymentSimulatorAccountToPay", new[] { "AccountToPay_Id" });
            DropIndex("dbo.PaymentSimulatorAccountToPay", new[] { "PaymentSimulator_Id" });
            DropIndex("dbo.Module", "UQ_dbo.Module.Name");
            DropIndex("dbo.Audit", new[] { "ModuleId" });
            DropIndex("dbo.Audit", new[] { "ApplicationUserId" });
            DropIndex("dbo.Provider", new[] { "AddressId" });
            DropIndex("dbo.Provider", "UQ_dbo.ProviderPhysicalPerson.Email");
            DropIndex("dbo.BankTransaction", new[] { "ProviderBankAccountId" });
            DropIndex("dbo.BankTransaction", new[] { "InternalBankAccountId" });
            DropIndex("dbo.BankTransaction", new[] { "AccountToPayId" });
            DropIndex("dbo.ProviderBankAccount", new[] { "ProviderId" });
            DropIndex("dbo.ProviderBankAccount", new[] { "BankAgencyId" });
            DropIndex("dbo.ProviderBankAccount", "UQ_dbo.ProviderBankAccount.Name");
            DropIndex("dbo.Bank", "UQ_dbo.Bank.Name");
            DropIndex("dbo.Bank", "UQ_dbo.Bank.Number");
            DropIndex("dbo.Address", new[] { "StateId" });
            DropIndex("dbo.BankAgency", new[] { "BankId" });
            DropIndex("dbo.BankAgency", "UQ_dbo.BankAgency.Email");
            DropIndex("dbo.BankAgency", "UQ_dbo.BankAgency.Number");
            DropIndex("dbo.BankAgency", new[] { "Id" });
            DropIndex("dbo.InternalBankAccount", new[] { "BankAgencyId" });
            DropIndex("dbo.InternalBankAccount", "UQ_dbo.InternalBankAccount.Number");
            DropIndex("dbo.PaymentSimulator", new[] { "InternalBankAccountId" });
            DropIndex("dbo.Installment", new[] { "AccountToPayId" });
            DropIndex("dbo.AccountToPay", new[] { "ProviderId" });
            DropTable("dbo.ProviderPhysicalPersonMap");
            DropTable("dbo.ProviderLegalPerson");
            DropTable("dbo.PaymentSimulatorAccountToPay");
            DropTable("dbo.Module");
            DropTable("dbo.ApplicationUser");
            DropTable("dbo.Audit");
            DropTable("dbo.Provider");
            DropTable("dbo.BankTransaction");
            DropTable("dbo.ProviderBankAccount");
            DropTable("dbo.Bank");
            DropTable("dbo.State");
            DropTable("dbo.Address");
            DropTable("dbo.BankAgency");
            DropTable("dbo.InternalBankAccount");
            DropTable("dbo.PaymentSimulator");
            DropTable("dbo.Installment");
            DropTable("dbo.AccountToPay");
        }
    }
}
