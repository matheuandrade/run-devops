using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shopping.Api.Models;
using Shopping.Api.Utils;

namespace Shopping.Api.Services;

public class MongoDbService
{
    private readonly IMongoCollection<User> _usersCollection;

    public MongoDbService(IOptions<MongoDbSettings> mongoSettings)
    {
        var mongoClient = new MongoClient(mongoSettings.Value.ConnectionString);
        var mongoDatabase = mongoClient.GetDatabase(mongoSettings.Value.DatabaseName);
        _usersCollection = mongoDatabase.GetCollection<User>(mongoSettings.Value.CollectionName);
    }

    public async Task<List<User>> GetAsync() =>
        await _usersCollection.Find(_ => true).ToListAsync();

    public async Task<User?> GetAsync(string id) =>
        await _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public async Task CreateAsync(User newUser) =>
        await _usersCollection.InsertOneAsync(newUser);

    public async Task UpdateAsync(string id, User updatedUser) =>
        await _usersCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

    public async Task RemoveAsync(string id) =>
        await _usersCollection.DeleteOneAsync(x => x.Id == id);

    // 🔽 NEW SEED METHOD
    public async Task SeedAsync()
    {
        var count = await _usersCollection.CountDocumentsAsync(_ => true);
        if (count > 0) return; // already seeded

        var defaultUsers = new List<User>
        {
            new User { Name = "Alice Johnson", Email = "alice@example.com" },
            new User { Name = "Bob Smith", Email = "bob@example.com" },
            new User { Name = "Charlie Brown", Email = "charlie@example.com" }
        };

        await _usersCollection.InsertManyAsync(defaultUsers);
    }
}
