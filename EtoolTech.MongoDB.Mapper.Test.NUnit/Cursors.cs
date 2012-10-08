﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Core.Interfaces;
using NUnit.Framework;

namespace EtoolTech.MongoDB.Mapper.Test.NUnit
{
    [TestFixture]
    public class Cursors
    {
        [Test]
        public void Test()
        {
            Helper.DropAllCollections();

            for (int i = 0; i < 100; i++)
            {
                var c = new Country { Code = "ES_" + i.ToString(), Name = "España" };
                c.Save<Country>();
                Assert.AreEqual(c.MongoMapper_Id, i + 1);
            }

            MongoCursor<Country> countries = MongoMapper.FindAsCursor<Country>();
            

            if (countries.GetType().GetInterface("IMongoCursorEvents") != null)
            {
                countries.OnEnumeratorGetCurrent += Cursors_OnGetCurrent;
            }

            List<Country> c3 = countries.ToList();

            CountryEnumerable countryEnumerable = new CountryEnumerable(countries);

            
            List<Country> c1 = countryEnumerable.ToList();

            foreach (Country c in countryEnumerable)
            {
                string s = c.Code;
            }

            List<Country> c2 = countryEnumerable.ToList();

        }

        void Cursors_OnGetCurrent(object sender, System.EventArgs e)
        {
            Country c = ((MongoCursor<Country>.OnEnumeratorGetCurrentEventArgs)e).Document;
        }
    }

    public class CountryEnumerable : IEnumerable<Country>
    {
        private readonly IEnumerable<Country> _cursor;

        public CountryEnumerable(IEnumerable<Country> cursor) { _cursor = cursor; }

        public IEnumerator<Country> GetEnumerator() { return new CountryEnumerator(_cursor); }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }

    public class CountryEnumerator : IEnumerator<Country>
    {        
        private readonly IEnumerator<Country> _enumerator;
        private Country _current;        

        public CountryEnumerator(IEnumerable<Country> cursor) { _enumerator = cursor.GetEnumerator();}

        public void Dispose() {}

        public bool MoveNext()
        {
            while (true)
            {
                bool hasNext = _enumerator.MoveNext();
                if (!hasNext) return false;
                _current = _enumerator.Current;
                bool isValid = _current.Code.Contains("1");
                if (isValid) break;
            }
            return true;
        }

        public void Reset() { _enumerator.Reset(); }

        public Country Current { get { return _current; } }

        object IEnumerator.Current { get { return Current; } }
    }
}