﻿using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class IncrementalIdTest
    {
       
        [Test]
        public void TestChildIncrementalId()
        {
            Helper.DropAllCollections();

            var c = new Country {Code = "ES", Name = "España"};
            c.Save();

            //Insert de personas
            var p = new Person
                {
                    Name = "Pepito Perez",
                    Age = 35,
                    BirthDate = DateTime.Now.AddDays(57).AddYears(-35),
                    Married = true,
                    Country = "ES",
                    BankBalance = decimal.Parse("3500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 10, BirthDate = DateTime.Now.AddDays(57).AddYears(-10), Name = "Juan Perez"});
            p.Childs.Add(
                new Child {ID = 2, Age = 7, BirthDate = DateTime.Now.AddDays(57).AddYears(-7), Name = "Ana Perez"});

            p.Save();

            p = new Person
                {
                    Name = "Juanito Sanchez",
                    Age = 25,
                    BirthDate = DateTime.Now.AddDays(52).AddYears(-38),
                    Married = true,
                    Country = "ES",
                    BankBalance = decimal.Parse("1500,00")
                };

            p.Childs.Add(
                new Child {ID = 1, Age = 5, BirthDate = DateTime.Now.AddDays(7).AddYears(-5), Name = "Toni Sanchez"});

            p.Save();

            var Persons = new List<Person>();
            Persons.MongoFind();

            long index = 1;
            foreach (Person person in Persons)
            {
                foreach (Child child in person.Childs)
                {
                    Assert.AreEqual(child._id, index);
                    index ++;
                }
            }
        }

        [Test]
        public void TestIncId()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country {Code = "ES_" + i.ToString(), Name = "España"};
                c.Save();
                Assert.AreEqual(c.m_id, i + 1);
            }
        }
    }
}