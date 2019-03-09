using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace norbot
{
    public static class Validators
    {
        public static ValidationResult IsValidPostcode(string postcode)
        {
            Regex regex = new Regex(@"([Gg][Ii][Rr] 0[Aa]{2})|((([A-Za-z][0-9]{1,2})|(([A-Za-z][A-Ha-hJ-Yj-y][0-9]{1,2})|(([A-Za-z][0-9][A-Za-z])|([A-Za-z][A-Ha-hJ-Yj-y][0-9][A-Za-z]?))))\s?[0-9][A-Za-z]{2})");
            Match match = regex.Match(postcode);
            if (match.Success)
            {
                return new ValidationResult(true, "Postcode", null);
            }
            else
            {
                return new ValidationResult(false, "Postcode", "Invalid postcode, please enter a valid UK postcode.");
            }
        }
        public static ValidationResult IsValidAreaPostcode(string postcode)
        {
            if (postcode.ToLower().StartsWith("nn"))
            {
                return new ValidationResult(true, "Postcode", null);
            }
            else
            {
                return new ValidationResult(false, "Postcode", "Postcode not in area, please enter a valid area postcode.");
            }
        }
    }
}
