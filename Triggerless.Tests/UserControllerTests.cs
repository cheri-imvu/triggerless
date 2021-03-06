using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Triggerless.API.Controllers;

namespace Triggerless.Tests
{
    [TestFixture]
    public class UserControllerTests
    {
        [Test]
        public async Task GetUser()
        {
            const long DIAMOND_BONES_ID = 34456817;
            var uc = new UserController();
            var user = await uc.GetUser(DIAMOND_BONES_ID);
            user.Dump();
            Assert.AreEqual("DiamondBones", user.AvatarName);
        }
    }
}
