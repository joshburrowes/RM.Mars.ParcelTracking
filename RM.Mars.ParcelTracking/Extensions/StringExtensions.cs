namespace RM.Mars.ParcelTracking.Extensions
{
    public static class StringExtensions
    {

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
