using MongoDB.Driver;
using openrmf_msg_score.Models;

namespace openrmf_msg_score.Data
{
    public class ScoreContext
    {
        private readonly IMongoDatabase _database = null;

        public ScoreContext(Settings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            if (client != null)
                _database = client.GetDatabase(settings.Database);
        }

        public IMongoCollection<Score> Scores
        {
            get
            {
                return _database.GetCollection<Score>("Scores");
            }
        }
    }
}