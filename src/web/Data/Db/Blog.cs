using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GiriGuru.Web.Data.Db
{
	public class Blog : IDisposable
	{
		// Has Dispose() already been called?
		Boolean isDisposed = false;

		public MongoClient client;
		public IMongoDatabase database;
		public IMongoCollection<BsonDocument> collectionActive;
		public IMongoCollection<BsonDocument> collectionBackup;

		public const string _collectionNameActive = "blog";
		public const string _collectionNameBackup = "blog_history";

		public Blog()
		{
			this.client = new MongoClient(AppSettings.DbSettings.GetClientSettings());
			this.database = client.GetDatabase(AppSettings.DbSettings.DatabaseName);
			this.collectionActive = database.GetCollection<BsonDocument>(_collectionNameActive);
			this.collectionBackup = database.GetCollection<BsonDocument>(_collectionNameBackup);

			//EnsureIndex();
		}

		public void EnsureIndex()
		{
			//collection.Indexes.DropAll();

			IndexKeysDefinition<BsonDocument> indexGuid = Builders<BsonDocument>.IndexKeys.Ascending("Guid");
			collectionActive.Indexes.CreateOneAsync(indexGuid, new CreateIndexOptions() { Unique = true });
			collectionBackup.Indexes.CreateOneAsync(indexGuid, new CreateIndexOptions() { Unique = false });
		}

		public void Update(Models.DbBlog blog)
		{
			//Backup existing content of the node
			BsonDocument bsonExisting = Read(blog.Guid);
			if (bsonExisting != null)
			{
				collectionBackup.InsertOne(bsonExisting);
				Delete(blog.Guid);
			}

			//Upsert new content

			if (blog._id == null)
				blog._id = ObjectId.GenerateNewId();

			if (blog._ts == DateTime.MinValue)
				blog._ts = DateTime.UtcNow;

			BsonDocument bson = BlogToBson(blog);

			collectionActive.InsertOne(bson);
		}

		public BsonDocument Read(Guid guid)
		{
			BsonDocument returnValue = null;

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Guid", guid.ToString());

			returnValue = collectionActive.Find<BsonDocument>(filter).SingleOrDefault();
			return returnValue;
		}

		public Models.DbBlog ReadAsBlog(Guid guid)
		{
			return BsonToBlog(Read(guid));
		}

		public void Delete(Guid guid)
		{
			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Guid", guid.ToString());

			DeleteResult result = collectionActive.DeleteOne(filter);
		}


		public BsonDocument BlogToBson(Models.DbBlog blog)
		{
			BsonDocument returnValue = null;

			if (blog != null)
			{
				BsonDocument bson = blog.ToBsonDocument();

				//Convert Guid into String
				bson["Guid"] = blog.Guid.ToString();

				returnValue = bson;
			}

			return returnValue;
		}


		public Models.DbBlog BsonToBlog(BsonDocument bson)
		{
			Models.DbBlog returnValue = BsonSerializer.Deserialize<Models.DbBlog>(bson);

			return returnValue;
		}

		#region Disposing
		// Implement IDisposable.
		public void Dispose()
		{
			ReleaseResources(true); // cleans both unmanaged and managed resources
			GC.SuppressFinalize(this); // supress finalization
		}

		protected void ReleaseResources(bool isFromDispose)
		{
			// Try to release resources only if they have not been previously released.
			if (!isDisposed)
			{
				if (isFromDispose)
				{
					// TODO: Release managed resources here
					// GC will automatically release Managed resources by calling the destructor,
					// but Dispose() need to release managed resources manually
				}
				//TODO: Release unmanaged resources here
				collectionActive = null;
				collectionBackup = null;
				database = null;
				client = null;
			}
			isDisposed = true; // Dispose() can be called numerous times
		}
		// Use C# destructor syntax for finalization code, invoked by GC only.
		~Blog()
		{
			// cleans only unmanaged stuffs
			ReleaseResources(false);
		}
		#endregion

	}
}
