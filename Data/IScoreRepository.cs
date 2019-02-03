using openstig_msg_score.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace openstig_msg_score.Data {
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetAllScores();
        Task<Score> GetScore(string id);

        // query after multiple parameters
        Task<IEnumerable<Score>> GetScore(string bodyText, DateTime updatedFrom, long headerSizeLimit);

        // add new note document
        Task AddScore(Score item);

        // remove a single document
        Task<bool> RemoveScore(string id);

        // update just a single document
        Task<bool> UpdateScore(string id, Score body);

        // should be used with high cautious, only in relation with demo setup
        Task<bool> RemoveAllScores();
    }
}