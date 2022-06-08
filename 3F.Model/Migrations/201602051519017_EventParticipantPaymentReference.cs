namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EventParticipantPaymentReference : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventParticipant", "Id_Payment", c => c.Int());
            CreateIndex("dbo.EventParticipant", "Id_Payment");
            AddForeignKey("dbo.EventParticipant", "Id_Payment", "dbo.Payment", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventParticipant", "Id_Payment", "dbo.Payment");
            DropIndex("dbo.EventParticipant", new[] { "Id_Payment" });
            DropColumn("dbo.EventParticipant", "Id_Payment");
        }
    }
}
