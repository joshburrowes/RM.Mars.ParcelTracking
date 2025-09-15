using FluentAssertions;
using NUnit.Framework;
using RM.Mars.ParcelTracking.Extensions;

namespace RM.Mars.ParcelTracking.Test.Extensions
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void IsValidBarcode_ReturnsFalse_WhenNullOrEmpty()
        {
            "".IsValidBarcode().Should().BeFalse();
            ((string)null).IsValidBarcode().Should().BeFalse();
        }

        [Test]
        public void IsValidBarcode_ReturnsFalse_WhenPrefixWrong()
        {
            "XXARS1234567890123456789M".IsValidBarcode().Should().BeFalse();
        }

        [Test]
        public void IsValidBarcode_ReturnsFalse_WhenLengthWrong()
        {
            "RMARS123456789012345678M".IsValidBarcode().Should().BeFalse();
            "RMARS12345678901234567890M".IsValidBarcode().Should().BeFalse();
        }

        [Test]
        public void IsValidBarcode_ReturnsFalse_WhenChecksumNotUpperLetter()
        {
            "RMARS1234567890123456789m".IsValidBarcode().Should().BeFalse();
            "RMARS12345678901234567891".IsValidBarcode().Should().BeFalse();
        }

        [Test]
        public void IsValidBarcode_ReturnsTrue_WhenValid()
        {
            "RMARS1234567890123456789M".IsValidBarcode().Should().BeTrue();
        }
    }
}
