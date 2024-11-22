using System.Globalization;

namespace SharedKernel.Postgres.TempEFCoreNaming.Internal;

public class LowerCaseNameRewriter : INameRewriter
{
    private readonly CultureInfo _culture;

    public LowerCaseNameRewriter(CultureInfo culture)
        => _culture = culture;

    public string RewriteName(string name)
        => name.ToLower(_culture);
}
