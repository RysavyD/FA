using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3F.Model.Model
{
    public partial class Payment
    {
        public Payment()
        {
            guid = Guid.NewGuid();
            EventParticipant = new HashSet<EventParticipant>();
        }
    }
}
