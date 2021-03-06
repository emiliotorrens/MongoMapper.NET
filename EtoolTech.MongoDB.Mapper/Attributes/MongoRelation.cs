using System;
using System.Linq;

namespace EtoolTech.MongoDB.Mapper.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class MongoRelation : Attribute
    {

        public MongoRelation(string Name, string CurrentFieldNames, string RelationObjectName, string RelationFieldNames, bool UpRelation = false)
        {
            this.Name = Name;
            this.CurrentFieldNames = CurrentFieldNames.Split(',').Select(key => key.Trim()).ToArray();
            this.RelationObjectName = RelationObjectName;
            this.RelationFieldNames = RelationFieldNames.Split(',').Select(key => key.Trim()).ToArray();
            this.UpRelation = UpRelation;
        }
        
        #region Constants and Fields

        internal string Name;

        internal string[] CurrentFieldNames;

        internal string RelationObjectName;

        internal string[] RelationFieldNames;

        internal bool UpRelation { get; set; }

        #endregion
    }
}