namespace DopplerBeplic.DopplerSecurity;

public static class Policies
{
    public const string Default = nameof(Default);
    public const string OnlySuperuser = nameof(OnlySuperuser);
    public const string OwnResourceOrSuperuser = nameof(OwnResourceOrSuperuser);
}
