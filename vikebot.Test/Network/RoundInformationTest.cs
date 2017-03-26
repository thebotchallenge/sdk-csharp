using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using vikebot.Network;

namespace vikebot.Test.Network
{
    [TestClass]
    public class RoundInformationTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            RoundInformation ri = new RoundInformation("xxx", "secretkey", "initvector", "gs01.vikebot.com", 2450);

            TestRoundInformationInstance(ri);
        }

        [TestMethod]
        public void FromJsonTest()
        {
            string json = "{\"ticket\":\"xxx\",\"aeskey\":\"secretkey\",\"aesiv\":\"initvector\",\"host\":\"gs01.vikebot.com\",\"port\":2450}";
            RoundInformation ri = JsonConvert.DeserializeObject<RoundInformation>(json);

            TestRoundInformationInstance(ri);
        }

        private void TestRoundInformationInstance(RoundInformation ri)
        {
            Assert.AreEqual("xxx", ri.Ticket);
            Assert.AreEqual("secretkey", ri.AesKey);
            Assert.AreEqual("initvector", ri.AesIv);
            Assert.AreEqual("gs01.vikebot.com", ri.Host);
            Assert.AreEqual(2450, ri.Port);
        }
    }
}
