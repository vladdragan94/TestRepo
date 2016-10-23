using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieDictionary.Entities;
using MovieDictionary.IRepositories;

namespace MovieDictionary.BL
{
    public class PostsManager
    {
        private IPosts repository;

        public PostsManager()
        {
            this.repository = new DAL.PostsRepository() as IPosts;
        }

        public PostsManager(IPosts postsRepository)
        {
            this.repository = postsRepository;
        }

        public List<Post> GetQuestions(string userId = null, string orderType = null, int pageNumber = 0)
        {
            return repository.GetQuestions(userId, orderType, pageNumber);
        }

        public List<Post> GetPosts(string userId = null, string orderType = null, int pageNumber = 0)
        {
            return repository.GetPosts(userId, orderType, pageNumber);
        }

        public Post GetPostDetails(int postId, string userId = null)
        {
            var post = repository.GetPost(postId, userId);
            post.Replies = repository.GetReplies(postId, userId);

            return post;
        }

        public int? AddPost(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Title) || string.IsNullOrWhiteSpace(post.Content))
                return null;

            return repository.AddPost(post);
        }

        public void LikePost(int postId, string userId, bool liked)
        {
            repository.LikePost(postId, userId, liked);
        }

        public void UnlikePost(int postId, string userId)
        {
            repository.UnlikePost(postId, userId);
        }

        public bool MarkAsAnswer(int postId, string userId)
        {
            return repository.MarkAsAnswer(postId, userId);
        }

        public bool UnmarkAnswer(int postId, string userId)
        {
            return repository.UnmarkAnswer(postId, userId);
        }

        public bool DeletePost(int postId, string userId)
        {
            return repository.DeletePost(postId, userId);
        }

        public ForumStats GetForumStats()
        {
            return repository.GetForumStats();
        }
    }
}
