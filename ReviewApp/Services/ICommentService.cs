using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface ICommentService
    {
        Task<List<Comment>> GetCommentsAsync(int? commentId = null, int? commentedBy = null, int? topicId = null);
        Task<int> AddCommentAsync(Comment comment);
        Task UpdateCommentAsync(int commentId, Comment comment);
        Task DeleteCommentAsync(int commentId);
    }

    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _commentRepository;

        public CommentService(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        public Task<List<Comment>> GetCommentsAsync(int? commentId = null, int? commentedBy = null, int? topicId = null)
        {
            return _commentRepository.GetCommentsAsync(commentId, commentedBy, topicId);
        }

        public Task<int> AddCommentAsync(Comment comment)
        {
            return _commentRepository.AddCommentAsync(comment);
        }

        public Task UpdateCommentAsync(int commentId, Comment comment)
        {
            return _commentRepository.UpdateCommentAsync(commentId, comment);
        }

        public Task DeleteCommentAsync(int commentId)
        {
            return _commentRepository.DeleteCommentAsync(commentId);
        }
    }
}
