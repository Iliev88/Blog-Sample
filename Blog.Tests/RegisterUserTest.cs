using Blog.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blog.Tests
{
    [TestFixture]
    public class RegisterUserTest
    {
        [Test]
        public void FullNameTestValid()
        {
            var user = new RegisterViewModel();
            user.FullName = "Iliya Iliev";
            var expected = "Iliya Iliev";
            
            var actual = user.FullName;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void FullNameTestMissing()
        {
            var user = new RegisterViewModel();
            user.FullName = "";
            var expected = "Iliya Iliev";

            var actual = user.FullName;

            Assert.AreEqual(expected, actual);
        }
    }
}
