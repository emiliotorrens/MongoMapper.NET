﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EtoolTech.MongoDB.Mapper.Exceptions;
using EtoolTech.MongoDB.Mapper.Interfaces;
using EtoolTech.MongoDB.Mapper.Configuration;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace EtoolTech.MongoDB.Mapper
{
    public class Finder : IFinder
    {
        internal static IFinder Instance {get {return new Finder();}}

        private Finder() {}

        private static void SaveOriginal<T>(T result)
        {
            var mongoMapperOriginable = result as IMongoMapperOriginable;
            if (mongoMapperOriginable != null)
            {
                (mongoMapperOriginable).SaveOriginal();
            }
        }

        #region FindAsList Methods

        public T FindById<T>(long id)
        {            
            var result = CollectionsManager.GetCollection(typeof(T).Name).FindOneByIdAs<T>(id);
            SaveOriginal(result);
            return result;
        }

    
        public object FindById(Type t, long id)
        {            
            var result = CollectionsManager.GetCollection(t.Name).FindOneByIdAs(t, id);
            SaveOriginal(result);
            return result;
        }       

        public BsonDocument FindBsonDocumentById<T>(long id)
        {            
            var result = CollectionsManager.GetCollection(typeof(T).Name).FindOneByIdAs<T>(id);
            return result.ToBsonDocument<T>();            
        }

        public T FindByKey<T>(params object[] values)
        {
            List<string> fields = Helper.GetPrimaryKey(typeof (T)).ToList();
            var keyValues = new Dictionary<string, object>();
            for (int i = 0; i < fields.Count; i++)
            {
                string field = fields[i].ToUpper() == "MongoMapper_id".ToUpper() ? "_id" : fields[i];
                keyValues.Add(field, values[i]);
            }

            return FindObjectByKey<T>(keyValues);
        }

        public T FindObjectByKey<T>(Dictionary<string, object> keyValues)
        {
            IMongoQuery query = Query.And(keyValues.Select(keyValue => MongoQuery.Eq(keyValue.Key, keyValue.Value)).ToArray());

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                throw new FindByKeyNotFoundException();
            }
            var o = result.First();
            SaveOriginal(o);
            return o;
        }

        public long FindIdByKey<T>(Dictionary<string, object> keyValues)
        {
            //Si la key es la interna y vieb
            if (keyValues.Count == 1 && keyValues.First().Key == "MongoMapper_Id" && keyValues.First().Value is long
                && (long) keyValues.First().Value == default(long))
            {
                return default(long);
            }

            IMongoQuery query = Query.And(keyValues.Select(keyValue => MongoQuery.Eq(keyValue.Key, keyValue.Value)).ToArray());

            MongoCursor<T> result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query).SetFields(Fields.Include("_id"));

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            if (result.Size() == 0)
            {
                return default(long);
            }

            return ((IMongoMapperIdeable) result.First()).MongoMapper_Id;
        }

        public List<T> FindAsList<T>(IMongoQuery query = null)
        {
            var list = FindAsCursor<T>(query).ToList();            
            foreach (var result in list)
            {
                SaveOriginal(result);
            }
            return list;

        }

        public MongoCursor<T> FindAsCursor<T>(IMongoQuery query = null)
        {
            if (query == null)
            {
                return CollectionsManager.GetCollection(typeof (T).Name).FindAllAs<T>();
            }

            var result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;
        }

        public List<T> FindAsList<T>(Expression<Func<T, object>> exp)
        {
            var list = FindAsCursor(exp).ToList();          
            foreach (var result in list)
            {
                SaveOriginal(result);
            }
            return list;
        }

        //TODO: Pendiente de probar
        public MongoCursor<T> FindAsCursor<T>(Expression<Func<T, object>> exp)
        {
            var ep = new ExpressionParser();
            IMongoQuery query = ep.ParseExpression(exp);
            
            var result = CollectionsManager.GetCollection(typeof (T).Name).FindAs<T>(query);

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine(result.Query.ToString());
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;

        }

        #endregion

        #region IFinder Members

        public List<T> AllAsList<T>()
        {
            var list =  AllAsCursor<T>().ToList();
            foreach (var result in list)
            {
                SaveOriginal(result);
            }
            return list;
        }

        public MongoCursor<T> AllAsCursor<T>()
        {
            var result = CollectionsManager.GetCollection(typeof (T).Name).FindAllAs<T>();

            if (ConfigManager.Out != null)
            {
                ConfigManager.Out.Write(String.Format("{0}: ", typeof(T).Name));
                ConfigManager.Out.WriteLine("{}");
                ConfigManager.Out.WriteLine(result.Explain().ToJson());
                ConfigManager.Out.WriteLine();
            }

            return result;
        }

        #endregion
    }
}