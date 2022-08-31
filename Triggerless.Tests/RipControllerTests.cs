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
    public class RipControllerTests
    {
        [Test]
        public void IPTest()
        {
            var t = new TestRipController();
            var m = t.Get(55712186);

        }
    }

    public class TestRipController: RipController
    {
        public override string AllowedIPs => "98.201.77.24;143.95.252.34;72.82.28.25;70.139.139.165";

        public override string RemoteIP => "70.139.139.165";

    }
}
