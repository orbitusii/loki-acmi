using System.Text.RegularExpressions;

namespace acmi_interpreter;

/// <summary>
/// A readonly struct representing the bare text sent by an ACMI provider.<br/>
/// This is broken down into its component parts
/// </summary>
public readonly struct ACMIMessage
{
    const string CommaFinder = @"(?<![\\]),";
    private static readonly Regex CommaSplitter = new(CommaFinder, RegexOptions.CultureInvariant | RegexOptions.Singleline);

    public ACMIMessage(string text)
    {
        BareText = text;

        var matches = CommaSplitter.Matches(text);
        Segments = new string[matches.Count + 1];
        int lastPos = -1;
        for (int i = 0; i < matches.Count; i++)
        {
            int startAt = lastPos + 1;
            int index = matches[i].Index;
            Segments[i] = text.Substring(startAt, index - startAt);
            lastPos = index;
        }
        Segments[^1] = text.Substring(lastPos+1, text.Length - 1 - lastPos);
    }

    public string[] Segments { get; init; }

    public string BareText { get; init; }

    public bool IsGlobal => BareText.StartsWith("0,");
    public ulong ObjectID { get; init; } = 0;
}
