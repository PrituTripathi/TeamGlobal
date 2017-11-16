namespace TeamGlobal.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCommit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CODE = c.String(maxLength: 100),
                        NAME = c.String(maxLength: 100),
                        CountryCode = c.String(maxLength: 100),
                        CountryName = c.String(maxLength: 100),
                        EIDPortCode = c.String(maxLength: 100),
                        TransportMode = c.String(maxLength: 100),
                        LOVStatus = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Location");
        }
    }
}
