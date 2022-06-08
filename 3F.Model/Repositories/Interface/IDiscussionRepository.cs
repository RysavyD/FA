using System;
using System.Collections.Generic;
using _3F.BusinessEntities.Diskuze;

namespace _3F.Model.Repositories.Interface
{
    public interface IDiscussionRepository
    {
        IEnumerable<BusinessEntities.Discussion> Discussions { get; }
        Tuple<int, IEnumerable<BusinessEntities.DiscussionItem>> GetDiscussionItems(int discussionId, int page = 1, int pagesize = 10);

        IEnumerable<LastDiscussion> GetLastDiscsuDiscussions();
    }
}
