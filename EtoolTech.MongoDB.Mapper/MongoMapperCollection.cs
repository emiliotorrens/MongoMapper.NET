﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Configuration;
using EtoolTech.MongoDB.Mapper.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EtoolTech.MongoDB.Mapper
{
    public class MongoMapperCollection<T> : IMongoMapperCollection<T>
    {
        public IFindFluent<T, T> Cursor { get; private set; }
        public FilterDefinition<T> LastQuery { get; private set; }

        public static MongoMapperCollection<T> Instance {get{return new MongoMapperCollection<T>();}}
        public static MongoMapperCollection<T> InstanceFromPrimary { get { return new MongoMapperCollection<T>(true); } }

        public bool FromPrimary { get; set; }

        private IMongoCollection<T> GetCollection()
        {
            return FromPrimary
                ? CollectionsManager.GetPrimaryCollection<T>(typeof (T).Name)
                : CollectionsManager.GetCollection<T>(typeof (T).Name);
        }
        
        public MongoMapperCollection()
        {
            FromPrimary = false;
            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(typeof(T).Name) * 1024 * 1024;
        }

        public MongoMapperCollection(bool FromPrimary)
        {
            this.FromPrimary = FromPrimary;
            BsonDefaults.MaxDocumentSize = ConfigManager.MaxDocumentSize(typeof(T).Name) * 1024 * 1024;
        }

        public IFindFluent<T, T> Find(FilterDefinition<T> Query)
        {
            LastQuery = Query;

            Cursor = GetCollection().Find(LastQuery);            
            return Cursor;
        }

        public IFindFluent<T, T> Find(string JsonQuery)
        {
            var document = ObjectSerializer.JsonStringToBsonDocument(JsonQuery);
            var query = new BsonDocument(document);
            LastQuery = query;

            Cursor = GetCollection().Find(LastQuery);            
            return Cursor;
        }

        public IFindFluent<T, T> Find(Expression<Func<T, object>> Field, object Value)
        {
            LastQuery = MongoQuery<T>.Eq(Field, Value);
            Cursor = GetCollection().Find<T>(LastQuery);
            return Cursor;
        }

        public IFindFluent<T, T> Find(string FieldName, object Value)
        {
            LastQuery = MongoQuery<T>.Eq(typeof (T).Name, FieldName, Value);
            Cursor = GetCollection().Find<T>(LastQuery);
            return Cursor;
        }

        public IFindFluent<T,T> Find()
        {
            LastQuery = new BsonDocument();
            Cursor = GetCollection().Find<T>(new BsonDocument());            
            return Cursor;
        }
   

        public T Pop(FilterDefinition<T> Query, SortDefinition<T> SortBy)
        {
            var col = GetCollection();
            
            FindOneAndDeleteOptions<T> args = new FindOneAndDeleteOptions<T>();
            args.Sort = SortBy;            

            var result = col.FindOneAndDeleteAsync( Query, args ).Result;

            return result;      

        }

        public T Pop()
        {
            return Pop(new BsonDocument(), Builders<T>.Sort.Ascending("$natural"));
        }

        public long Total
        {
            get
            {
                var t = GetCollection().Find(new BsonDocument()).CountAsync();
                t.Wait();
                return t.Result;
            }
        }

        public long Count 
        {
            get
            {
                var t = Cursor.CountAsync();
                t.Wait();
                return t.Result; ;
            }
        }

        public List<T> ToList()
        {
            return Cursor.ToListAsync().Result;
        }

        public T First()
        {
            return Cursor.FirstAsync().Result;            
        }

        public T Last()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<T> GetEnumerator()
        {                        
            return new MongoMapperEnumerator<T>(Cursor.ToCursorAsync().Result);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MongoMapperEnumerator<T> : IEnumerator<T>
    {
        private readonly IAsyncCursor<T> _enumerator;
        private T _current;

        public MongoMapperEnumerator(IAsyncCursor<T> Cursor)
        {
            _enumerator = Cursor;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            while (true)
            {
                bool hasNext = _enumerator.MoveNextAsync().Result;
                if (!hasNext) return false;
                _current = _enumerator.Current.First();
                return true;
            }            
        }

        public void Reset() { throw new NotImplementedException(); }

        public T Current { get { return _current; } }

        object IEnumerator.Current { get { return Current; } }
    }
}
