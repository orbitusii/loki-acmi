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

        if(BareText.StartsWith('-'))
        {
            BareText.Remove(0, 1);
            IsDestroyed = true;
        }

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
    public bool IsDestroyed { get; init; } = false;
    public bool IsEvent => Segments[1].StartsWith("Event=");

    public ulong ObjectID => ulong.TryParse(Segments[0], System.Globalization.NumberStyles.AllowHexSpecifier, null, out ulong result) ? result : ulong.MaxValue;
}
