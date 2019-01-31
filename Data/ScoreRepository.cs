using openstig_msg_score.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

namespace openstig_msg_score.Data {
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

        // query after Id or InternalId (BSonId value)
        //
        public async Task<Score> GetScore(string id)
        {
            try
            {
                ObjectId internalId = GetInternalId(id);
                return await _context.Scores
                                .Find(Score => Score.id == new Guid(id)).FirstOrDefaultAsync();
                                //|| Score.InternalId == internalId).FirstOrDefaultAsync();
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

        private ObjectId GetInternalId(string id)
        {
            ObjectId internalId;
            if (!ObjectId.TryParse(id, out internalId))
                internalId = ObjectId.Empty;

            return internalId;
        }
        
        public async Task AddScore(Score item)
        {
            try
            {
                await _context.Scores.InsertOneAsync(item);
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveScore(string id)
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Scores.DeleteOneAsync(
                        Builders<Score>.Filter.Eq("Id", id));

                return actionResult.IsAcknowledged 
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> UpdateScore(string id, Score body)
        {
            var filter = Builders<Score>.Filter.Eq(s => s.id.ToString(), id);
            var update = Builders<Score>.Update
                            .Set(s => s, body)
                            .CurrentDate(s => s.updatedOn);

            try
            {
                UpdateResult actionResult 
                    = await _context.Scores.UpdateOneAsync(filter, update);

                return actionResult.IsAcknowledged
                    && actionResult.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }

        public async Task<bool> RemoveAllScores()
        {
            try
            {
                DeleteResult actionResult 
                    = await _context.Scores.DeleteManyAsync(new BsonDocument());

                return actionResult.IsAcknowledged
                    && actionResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // log or manage the exception
                throw ex;
            }
        }
    }
}