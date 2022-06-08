namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ParticipantIdInHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventParticipantHistory", "Id_Participant", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventParticipantHistory", "Id_Participant");
        }
    }
}
