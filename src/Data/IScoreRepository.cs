// Copyright (c) Cingulara LLC 2019 and Tutela LLC 2019. All rights reserved.
// Licensed under the GNU GENERAL PUBLIC LICENSE Version 3, 29 June 2007 license. See LICENSE file in the project root for full license information.
using openrmf_msg_score.Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace openrmf_msg_score.Data {
    public interface IScoreRepository
    {
        Task<IEnumerable<Score>> GetAllScores();
        Task<Score> GetScore(string id);

        // query after multiple parameters
        Task<IEnumerable<Score>> GetScore(string bodyText, DateTime updatedFrom, long headerSizeLimit);

        // send back the score based on the checklist artifact Id
        Task<Score> GetScorebyArtifact(string artifactId);

        // get the scores for all the checklists in a system
        Task<IEnumerable<Score>> GetSystemScores(string systemGroupId);

        // add new note document
        Task<Score> AddScore(Score item);

        // remove a single document
        Task<bool> RemoveScore(ObjectId id);

        // update just a single document
        Task<bool> UpdateScore(Score body);

    }
}