using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;

namespace SEV.Common.Tests
{
    [TestFixture]
    public class DomainValidationExceptionTests
    {
        private const string TestError = "test error message";

        [Test]
        public void WhenCreateDomainValidationException_ThenShouldInitializeErrorsCollection()
        {
            var exception = new DomainValidationException();

            Assert.That(exception.Errors, Is.Not.Null);
            Assert.That(exception.Errors.Any(), Is.False);
        }

        [Test]
        public void GivenMessageParam_WhenCreateDomainValidationException_ThenShouldPutMessageInErrorsCollection()
        {
            var exception = new DomainValidationException(TestError);

            Assert.That(exception.Errors, Is.Not.Null);
            Assert.That(exception.Errors["model"], Is.Not.Null);
            Assert.That(exception.Errors["model"].Single(), Is.EqualTo(TestError));
            Assert.That(exception.Message, Is.EqualTo(TestError));
        }

        [Test]
        public void GivenMessageAndExceptionParams_WhenCreateDomainValidationException_ThenShouldPutMessageInErrorsCollection()
        {
            var innerEx = new Exception();

            var exception = new DomainValidationException(TestError, innerEx);

            Assert.That(exception.Errors, Is.Not.Null);
            Assert.That(exception.Errors["model"], Is.Not.Null);
            Assert.That(exception.Errors["model"].Single(), Is.EqualTo(TestError));
            Assert.That(exception.Message, Is.EqualTo(TestError));
            Assert.That(exception.InnerException, Is.SameAs(innerEx));
        }

        [Test]
        public void GivenValidationResultsParam_WhenCreateDomainValidationException_ThenShouldInitializeErrorsCollectionFromValidationResults()
        {
            const string testKey = "testprop";
            var results = new[]
            {
                new ValidationResult(TestError, new[] { testKey }), new ValidationResult(""), 
            };

            var exception = new DomainValidationException(results);

            Assert.That(exception.Errors, Is.Not.Null);
            Assert.That(exception.Errors[testKey], Is.Not.Null);
            Assert.That(exception.Errors[testKey].Single(), Is.EqualTo(TestError));
            Assert.That(exception.Errors["model"], Is.Not.Null);
            Assert.That(exception.Errors["model"].Single(), Is.EqualTo(string.Empty));
        }
    }
}
