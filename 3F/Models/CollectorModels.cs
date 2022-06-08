using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using _3F.Model.Model;

namespace _3F.Web.Models.Collector
{
    public class CollectorIndexViewModel : BaseViewModel
    {
        public int Count { get; set; }
        public int DifferentCount { get; set; }
        public string MostItemName { get; set; }
        public int MostItemId { get; set; }
        public int MostItemIdCount { get; set; }

        public int MostItemInRow { get; set; }
        public User MostItemInRowUser { get; set; }

        public User Top1User { get; set; }
        public int Top1UserCount { get; set; }
        public User Top2User { get; set; }
        public int Top2UserCount { get; set; }
        public User Top3User { get; set; }
        public int Top3UserCount { get; set; }

        public string Top1HaveItemName { get; set; }
        public int Top1HaveItemId { get; set; }
        public int Top1HaveItemCount { get; set; }
        public string Top2HaveItemName { get; set; }
        public int Top2HaveItemId { get; set; }
        public int Top2HaveItemCount { get; set; }
        public string Top3HaveItemName { get; set; }
        public int Top3HaveItemId { get; set; }
        public int Top3HaveItemCount { get; set; }

        public string Top1WantItemName { get; set; }
        public int Top1WantItemId { get; set; }
        public int Top1WantItemCount { get; set; }
        public string Top2WantItemName { get; set; }
        public int Top2WantItemId { get; set; }
        public int Top2WantItemCount { get; set; }
        public string Top3WantItemName { get; set; }
        public int Top3WantItemId { get; set; }
        public int Top3WantItemCount { get; set; }
    }

    public class CollectorDetailViewModel : BaseViewModel
    {
        public int ItemId { get; set; }

        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public string ImageUrl { get; set; }

        public string BaseUrl { get; set; }

        public IEnumerable<CollertorItem> NearestItems { get; set; }

        public IEnumerable<User> HaveUser { get; set; }

        public IEnumerable<User> VisitUser { get; set; }

        public IEnumerable<User> WantUser { get; set; }
    }

    public class CollectorItemsOverview : EnumerableBaseViewModel<CollertorItem>
    {
        public string BaseUrl { get; set; }
    }

    public class CollertorItem
    {
        public int Id_Item { get; set; }
        public int ItemNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Distance { get; set; }
        public string Color { get; set; }
    }

    public class ItemWithOwner
    {
        public int ItemNumber { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
    }

    public class OwnerItems : EnumerableBaseViewModel<ItemWithOwner>
    {
        public string BaseUrl { get; set; }
    }

    public class OwnerRecord
    {
        public User User { get; set; }
        public int HaveCount { get; set; }
        public int VisitedCount { get; set; }
        public int WantCount { get; set; }
    }

    public class OwnerRecords : EnumerableBaseViewModel<OwnerRecord>
    {
        public string BaseUrl { get; set; }
    }
}