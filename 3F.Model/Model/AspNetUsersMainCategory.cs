namespace _3F.Model.Model
{
    using _3F.BusinessEntities.Enum;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AspNetUsersMainCategory
    {
        [Key, Column(Order = 0)]
        public int Id_User { get; set; }

        [Key, Column(Order = 1)]
        public virtual MainCategory MainCategory { get; set; }

        public virtual AspNetUsers AspNetUsers { get; set; }
    }
}
