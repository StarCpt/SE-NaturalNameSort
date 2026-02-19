using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace NaturalBlockSorting;

public class NaturalStringComparer : IComparer<string>
{
    int IComparer<string>.Compare(string? x, string? y) => Compare(x, y);

    public static int Compare(string? left, string? right)
    {
        left = left?.TrimEnd() ?? string.Empty;
        right = right?.TrimEnd() ?? string.Empty;

        int leftStrLength = left.Length;
        int rightStrLength = right.Length;

        // get name length without trailing numbers
        while (leftStrLength > 0 && left[leftStrLength - 1] is >= '0' and <= '9')
            leftStrLength--;
        while (rightStrLength > 0 && right[rightStrLength - 1] is >= '0' and <= '9')
            rightStrLength--;

        // return early if strings without trailing numbers aren't the same
        // also return if one or both strings contain no trailing number
        if (leftStrLength != rightStrLength || leftStrLength == left.Length || rightStrLength == right.Length)
        {
            return left.CompareTo(right);
        }

        int stringOnlyComparison = string.Compare(left, 0, right, 0, leftStrLength);
        if (stringOnlyComparison != 0)
        {
            return stringOnlyComparison;
        }

        int leftNumberStart = leftStrLength;
        int rightNumberStart = rightStrLength;

        // trim leading zeroes
        while (leftNumberStart < left.Length && left[leftNumberStart] == '0')
            leftNumberStart++;
        while (rightNumberStart < right.Length && right[rightNumberStart] == '0')
            rightNumberStart++;

        string leftNumberStr = leftNumberStart < left.Length ? left.Substring(leftNumberStart) : "0";
        string rightNumberStr = rightNumberStart < right.Length ? right.Substring(rightNumberStart) : "0";

        int? result = null;

        // number may exceed ulong.MaxValue if over 19 characters
        if (leftNumberStr.Length > 19 || rightNumberStr.Length > 19)
        {
            if (BigInteger.TryParse(leftNumberStr, NumberStyles.None, CultureInfo.InvariantCulture, out BigInteger leftVal)
                && BigInteger.TryParse(rightNumberStr, NumberStyles.None, CultureInfo.InvariantCulture, out BigInteger rightVal))
            {
                result = leftVal.CompareTo(rightVal);
            }
        }
        else
        {
            if (ulong.TryParse(leftNumberStr, NumberStyles.None, CultureInfo.InvariantCulture, out ulong leftVal)
                && ulong.TryParse(rightNumberStr, NumberStyles.None, CultureInfo.InvariantCulture, out ulong rightVal))
            {
                result = leftVal.CompareTo(rightVal);
            }
        }

        if (result is 0)
        {
            // if number has leading zeroes, the one with more leading zeroes precedes the other
            int leftLeadingZeroes = leftNumberStart - leftStrLength;
            int rightLeadingZeroes = rightNumberStart - rightStrLength;

            // swap left/right since we want descending order (001, 01, 1)
            result = rightLeadingZeroes.CompareTo(leftLeadingZeroes);
        }

        // fall back to normal string comparison if we couldn't parse the number
        return result ?? left.CompareTo(right);
    }
}
