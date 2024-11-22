using System.Globalization;

namespace SharedKernel.Postgres.TempEFCoreNaming.Internal;

public class UpperCaseNameRewriter : INameRewriter
{
    private readonly CultureInfo _culture;

    public UpperCaseNameRewriter(CultureInfo culture)
        => _culture = culture;

    public string RewriteName(string name)
        => name.ToUpper(_culture);
}
