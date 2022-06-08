namespace _3F.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        LoginType = c.Int(nullable: false),
                        HtmlName = c.String(),
                        DateCreated = c.DateTime(nullable: false),
                        DateLastActivity = c.DateTime(nullable: false),
                        ProfilePhoto = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                        VariableSymbol = c.Int(),
                        Profile_Id = c.Int(),
                        RegisterType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Profiles", t => t.Profile_Id);
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);
            
            CreateTable(
                "dbo.Discussion",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        IsAlone = c.Boolean(nullable: false),
                        HtmlName = c.String(nullable: false),
                        Perex = c.String(),
                        Id_Author = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_Author);
            
            CreateTable(
                "dbo.DiscussionItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(nullable: false),
                        DateTime = c.DateTime(nullable: false),
                        Id_Discussion = c.Int(nullable: false),
                        Id_Author = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discussion", t => t.Id_Discussion)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_Author);
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 150),
                        Perex = c.String(nullable: false, maxLength: 150),
                        Description = c.String(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Place = c.String(maxLength: 150),
                        StartDateTime = c.DateTime(nullable: false),
                        StopDateTime = c.DateTime(nullable: false),
                        LastSignINDateTime = c.DateTime(),
                        MeetDateTime = c.DateTime(nullable: false),
                        MeetPlace = c.String(nullable: false, maxLength: 150),
                        Contact = c.String(maxLength: 150),
                        Price = c.Int(nullable: false),
                        HtmlName = c.String(nullable: false, maxLength: 200),
                        Id_Discussion = c.Int(nullable: false),
                        Link = c.String(),
                        BankAccount = c.String(maxLength: 50),
                        MayBeLogOn = c.Boolean(nullable: false),
                        LastPaidDateTime = c.DateTime(),
                        ExternParticipants = c.Int(nullable: false),
                        MinimumParticipants = c.Int(nullable: false),
                        Photo = c.String(),
                        AccountSymbol = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        Costs = c.Int(nullable: false),
                        EventType = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Discussion", t => t.Id_Discussion);
            
            CreateTable(
                "dbo.EventOrganisator",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Event = c.Int(nullable: false),
                        Id_User = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.EventParticipant",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Id_Event = c.Int(nullable: false),
                        EventLoginStatus = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        IsExternal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.EventParticipantHistory",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Id_Event = c.Int(nullable: false),
                        OldEventLoginStatus = c.Int(nullable: false),
                        NewEventLoginStatus = c.Int(nullable: false),
                        Time = c.DateTime(nullable: false),
                        IsExternal = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.EventSummary",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Perex = c.String(nullable: false, maxLength: 300),
                        Description = c.String(nullable: false),
                        Id_Event = c.Int(nullable: false),
                        Id_Discussion = c.Int(nullable: false),
                        Id_User = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.Discussion", t => t.Id_Discussion)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User, cascadeDelete: true);
            
            CreateTable(
                "dbo.Payment",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Id_Event = c.Int(nullable: false),
                        Note = c.String(),
                        Amount = c.Double(nullable: false),
                        CreateDate = c.DateTime(),
                        Status = c.Int(nullable: false),
                        UpdateDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.PhotoAlbum",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Id_Event = c.Int(nullable: false),
                        AlbumLink = c.String(nullable: false, maxLength: 200),
                        CoverPhotoLink = c.String(maxLength: 200),
                        PhotoCount = c.Int(nullable: false),
                        Id_Discussion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Event", t => t.Id_Event)
                .ForeignKey("dbo.Discussion", t => t.Id_Discussion)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.Message",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Sender = c.Int(nullable: false),
                        Subject = c.String(nullable: false, maxLength: 50),
                        Text = c.String(nullable: false),
                        Time = c.DateTime(nullable: false),
                        Visible = c.Boolean(nullable: false),
                        Id_ReplyMessage = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_Sender);
            
            CreateTable(
                "dbo.MessageRecipient",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Message = c.Int(nullable: false),
                        Id_User = c.Int(nullable: false),
                        Unreaded = c.Boolean(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Message", t => t.Id_Message)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.OldPassword",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PasswordHash = c.Binary(nullable: false, maxLength: 64, fixedLength: true),
                        PasswordSalt = c.Binary(nullable: false, maxLength: 128, fixedLength: true),
                        Id_User = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.OrganisationMember",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_User = c.Int(nullable: false),
                        Id_Organisation = c.Int(nullable: false),
                        From = c.DateTime(nullable: false),
                        To = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Organisation", t => t.Id_Organisation)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_User);
            
            CreateTable(
                "dbo.Organisation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 500),
                        HtmlName = c.String(),
                        ICO = c.Int(nullable: false),
                        BankAccount = c.String(),
                        Address = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Profiles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        City = c.String(),
                        BirhtYear = c.Int(),
                        Motto = c.String(),
                        Hobbies = c.String(),
                        Status = c.Int(nullable: false),
                        Link = c.String(maxLength: 200),
                        SendNewActionToMail = c.Boolean(nullable: false),
                        SendMessagesToMail = c.Boolean(nullable: false),
                        SendMessagesFromAdminToMail = c.Boolean(nullable: false),
                        SendMayBeEventNotice = c.Boolean(nullable: false),
                        Sex = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TouristCardOwner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Owner = c.Int(nullable: false),
                        Id_Card = c.Int(nullable: false),
                        Id_Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TouristCard", t => t.Id_Card)
                .ForeignKey("dbo.TouristStampStatus", t => t.Id_Status)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_Owner);
            
            CreateTable(
                "dbo.TouristCard",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CardId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Position = c.Geography(),
                        ImageUrl = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TouristStampStatus",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Status = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TouristStampOwner",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Id_Owner = c.Int(nullable: false),
                        Id_Stamp = c.Int(nullable: false),
                        Id_Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TouristStamp", t => t.Id_Stamp)
                .ForeignKey("dbo.TouristStampStatus", t => t.Id_Status)
                .ForeignKey("dbo.AspNetUsers", t => t.Id_Owner);
            
            CreateTable(
                "dbo.TouristStamp",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        StampId = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(),
                        Position = c.Geography(),
                        ImageUrl = c.String(maxLength: 150),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.__MigrationHistory",
                c => new
                    {
                        MigrationId = c.String(nullable: false, maxLength: 150),
                        ContextKey = c.String(nullable: false, maxLength: 300),
                        Model = c.Binary(nullable: false),
                        ProductVersion = c.String(nullable: false, maxLength: 32),
                    })
                .PrimaryKey(t => new { t.MigrationId, t.ContextKey });
            
            CreateTable(
                "dbo.PeriodicEvents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PeriodicEventType = c.Int(nullable: false),
                        EventNameFormat = c.String(nullable: false),
                        PeriodicParameter = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.RobotTask",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TaskType = c.String(nullable: false, maxLength: 2),
                        ID_TaskType = c.String(nullable: false, maxLength: 40),
                        Finished = c.Boolean(nullable: false),
                        CreateDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Text",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Key = c.String(nullable: false, maxLength: 50),
                        Value = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.VersionDB",
                c => new
                    {
                        Number = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.Number, t.Date });
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        RoleId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.UserId })
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.TouristStampOwner", "Id_Owner", "dbo.AspNetUsers");
            DropForeignKey("dbo.TouristCardOwner", "Id_Owner", "dbo.AspNetUsers");
            DropForeignKey("dbo.TouristStampOwner", "Id_Status", "dbo.TouristStampStatus");
            DropForeignKey("dbo.TouristStampOwner", "Id_Stamp", "dbo.TouristStamp");
            DropForeignKey("dbo.TouristCardOwner", "Id_Status", "dbo.TouristStampStatus");
            DropForeignKey("dbo.TouristCardOwner", "Id_Card", "dbo.TouristCard");
            DropForeignKey("dbo.AspNetUsers", "Profile_Id", "dbo.Profiles");
            DropForeignKey("dbo.PhotoAlbum", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.Payment", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationMember", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.OrganisationMember", "Id_Organisation", "dbo.Organisation");
            DropForeignKey("dbo.OldPassword", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageRecipient", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.Message", "Id_Sender", "dbo.AspNetUsers");
            DropForeignKey("dbo.MessageRecipient", "Id_Message", "dbo.Message");
            DropForeignKey("dbo.EventSummary", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventParticipantHistory", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventParticipant", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.EventOrganisator", "Id_User", "dbo.AspNetUsers");
            DropForeignKey("dbo.DiscussionItem", "Id_Author", "dbo.AspNetUsers");
            DropForeignKey("dbo.Discussion", "Id_Author", "dbo.AspNetUsers");
            DropForeignKey("dbo.PhotoAlbum", "Id_Discussion", "dbo.Discussion");
            DropForeignKey("dbo.EventSummary", "Id_Discussion", "dbo.Discussion");
            DropForeignKey("dbo.Event", "Id_Discussion", "dbo.Discussion");
            DropForeignKey("dbo.PhotoAlbum", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.Payment", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.EventSummary", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.EventParticipantHistory", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.EventParticipant", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.EventOrganisator", "Id_Event", "dbo.Event");
            DropForeignKey("dbo.DiscussionItem", "Id_Discussion", "dbo.Discussion");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.VersionDB");
            DropTable("dbo.Text");
            DropTable("dbo.RobotTask");
            DropTable("dbo.PeriodicEvents");
            DropTable("dbo.__MigrationHistory");
            DropTable("dbo.TouristStamp");
            DropTable("dbo.TouristStampOwner");
            DropTable("dbo.TouristStampStatus");
            DropTable("dbo.TouristCard");
            DropTable("dbo.TouristCardOwner");
            DropTable("dbo.Profiles");
            DropTable("dbo.Organisation");
            DropTable("dbo.OrganisationMember");
            DropTable("dbo.OldPassword");
            DropTable("dbo.MessageRecipient");
            DropTable("dbo.Message");
            DropTable("dbo.PhotoAlbum");
            DropTable("dbo.Payment");
            DropTable("dbo.EventSummary");
            DropTable("dbo.EventParticipantHistory");
            DropTable("dbo.EventParticipant");
            DropTable("dbo.EventOrganisator");
            DropTable("dbo.Event");
            DropTable("dbo.DiscussionItem");
            DropTable("dbo.Discussion");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.AspNetRoles");
        }
    }
}
