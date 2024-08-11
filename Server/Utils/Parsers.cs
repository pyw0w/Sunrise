using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Paddings;
using Org.BouncyCastle.Crypto.Parameters;
using Sunrise.Server.Objects;

namespace Sunrise.Server.Utils;

public static class Parsers
{
    private const string StableKey = "osu!-scoreburgr---------";

    public static LoginRequest ParseLogin(string strToParse)
    {
        var lines = strToParse.Split('\n');

        if (lines.Length < 3)
        {
            throw new Exception("Login input string does not contain enough data.");
        }

        var clientEssentials = lines[2].Split('|');

        if (clientEssentials.Length < 4)
        {
            throw new Exception("Login input string does not contain enough client data.");
        }

        return new LoginRequest(lines[0], lines[1], clientEssentials[0], short.Parse(clientEssentials[1]), clientEssentials[2] == "1", clientEssentials[3], clientEssentials[4] == "1");
    }

    public static string ParseSubmittedScore(SubmitScoreRequest data)
    {
        var keyConcatenated = $"{StableKey}{data.OsuVersion}";
        var keyBytes = Encoding.Default.GetBytes(keyConcatenated);

        var ivBytes = Convert.FromBase64String(data.Iv!);
        var encodedStrBytes = Convert.FromBase64String(data.ScoreEncoded!);

        var engine = new RijndaelEngine(256);
        var blockCipher = new PaddedBufferedBlockCipher(new CbcBlockCipher(engine), new Pkcs7Padding());

        ICipherParameters keyParam = new KeyParameter(keyBytes);
        ICipherParameters parameters = new ParametersWithIV(keyParam, ivBytes);

        blockCipher.Init(false, parameters);

        var outputBytes = new byte[blockCipher.GetOutputSize(encodedStrBytes.Length)];
        var len = blockCipher.ProcessBytes(encodedStrBytes, 0, encodedStrBytes.Length, outputBytes, 0);

        Array.Resize(ref outputBytes, len);
        return Encoding.UTF8.GetString(outputBytes);
    }

    public static string SecondsToString(int seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);
        return time.ToString(@"mm\:ss");
    }
}