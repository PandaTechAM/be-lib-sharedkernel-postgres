namespace SharedKernel.Postgres.TempEFCoreNaming.Internal;

public interface INameRewriter
{
    string RewriteName(string name);
}