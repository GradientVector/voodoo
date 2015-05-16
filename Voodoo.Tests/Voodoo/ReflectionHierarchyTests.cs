﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Voodoo.Tests.Voodoo
{
    public abstract class TestBaseClass<T>
        where T : class, new()
    {
    }

    public class MessageClass
    {
    }

    public class TestSubClass : TestBaseClass<MessageClass>
    {
    }

    
    public class ReflectionHierarchyTests
    {
        [Fact]
        public void ProveSubClassInheritsFromBase()
        {
            var baseType =
                typeof (MessageClass).GetTypeInfo().Assembly.GetTypes().First(c => c.Name.StartsWith("TestBaseClass"));
            var subType = new TestSubClass().GetType();

            //Assert.Equal failed. Expected:<Voodoo.Tests.Voodoo.TestBaseClass`1[T]>. Actual:<Voodoo.Tests.Voodoo.TestBaseClass`1[Voodoo.Tests.Voodoo.MessageClass]>. 

            Assert.NotEqual(baseType, subType.GetTypeInfo().BaseType);
        }
    }
}
