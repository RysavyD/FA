namespace _3F.Model.Model
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Entities : DbContext
    {
        public Entities()
            : base("name=DefaultConnection")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<Entities, Migrations.Configuration>("DefaultConnection"));

            //Database.Log = s => System.IO.File.AppendAllText(@"d:\dbLog.txt", s);
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Discussion> Discussion { get; set; }
        public virtual DbSet<DiscussionItem> DiscussionItem { get; set; }
        public virtual DbSet<Event> Event { get; set; }
        public virtual DbSet<EventInvitation> EventInvitation { get; set; }
        public virtual DbSet<EventOrganisator> EventOrganisator { get; set; }
        public virtual DbSet<EventParticipant> EventParticipant { get; set; }
        public virtual DbSet<EventParticipantHistory> EventParticipantHistory { get; set; }
        public virtual DbSet<EventSummary> EventSummary { get; set; }
        public virtual DbSet<Message> Message { get; set; }
        public virtual DbSet<MessageRecipient> MessageRecipient { get; set; }
        public virtual DbSet<OldPassword> OldPassword { get; set; }
        public virtual DbSet<Payment> Payment { get; set; }
        public virtual DbSet<PhotoAlbum> PhotoAlbum { get; set; }
        public virtual DbSet<Profiles> Profiles { get; set; }
        public virtual DbSet<TouristCard> TouristCard { get; set; }
        public virtual DbSet<TouristCardOwner> TouristCardOwner { get; set; }
        public virtual DbSet<TouristStamp> TouristStamp { get; set; }
        public virtual DbSet<TouristStampOwner> TouristStampOwner { get; set; }
        public virtual DbSet<PeriodicEvent> PeriodicEvent { get; set; }
        public virtual DbSet<ProgramSetting> ProgramSetting { get; set; }
        public virtual DbSet<EventCategory> EventCategory { get; set; }
        public virtual DbSet<Help> Help { get; set; }
        public virtual DbSet<Post> Post { get; set; }
        public virtual DbSet<FileUploadInfo> FileUploadInfo { get; set; }
        public virtual DbSet<KnowFrom> KnowFrom { get; set; }
        public virtual DbSet<AspNetUsersMainCategory> AspNetUsersMainCategory { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoles>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Discussion)
                .WithOptional(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Author);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.DiscussionItem)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Author)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.EventOrganisator)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.EventParticipant)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.EventParticipantHistory)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.EventSummary)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Message)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Sender)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.MessageRecipient)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.OldPassword)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.OrganisationMember)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Payment)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.PhotoAlbum)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.TouristCardOwner)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Owner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.TouristStampOwner)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_Owner)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.EventInvitation)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.MainCategories)
                .WithRequired(e => e.AspNetUsers)
                .HasForeignKey(e => e.Id_User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discussion>()
                .HasMany(e => e.DiscussionItem)
                .WithRequired(e => e.Discussion)
                .HasForeignKey(e => e.Id_Discussion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discussion>()
                .HasMany(e => e.Event)
                .WithRequired(e => e.Discussion)
                .HasForeignKey(e => e.Id_Discussion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discussion>()
                .HasMany(e => e.EventSummary)
                .WithRequired(e => e.Discussion)
                .HasForeignKey(e => e.Id_Discussion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Discussion>()
                .HasMany(e => e.PhotoAlbum)
                .WithRequired(e => e.Discussion)
                .HasForeignKey(e => e.Id_Discussion)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventOrganisator)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventParticipant)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventParticipantHistory)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventSummary)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.Payment)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.PhotoAlbum)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventInvitation)
                .WithRequired(e => e.Event)
                .HasForeignKey(e => e.Id_Event)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Event>()
                .HasMany(e => e.EventCategories)
                .WithMany(e => e.Events)
                .Map(e => e.ToTable("EventCategories")
                    .MapLeftKey("Id_Event")
                    .MapRightKey("Id_EventCategory"));

            modelBuilder.Entity<Message>()
                .HasMany(e => e.MessageRecipient)
                .WithRequired(e => e.Message)
                .HasForeignKey(e => e.Id_Message)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OldPassword>()
                .Property(e => e.PasswordHash)
                .IsFixedLength();

            modelBuilder.Entity<OldPassword>()
                .Property(e => e.PasswordSalt)
                .IsFixedLength();

            modelBuilder.Entity<Organisation>()
                .HasMany(e => e.OrganisationUser)
                .WithRequired(e => e.Organisation)
                .HasForeignKey(e => e.Id_Organisation)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Profiles>()
                .HasMany(e => e.AspNetUsers)
                .WithOptional(e => e.Profiles)
                .HasForeignKey(e => e.Profile_Id);

            modelBuilder.Entity<TouristCard>()
                .HasMany(e => e.TouristCardOwner)
                .WithRequired(e => e.TouristCard)
                .HasForeignKey(e => e.Id_Item)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<TouristStamp>()
                .HasMany(e => e.TouristStampOwner)
                .WithRequired(e => e.TouristStamp)
                .HasForeignKey(e => e.Id_Item)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Payment>()
                .HasMany(e => e.EventParticipant)
                .WithOptional(e => e.Payment)
                .HasForeignKey(e => e.Id_Payment)
                .WillCascadeOnDelete(false);
        }
    }
}
