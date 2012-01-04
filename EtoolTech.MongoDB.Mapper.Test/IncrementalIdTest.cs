﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EtoolTech.MongoDB.Mapper.Test
{
    using EtoolTech.MongoDB.Mapper.Test.Classes;

    [TestClass]
    public class IncrementalIdTest
    {
        [TestMethod]
        public void TestIncId()
        {
            Helper.Db.Drop();

            for (int i = 0; i < 100; i++)
            {
                Country c = new Country { Code = "ES_"+i.ToString(), Name = "España" };
                c.Save<Country>();
                Assert.AreEqual(c.MongoMapper_Id, i+1);                
            }

            
        }
    }
}