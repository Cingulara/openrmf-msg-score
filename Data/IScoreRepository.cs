using openrmf_msg_score.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace openrmf_msg_score.Data {
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetAllScores();
        Task<Score> GetScore(string id);

        // query after multiple parameters
        Task<IEnumerable<Score>> GetScore(string bodyText, DateTime updatedFrom, long headerSizeLimit);

        // add new note document
        Task<Score> AddScore(Score item);

        // remove a single document
        Task<bool> RemoveScore(string id);

        // update just a single document
        Task<bool> UpdateScore(Score body);

    }
}