﻿namespace EtoolTech.MongoDB.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using EtoolTech.MongoDB.Mapper.Interfaces;

    public static class ReflectionUtility
    {
        #region Public Methods

        public static void BuildSchema(Assembly assembly)
        {
            List<Type> types = assembly.GetTypes().Where(t => t.BaseType == typeof(MongoMapper)).ToList();
            foreach (Type type in types)
            {
                Helper.RebuildClass(type, true);
            }
        }

        public static void CopyObject<T>(T source, T destination)
        {
            Type t = source.GetType();

            foreach (PropertyInfo property in t.GetProperties())
            {
                property.SetValue(destination, property.GetValue(source, null), null);
            }
        }

        public static string GetPropertyName<T>(Expression<Func<T, object>> exp)
        {
            var memberExpression = exp.Body as UnaryExpression;
            if (memberExpression != null)
            {
                var memberexpresion2 = memberExpression.Operand as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }
            else
            {
                var memberexpresion2 = exp.Body as MemberExpression;
                if (memberexpresion2 != null)
                {
                    return memberexpresion2.Member.Name;
                }
            }

            return string.Empty;
        }

        public static object GetPropertyValue(object obj, string propertyName)
        {
            if ((propertyName == "MongoMapper_Id") && (obj is IMongoMapperIdeable))
            {
                return ((IMongoMapperIdeable)obj).MongoMapper_Id;
            }

            Type t = obj.GetType();
            PropertyInfo property = t.GetProperty(propertyName);
            return property.GetValue(obj, null);
        }

        public static T GetPropertyValue<T>(object obj, string propertyName)
        {
            return (T)GetPropertyValue(obj, propertyName);
        }

        #endregion
    }
}