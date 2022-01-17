using Statiq.Common;
using System.Xml.Linq;

namespace Nullforce.Statiq.Shortcodes;

public class YouTubeShortcode : SyncShortcode
{
    private const string Id = nameof(Id);
    private const string Width = nameof(Width);
    private const string Height = nameof(Height);
    private const string StartTime = nameof(StartTime);

    public override ShortcodeResult Execute(
        KeyValuePair<string, string>[] args,
        string content,
        IDocument document,
        IExecutionContext context)
    {
        // The positional arguments
        var arguments = args.ToDictionary(Id);
        arguments.RequireKeys(Id);

        // The named arguments
        var width = arguments.GetInt(Width, 560);
        var height = arguments.GetInt(Height, 315);
        var startTime = arguments.GetInt(StartTime, 0);

        var div = new XElement("div", new XAttribute("class", "youtube"));
        var iframe = new XElement(
            "iframe",
            new XAttribute("width", width),
            new XAttribute("height", height),
            new XAttribute("src", $"https://www.youtube.com/embed/{arguments[Id]}?start={startTime}"),
            new XAttribute("title", "YouTube video player"),
            new XAttribute("frameborder", "0"),
            new XAttribute("allow", "accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture"),
            new XAttribute("allowfullscreen", "")
        );

        // It's very important to have this content within an iframe, otherwise
        // the element isn't properly terminated
        iframe.Add(new XText(""));

        div.Add(iframe);

        return div.ToString();
    }
}
