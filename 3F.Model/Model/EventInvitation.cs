namespace _3F.Model.Model
{
    public partial class EventInvitation : IPrimaryKey
    {
        public int Id { get; set; }

        public int Id_Event { get; set; }

        public int Id_User { get; set; }

        public Event Event { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
