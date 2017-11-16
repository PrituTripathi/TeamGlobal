namespace TeamGlobal.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatePortListValidation : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PortList", "Preference", c => c.String(maxLength: 25));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PortList", "Preference", c => c.Int(nullable: false));
        }
    }
}
