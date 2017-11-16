namespace TeamGlobal.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePortList : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortList",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Preference = c.Int(nullable: false),
                        OriginCode = c.String(),
                        DestinationCode = c.String(),
                        RoutingCode = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PortList");
        }
    }
}
