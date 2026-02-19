using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;

namespace NaturalBlockSorting;

public class NaturalStringComparer : IComparer<string>
{
    int IComparer<string>.Compare(string x, string y) => Compare(x, y);

    public static int Compare(string left, string right)
    {
        int leftStrLength = left.Length;
        int rightStrLength = right.Length;

        // get name length without trailing numbers
        while (leftStrLength > 0 && left[leftStrLength - 1] is >= '0' and <= '9')
            leftStrLength--;
        while (rightStrLength > 0 && right[rightStrLength - 1] is >= '0' and <= '9')
            rightStrLength--;

        // return early if strings without trailing numbers aren't the same
        if (leftStrLength != rightStrLength)
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

        string leftNumberStr = left.Substring(leftNumberStart);
        string rightNumberStr = right.Substring(rightNumberStart);

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

        // fall back to normal string comparison if we couldn't parse the number
        return result ?? left.CompareTo(right);
    }
}
