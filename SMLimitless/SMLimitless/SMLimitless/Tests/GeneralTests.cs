//-----------------------------------------------------------------------
// <copyright file="GeneralTests.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace SMLimitless.Tests
{
    /// <summary>
    /// A collection of general or miscellaneous tests.
    /// </summary>
    [TestFixture]
    public class GeneralTests
    {
        /// <summary>
        /// More of a developer test than a software test, really.
        /// </summary>
        [Test]
        public void MostBasicTest()
        {
            int result = 2 + 2;
            Assert.AreEqual(result, 4);
        }

        /// <summary>
        /// Likewise.
        /// </summary>
        [Test]
        public void AnotherBasicTest()
        {
            string[] strs = new[] { "Hello, ", "world!" };
            string result = string.Concat(strs);
            Assert.AreEqual(result, "Hello, world!");
        }
    }
}
