namespace Sunrise.Server.Types.Enums;

[Flags]
public enum LastFmfLags
{
    RunWithLdFlag = 1 << 14,
    ConsoleOpen = 1 << 15,
    ExtraThreads = 1 << 16,
    HqAssembly = 1 << 17,
    HqFile = 1 << 18,
    RegistryEdits = 1 << 19
}