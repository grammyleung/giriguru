using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace GiriGuru.Web.Data.Db
{
	public class Tag : IDisposable
	{
		// Has Dispose() already been called?
		Boolean isDisposed = false;

		public MongoClient client;
		public IMongoDatabase database;
		public IMongoCollection<BsonDocument> collection;

		public const string _collectionName = "tag";

		public Tag()
		{
			this.client = new MongoClient(AppSettings.DbSettings.GetClientSettings());
			this.database = client.GetDatabase(AppSettings.DbSettings.DatabaseName);
			this.collection = database.GetCollection<BsonDocument>(_collectionName);

			//EnsureIndex();
		}

		public void EnsureIndex()
		{
			//collection.Indexes.DropAll();

			IndexKeysDefinition<BsonDocument> indexGuid = Builders<BsonDocument>.IndexKeys.Ascending("Guid");
			collection.Indexes.CreateOneAsync(indexGuid, new CreateIndexOptions() { Unique = true });
		}

		public void Update(Models.DbTagGroup group)
		{
			//Backup existing content of the node
			BsonDocument bsonExisting = ReadGroup(group.Guid);
			if (bsonExisting != null)
			{
				Delete(group.Guid);
			}

			//Upsert new content

			if (group._id == null)
				group._id = ObjectId.GenerateNewId();

			if (group.Guid == Guid.Empty)
				group.Guid = Guid.NewGuid();

			if (group._tsInsert == DateTime.MinValue)
				group._tsInsert = DateTime.UtcNow;

			group._tsUpdate = DateTime.UtcNow;

			BsonDocument bson = GroupToBson(group);

			collection.InsertOne(bson);
		}

		public BsonDocument ReadGroup(Guid guid)
		{
			BsonDocument returnValue = null;

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Guid", guid.ToString());

			returnValue = collection.Find<BsonDocument>(filter).SingleOrDefault();
			return returnValue;
		}

		public BsonDocument ReadTag(Guid guid)
		{
			BsonDocument returnValue = null;

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Tags.Guid", guid.ToString());

			returnValue = collection.Find<BsonDocument>(filter).SingleOrDefault();
			return returnValue;
		}


		public Models.DbTagGroup[] ReadAll()
		{
			List<Models.DbTagGroup> returnValue = new List<Models.DbTagGroup>();

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Exists("_id");

			List<BsonDocument> listBson = collection.Find<BsonDocument>(filter).ToList();
			foreach (BsonDocument bson in listBson)
			{
				returnValue.Add(BsonToGroup(bson));
			}

			return returnValue.ToArray();
		}

		public Models.DbTagGroup ReadAsGroup(Guid guid)
		{
			return BsonToGroup(ReadGroup(guid));
		}

		public Models.DbTagGroup ReadAsGroupWithTag(Guid tagGuid)
		{
			return BsonToGroup(ReadTag(tagGuid));
		}

		public Models.DbTag ReadAsTag(Guid guid)
		{
			Models.DbTag returnValue = null;

			Models.DbTagGroup group = BsonToGroup(ReadTag(guid));

			if (group != null)
			{
				Models.DbTag tag = group.Tags.Where(i => i.Guid == guid).FirstOrDefault();
				if (tag != null)
				{
					returnValue = tag;
				}
			}

			return returnValue;
		}

		public void Delete(Guid guid)
		{
			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Guid", guid.ToString());

			DeleteResult result = collection.DeleteOne(filter);
		}


		public BsonDocument GroupToBson(Models.DbTagGroup group)
		{
			BsonDocument returnValue = null;

			if (group != null)
			{
				BsonDocument bson = group.ToBsonDocument();

				//Convert Guid into String
				bson["Guid"] = group.Guid.ToString();

				if (group.Tags != null)
				{
					for (int i = 0; i < group.Tags.Count; i++)
					{
						bson["Tags"][i]["Guid"] = group.Tags[i].Guid.ToString();
					}
				}

				returnValue = bson;
			}

			return returnValue;
		}

		public Models.DbTagGroup BsonToGroup(BsonDocument bson)
		{
			Models.DbTagGroup returnValue = null;

			if (bson != null)
			{
				returnValue = BsonSerializer.Deserialize<Models.DbTagGroup>(bson);
			}

			return returnValue;
		}


		public bool CheckGroupDuplication(Guid groupGuid, string text)
		{
			bool returnValue = false;

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Ne("Guid", groupGuid.ToString())
				& (filterBuilder.Eq("Name_en", text.Trim()) | filterBuilder.Eq("Name_zh", text.Trim()));

			returnValue = collection.Find<BsonDocument>(filter).Any();

			return returnValue;
		}

		public bool CheckTagDuplication(Guid tagGuid, string text)
		{
			bool returnValue = false;

			FilterDefinitionBuilder<BsonDocument> filterBuilder = Builders<BsonDocument>.Filter;
			FilterDefinition<BsonDocument> filter = filterBuilder.Eq("Tags.Name_en", text.Trim()) | filterBuilder.Eq("Tags.Name_zh", text.Trim());

			List<BsonDocument> listBson = collection.Find<BsonDocument>(filter).ToList();
			if (listBson.Count > 0)
			{
				if (listBson.Count() == 1)
				{
					Models.DbTagGroup group = BsonToGroup(listBson[0]);
					List<Models.DbTag> tags = group.Tags.Where(i => i.Name_en == text.Trim() || i.Name_zh == text.Trim()).ToList();
					if (tags.Count == 0)
					{
						if (tags.Count() > 1)
						{
							returnValue = true;
						}
						else if (tags[0].Guid != tagGuid)
						{
							returnValue = true;
						}
					}
				}
				else
				{
					returnValue = true;
				}
			}

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
				collection = null;
				database = null;
				client = null;
			}
			isDisposed = true; // Dispose() can be called numerous times
		}
		// Use C# destructor syntax for finalization code, invoked by GC only.
		~Tag()
		{
			// cleans only unmanaged stuffs
			ReleaseResources(false);
		}
		#endregion

	}
}
