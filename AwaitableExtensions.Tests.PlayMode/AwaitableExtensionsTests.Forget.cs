using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using UnityEngine;

// ReSharper disable PartialTypeWithSinglePart

namespace AwaitableExtensions.Tests {
    sealed partial class AwaitableExtensionsTests {
        [Test]
        public void Forget() {
            var list = new List<Exception>();
            var exception = new Exception();
            TestCase().Invoking(x => x.Forget(list.Add, false)).Should().NotThrow();
            list.Should().ContainSingle().Which.Should().Be(exception);
            return;

            async Awaitable TestCase() {
                await Awaitable.MainThreadAsync();
                throw exception;
            }
        }
    }
}
