namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentGuid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payment", "guid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payment", "guid");
        }
    }
}
