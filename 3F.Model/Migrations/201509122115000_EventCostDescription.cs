namespace _3F.Model.Migrations
{
    using System.Data.Entity.Migrations;

    public partial class _201509122115000_EventCostDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Event", "CostsDescription", c => c.String(nullable: true));
        }

        public override void Down()
        {
            DropColumn("dbo.Event", "CostsDescription");
        }
    }
}
