using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieDictionary.IRepositories
{
    public interface IPosts
    {
        List<Entities.Post> GetQuestions(string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultPageSize);

        List<Entities.Post> GetPosts(string userId = null, string orderType = null, int pageNumber = 0, int pageSize = Entities.Constants.ApplicationConfigurations.DefaultPageSize);

        Entities.Post GetPost(int postId, string userId = null);

        List<Entities.Post> GetReplies(int postId, string userId = null);

        int AddPost(Entities.Post forumPost);

        void LikePost(int postId, string userId, bool liked);

        void UnlikePost(int postId, string userId);

        bool MarkAsAnswer(int postId, string userId);

        bool UnmarkAnswer(int postId, string userId);

        bool DeletePost(int postId, string userId);

        Entities.ForumStats GetForumStats();
    }
}
