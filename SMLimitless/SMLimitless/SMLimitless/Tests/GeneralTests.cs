//-----------------------------------------------------------------------
// <copyright file="GeneralTests.cs" company="Chris Akridge">
//     Copyrighted unter the MIT Public License.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using SMLimitless.Extensions;

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

        /// <summary>
        /// Tests that the Vector2Extensions.GetVectorEqualityTypes(IEnumerable{Vector2}) method works properly for four collections of vectors.
        /// </summary>
        [Test]
        public void VectorEqualityTypeTest()
        {
            bool check1 = false, check2 = false, check3 = false, check4 = false;
            List<Vector2> vectors = new List<Vector2>();
            vectors.Add(new Vector2(0f, 0f));
            vectors.Add(new Vector2(2945f, 294865f));
            check1 = Vector2Extensions.GetVectorEqualityTypes(vectors) == VectorCollectionEqualityTypes.NoEquality;

            vectors = new List<Vector2>();
            vectors.Add(new Vector2(0f, 0f));
            vectors.Add(new Vector2(0f, 16f));
            check2 = Vector2Extensions.GetVectorEqualityTypes(vectors) == VectorCollectionEqualityTypes.SomeXComponentsEqual;

            vectors = new List<Vector2>();
            vectors.Add(new Vector2(0f, 0f));
            vectors.Add(new Vector2(16f, 0f));
            check3 = Vector2Extensions.GetVectorEqualityTypes(vectors) == VectorCollectionEqualityTypes.SomeYComponentsEqual;

            vectors = new List<Vector2>();
            vectors.Add(new Vector2(0f, 0f));
            vectors.Add(new Vector2(0f, 0f));
            vectors.Add(new Vector2(3948575f, 39576421f));
            check4 = Vector2Extensions.GetVectorEqualityTypes(vectors) == VectorCollectionEqualityTypes.Both;

            Assert.IsTrue(check1 && check2 && check3 && check4);
        }
    }
}
