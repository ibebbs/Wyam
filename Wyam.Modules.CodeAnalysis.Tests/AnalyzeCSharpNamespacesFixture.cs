﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using Wyam.Common;

namespace Wyam.Modules.CodeAnalysis.Tests
{
    [TestFixture]
    public class AnalyzeCSharpNamespacesFixture
    {
        [Test]
        public void GetsTopLevelNamespaces()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new [] { string.Empty, "Foo", "Bar" }, results.Select(x => x["Name"]));
            stream.Dispose();
        }

        [Test]
        public void TopLevelNamespaceContainsDirectlyNestedNamespaces()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Baz
                {
                }

                namespace Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { string.Empty, "Foo", "Baz", "Bar" }, results.Select(x => x["Name"]));
            CollectionAssert.AreEquivalent(new [] { "Foo", "Bar" }, 
                results.Single(x => x["Name"].Equals(string.Empty)).Get<IEnumerable<IDocument>>("MemberNamespaces").Select(x => x["Name"]));
            stream.Dispose();
        }

        [Test]
        public void NestedNamespaceContainsDirectlyNestedNamespaces()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Baz
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { string.Empty, "Foo", "Baz", "Bar" }, results.Select(x => x["Name"]));
            CollectionAssert.AreEquivalent(new[] { "Baz", "Bar" },
                results.Single(x => x["Name"].Equals("Foo")).Get<IEnumerable<IDocument>>("MemberNamespaces").Select(x => x["Name"]));
            stream.Dispose();
        }

        [Test]
        public void NamespaceFullNameDoesNotContainFullHierarchy()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { string.Empty, "Foo", "Bar" }, results.Select(x => x["FullName"]));
            stream.Dispose();
        }

        [Test]
        public void NamespaceQualifiedNameContainsFullHierarchy()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { string.Empty, "Foo", "Foo.Bar" }, results.Select(x => x["QualifiedName"]));
            stream.Dispose();
        }

        [Test]
        public void NamespaceDisplayNameContainsFullHierarchy()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { string.Empty, "Foo", "Foo.Bar" }, results.Select(x => x["DisplayName"]));
            stream.Dispose();
        }

        [Test]
        public void NamespaceKindIsNamespace()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { "Namespace", "Namespace", "Namespace" }, results.Select(x => x["Kind"]));
            stream.Dispose();
        }

        [Test]
        public void NestedNamespacesReferenceParents()
        {
            // Given
            string code = @"
                namespace Foo
                {
                }

                namespace Foo.Bar
                {
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            Assert.AreEqual("Foo", results.Single(x => x["Name"].Equals("Bar")).Get<IDocument>("ContainingNamespace")["Name"]);
            Assert.AreEqual(string.Empty, results.Single(x => x["Name"].Equals("Foo")).Get<IDocument>("ContainingNamespace")["Name"]);
            stream.Dispose();
        }

        [Test]
        public void NamespacesContainTypes()
        {
            // Given
            string code = @"
                namespace Foo
                {
                    class Red
                    {
                    }
                }

                namespace Foo.Bar
                {
                    class Blue
                    {
                    }

                    class Green
                    {
                    }
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document }, context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new [] { "Red" }, 
                results.Single(x => x["Name"].Equals("Foo")).Get<IEnumerable<IDocument>>("MemberTypes").Select(x => x["Name"]));
            CollectionAssert.AreEquivalent(new[] { "Blue", "Green" },
                results.Single(x => x["Name"].Equals("Bar")).Get<IEnumerable<IDocument>>("MemberTypes").Select(x => x["Name"]));
            stream.Dispose();
        }

        [Test]
        public void NamespacesDoNotContainNestedTypes()
        {
            // Given
            string code = @"
                namespace Foo
                {
                    class Blue
                    {
                        class Green
                        {
                        }
                    }
                }
            ";
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(code));
            IDocument document = Substitute.For<IDocument>();
            document.GetStream().Returns(stream);
            IExecutionContext context = Substitute.For<IExecutionContext>();
            context.GetNewDocument(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<IEnumerable<KeyValuePair<string, object>>>())
                .Returns(x => new TestDocument((IEnumerable<KeyValuePair<string, object>>)x[2]));
            IModule module = new AnalyzeCSharp();

            // When
            List<IDocument> results = module.Execute(new[] { document },context).ToList();  // Make sure to materialize the result list

            // Then
            CollectionAssert.AreEquivalent(new[] { "Blue" }, 
                results.Single(x => x["Name"].Equals("Foo")).Get<IEnumerable<IDocument>>("MemberTypes").Select(x => x["Name"]));
            stream.Dispose();
        }

        // TODO: Test that FullName contains containing namespaces
    }

}
