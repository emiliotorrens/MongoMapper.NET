﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using MongoDB.Bson;

namespace EtoolTech.MongoDB.Mapper
{
    internal static class ObjectSerializer
    {
        #region Methods

        public static string BsonDocumentToJsonString(BsonDocument Document)
        {
            return Document.ToJson();
        }


        public static BsonDocument JsonStringToBsonDocument(string JsonString)
        {
            BsonDocument document = BsonDocument.Parse(JsonString);
            return document;
        }

        internal static byte[] ToByteArray(Object Obj)
        {
            if (Obj == null)
            {
                return null;
            }

            byte[] data;
            using (var ms = new MemoryStream())
            {
                var b = new BinaryFormatter();
                b.Serialize(ms, Obj);
                data = ms.ToArray();
                ms.Close();
            }

            return data;
        }

        internal static T ToObjectSerialize<T>(byte[] SerializedObject)
        {
            if (SerializedObject == null)
            {
                return default(T);
            }

            Object obj;
            using (var ms = new MemoryStream())
            {
                ms.Write(SerializedObject, 0, SerializedObject.Length);
                ms.Seek(0, 0);
                var b = new BinaryFormatter();
                obj = b.Deserialize(ms);
                ms.Close();
            }
            return (T) obj;
        }

        #endregion
    }
}