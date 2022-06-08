using System.Linq;

namespace _3F.Model.Model
{
    public partial class Discussion
    {
        public bool IsPublic
        {
            get
            {
                var eventEntity = this.Event.FirstOrDefault();
                if (eventEntity == null) // diskuze nebo zapis k akci, fotoalbum
                    return true;

                if (eventEntity.State == EventStateEnum.Active && eventEntity.EventType != EventTypeEnum.Soukroma)
                    return true;

                return false;
            }
        }
    }
}
