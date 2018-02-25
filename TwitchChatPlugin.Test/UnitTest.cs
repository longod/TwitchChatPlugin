using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TwitchChatPlugin.Test {
    [TestClass]
    public class UnitTest {
        [TestMethod]
        public void TestCryption() {
            string p = "password";
            string e = Cryptography.Encrypt( p );
            string d = Cryptography.Decrypt( e );
            Assert.AreEqual( d, p );
        }
    }
}
