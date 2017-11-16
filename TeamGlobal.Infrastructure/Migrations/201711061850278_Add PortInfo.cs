namespace TeamGlobal.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddPortInfo : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PortInfo",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PORT_A = c.String(),
                        PORT_B = c.String(),
                        PORT_C = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.PortInfo");
        }
    }
}
