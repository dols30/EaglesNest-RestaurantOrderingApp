using System.Globalization;

namespace backendKiosk.Project.Models;

public sealed record OrderTotals(decimal Subtotal, decimal Tax, decimal Tip, decimal Total)
{
    public const decimal TaxRate = 0.07m;

    public static OrderTotals Calculate(decimal subtotal, decimal tip)
    {
        if (subtotal < 0 || tip < 0) throw new ArgumentOutOfRangeException(nameof(tip), "Amounts must be nonnegative.");
        subtotal = decimal.Round(subtotal, 2, MidpointRounding.AwayFromZero);
        tip = decimal.Round(tip, 2, MidpointRounding.AwayFromZero);
        var tax = decimal.Round(subtotal * TaxRate, 2, MidpointRounding.AwayFromZero);
        return new(subtotal, tax, tip, subtotal + tax + tip);
    }

    public static bool TryParseTip(string? text, out decimal tip)
    {
        tip = 0;
        if (string.IsNullOrWhiteSpace(text)) return true;
        return decimal.TryParse(text.Trim(), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out tip) && tip >= 0;
    }
}
