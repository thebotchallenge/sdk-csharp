using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using vikebot.Encoding;

namespace vikebot.Test.Encoding
{
    [TestClass]
    public class DefaultEncodingTest
    {
        [TestMethod]
        public void StringEncodingTest()
        {
            Assert.AreEqual(System.Text.Encoding.UTF8, DefaultEncoding.String);
        }
    }
}
