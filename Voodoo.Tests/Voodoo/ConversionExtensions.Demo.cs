﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Voodoo.Tests.Voodoo
{
    
    public class ConversionExtensions
    {
        public interface IHaveAProperty
        {         
            string AProperty { get; set; }
        }

        public class Foo : IHaveAProperty
        {
            public string AProperty { get; set; }
        }

        public class Bar : IHaveAProperty
        {
            public string AProperty { get; set; }
        }

        [Fact]
        public void ToAndAs()
        {
            var foo = new Foo() {AProperty = "A"};
            var bar = new Bar() { AProperty = "B" };

            var toInterface = foo.To<IHaveAProperty>();
            Assert.Equal(foo, toInterface);

            var cantCast = bar.To<Foo>();
            Assert.NotNull(bar);
            Assert.NotEqual<object>(bar, cantCast);

            decimal? number = null;
            Assert.Equal(null, number.As<decimal?>());
            Assert.Equal(0, number.As<decimal>());
        }
    }
}
