// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
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
                return await _context.Scores
                        .Find(_ => true).ToListAsync();
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
                return await _context.Scores.Find(Score => Score.InternalId == GetInternalId(id)).FirstOrDefaultAsync();
        }

        // query after artifactId
        public async Task<Score> GetScorebyArtifact(string artifactId)
        {
                return await _context.Scores.Find(Score => Score.artifactId == GetInternalId(artifactId)).FirstOrDefaultAsync();
        }
        
        // query after body text, updated time, and header image size
        public async Task<IEnumerable<Score>> GetScore(string bodyText, DateTime updatedFrom, long headerSizeLimit)
        {
                var query = _context.Scores.Find(Score => Score.title.Contains(bodyText) &&
                                    Score.updatedOn >= updatedFrom);

                return await query.ToListAsync();
        }
        
        public async Task<IEnumerable<Score>> GetSystemScores(string systemGroupId)
        {
                return await _context.Scores
                    .Find(x => x.systemGroupId == systemGroupId).ToListAsync();
        }

        public async Task<Score> AddScore(Score item)
        {
                await _context.Scores.InsertOneAsync(item);
                return item;
        }

        public async Task<bool> RemoveScore(ObjectId id)
        {
                DeleteResult actionResult = await _context.Scores.DeleteOneAsync(Builders<Score>.Filter.Eq("artifactId", id));
                return actionResult.IsAcknowledged && actionResult.DeletedCount > 0;
        }

        private async Task<Score> GetScoreByArtifact(ObjectId artifactId)
        {
                return await _context.Scores.Find(Score => Score.artifactId == artifactId).FirstOrDefaultAsync();
        }
        
        public async Task<bool> UpdateScore(Score body)
        {
            var filter = Builders<Score>.Filter.Eq(s => s.artifactId, body.artifactId);
                // get the old InternalId as we are going off artifactid not InternalId for this
                var oldScore = await GetScoreByArtifact(body.artifactId);
                if (oldScore != null){
                    body.InternalId = oldScore.InternalId;
                }
                else
                {
                    body.created = DateTime.Now;
                    var result = await AddScore(body);
                    if (!result.InternalId.ToString().StartsWith("0000"))
                        return true;
                    else
                        return false;
                }
                var actionResult = await _context.Scores.ReplaceOneAsync(filter, body);
                if (actionResult.ModifiedCount == 0) { //never was entered, so Insert
                    body.created = DateTime.Now;
                    var result = await AddScore(body);
                    if (!result.InternalId.ToString().StartsWith("0000"))
                        return true;
                    else
                        return false;
                }
                return actionResult.IsAcknowledged && actionResult.ModifiedCount > 0;
        }
    }
}