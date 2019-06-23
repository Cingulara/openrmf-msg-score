using openrmf_msg_score.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace openrmf_msg_score.Data {
    public class ScoreRepository : IScoreRepository
    {
        private readonly ScoreContext _context = null;

        public ScoreRepository(Settings settings)
        {
            _context = new ScoreContext(settings);
        }

        public async Task<IEnumerable<Score>> GetAllScores()
        {
            try
            {
                return await _context.Scores
                        .Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Score> GetScore(string id)
        {
            try
            {
                return await _context.Scores.Find(Score => Score.InternalId == GetInternalId(id)).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        // query after body text, updated time, and header image size
        //
        public async Task<IEnumerable<Score>> GetScore(string bodyText, DateTime updatedFrom, long headerSizeLimit)
        {
            try
            {
                var query = _context.Scores.Find(Score => Score.title.Contains(bodyText) &&
                                    Score.updatedOn >= updatedFrom);

                return await query.ToListAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        
        public async Task<Score> AddScore(Score item)
        {
            try
            {
                await _context.Scores.InsertOneAsync(item);
                return item;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveScore(ObjectId id)
        {
            try
            {
                DeleteResult actionResult = await _context.Scores.DeleteOneAsync(Builders<Score>.Filter.Eq("artifactId", id));
                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        private async Task<Score> GetScoreByArtifact(ObjectId artifactId)
        {
            try
            {
                return await _context.Scores.Find(Score => Score.artifactId == artifactId).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
        
        public async Task<bool> UpdateScore(Score body)
        {
            var filter = Builders<Score>.Filter.Eq(s => s.artifactId, body.artifactId);
            try
            {
                // get the old InternalId as we are going off artifactid not InternalId for this
                var oldScore = await GetScoreByArtifact(body.artifactId);
                if (oldScore != null){
                    body.InternalId = oldScore.InternalId;
                }
                else
                {
                    body.created = DateTime.Now;
                    var result = await AddScore(body);
                    if (result.InternalId != null && !result.InternalId.ToString().StartsWith("0000"))
                        return true;
                    else
                        return false;
                }
                var actionResult = await _context.Scores.ReplaceOneAsync(filter, body);
                if (actionResult.ModifiedCount == 0) { //never was entered, so Insert
                    body.created = DateTime.Now;
                    var result = await AddScore(body);
                    if (result.InternalId != null && !result.InternalId.ToString().StartsWith("0000"))
                        return true;
                    else
                        return false;
                }
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}