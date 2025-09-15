namespace RM.Mars.ParcelTracking.Extensions
{
    /// <summary>
    /// Extension methods for string validation and formatting.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Validates if the string is a valid parcel barcode.
        /// </summary>
        /// <param name="barcode">Barcode string to validate.</param>
        /// <returns>True if valid, otherwise false.</returns>
        public static bool IsValidBarcode(this string barcode)
        {
            const string barcodePrefix = "RMARS";
            const int numericSectionLength = 19;

            if (string.IsNullOrEmpty(barcode))
            {
                return false;
            }

            if(!barcode.StartsWith(barcodePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            int requiredLength = barcodePrefix.Length + numericSectionLength + 1;
            if (barcode.Length != requiredLength)
            {
                return false;
            }

            char checkSumChar = barcode[^1];
            return char.IsLetter(checkSumChar) && char.IsUpper(checkSumChar);
        }
    }
}
