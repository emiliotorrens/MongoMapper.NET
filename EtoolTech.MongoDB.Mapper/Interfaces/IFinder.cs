using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper.Interfaces
{
    public interface IFinder
    {
        #region Public Methods

        MongoCursor<T> AllAsCursor<T>();

        List<T> AllAsList<T>();

        MongoCursor<T> FindAsCursor<T>(IMongoQuery Query);

        List<T> FindAsList<T>(IMongoQuery Query);

        BsonDocument FindBsonDocumentById<T>(long Id);

        T FindById<T>(long Id);

        object FindById(Type Type, long Id);

        T FindByKey<T>(params object[] Values);

        long FindIdByKey<T>(Dictionary<string, object> KeyValues);

        long FindIdByKey(Type T, Dictionary<string, object> KeyValues);

        T FindObjectByKey<T>(Dictionary<string, object> KeyValues);

        #endregion
    }
}