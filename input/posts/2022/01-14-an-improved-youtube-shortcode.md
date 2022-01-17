Title: An Improved YouTube Shortcode for Statiq
Published: 01/14/2022 21:10
Tags:
  - statiq
  - statiq shortcode
  - youtube
Categories:
  - Statiq
---
# A YouTube Shortcode

**Q:** Why would we need a shortcode for YouTube when one already exists in
Statiq.Web?

**A:** The included one has several limitations, such as not allowing you to specify
a start time.

<!-- more -->

## The Code

For this approach, we build an `iframe` like the one you'd get via the *share
&gt; embed* option on the YouTube video.

```csharp
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
```

## Notes about the Code

Only the positional elements should be provided to the `ToDictionary()`
extension method. Any named arguments will be added even though they are not
listed here.

```csharp
var arguments = args.ToDictionary(/* positional element list */);
```

Any named arguments should be retrieved as such, with a default provided.

```csharp
var namedArgument = arguments.GetXXX(/* name */, /* default value */);
```

### Properly Terminate an iframe

Adding the empty content to the `iframe` element is important otherwise we end
up with an `iframe` that looks like:

```html
<iframe ... />
```

When we want a properly terminated element that doesn't "eat" everything that
follows it, like this:

```html
<iframe ...></iframe>
```

## The Shortcode

*Shortcode:*

```text
<?# YouTube dQw4w9WgXcQ Width=600 Height=337 StartTime=0 /?>
```

*HTML:*

```html
<div class="youtube">
  <iframe
    width="600"
    height="337"
    src="https://www.youtube.com/embed/dQw4w9WgXcQ?start=0"
    title="YouTube video player"
    frameborder="0"
    allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen="">
  </iframe>
</div>
```

*iframe embedded video:*

<?# YouTube dQw4w9WgXcQ Width=600 Height=337 StartTime=0 /?>
