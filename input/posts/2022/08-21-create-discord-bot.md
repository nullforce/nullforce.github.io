Title: Creating a Discord Bot with Discord.Net and DSharpPlus
Published: 08/21/2022
IsPost: true
Tags:
  - discord
  - discord.net
  - dsharpplus
Categories:
  - Discord
---
# Creating a Discord Bot with Discord.Net and DSharpPlus

We create a basic Discord bot with a simple ping command that responds with a pong message.
We'll build the same bot with both the [Discord.Net][DiscordNetDocs] and [DSharpPlus][DSharpPlusDocs] libraries.

<!--more-->

Steps:
- Create a worker service
- Install and configure the Discord command library
- Add a ping command

Not covered:
- Creating a Discord API token for your bot (read the [Discord developer docs][DiscordDeveloperDocs])
- Adding your bot to a Discord server (read the [Discord developer docs][DiscordDeveloperDocs])
- Full source (available on [GitHub][SourceCodeGitHub])
- Hosting your bot (we may cover this later)

## The Code

The source code for this post is available on GitHub at [nullforce-public/discord-nullbot-net][SourceCodeGitHub].

### Create a Worker Service

We'll create a standard .NET **console** application and add a hosted worker service to it.

*Program.cs:*

```csharp
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nullbot;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();
```

*Worker.cs:*

```csharp
public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _provider;

    public Worker(
        ILogger<Worker> logger,
        IConfiguration config,
        IServiceProvider provider)
    {
        _logger = logger;
        _config = config;
        _provider = provider;
    }

    public override async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting NullBot...");
        await base.StartAsync(cancellationToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping NullBot...");
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(60 * 1000, stoppingToken);
        }
    }
}
```


## Install and Configure

Discord.Net only requires a single NuGet package, while DSharpPlus divides responsibilities across multiple NuGet packages.

### Install from nuget.org

For **Discord.Net**, install the following NuGet package(s):
- `Discord.Net` (3.7.2)

For **DSharpPlus**, install the following NuGet package(s):
- `DSharpPlus` (4.2.0)
- `DSharpPlus.CommandsNext` (4.2.0)

### Connecting to Discord

First we must connect the bot to Discord, both libraries have a very similar setup.

**Discord.Net** *StartAsync in Worker.cs:*

```csharp
// Initialize client
_discordClient = new DiscordSocketClient(new DiscordSocketConfig()
{
    GatewayIntents = GatewayIntents.AllUnprivileged,
    LogLevel = LogSeverity.Verbose,
    MessageCacheSize = 1_000,
});

...

// Connect to Discord
await _discordClient.LoginAsync(TokenType.Bot, discordToken);
await _discordClient.StartAsync();
```

**DSharpPlus** *StartAsync in Worker.cs:*

```csharp
// Initialize client
_discordClient = new DiscordClient(new DiscordConfiguration()
{
    Intents = DiscordIntents.AllUnprivileged,
    Token = discordToken,
    TokenType = TokenType.Bot,
});

...

// Connect to Discord
await _discordClient.ConnectAsync();
```


## Add a Command

While the code in the previous section connects the bot to Discord, it doesn't really do much else.
For that, we'll add a basic command.

To do so, we must:
- Create a module for the command
- Define the command and its interaction
- Register the command module within the library

### Command Module

Both libraries follow a similar approach:
1. Create a module class that derives from a base module class.
2. Create an async method for the command and decorate it with some attributes

**Discord.Net** *BasicCommands.cs:*

```csharp
using Discord.Commands;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Nullbot.Commands;

public class BasicCommands : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<BasicCommands> _logger;

    public BasicCommands(ILogger<BasicCommands> logger)
    {
        _logger = logger;
    }

    [Command("ping")]
    [Summary("Responds with a pong message.")]
    public async Task PingAsync() => await ReplyAsync("pong!");
}
```

**DSharpPlus** *BasicCommands.cs:*

```csharp
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Nullbot.Commands;

public class BasicCommands : BaseCommandModule
{
    private readonly ILogger<BasicCommands> _logger;

    public BasicCommands(ILogger<BasicCommands> logger)
    {
        _logger = logger;
    }

    [Command("ping")]
    [Description("Responds with a pong message.")]
    public async Task PingAsync(CommandContext context) => await context.RespondAsync("pong!");
}
```

### Register the Command Module and Commands

Finally, we'll need to tell the library about the command we created earlier.
Discord.Net is a little bit more verbose, but the process is similar for each library. Both
provide a method to register all command modules and commands within the assembly.

**Discord.Net** *StartAsync in Worker.cs:*

```csharp
// Setup text commands
_commands = new CommandService(new CommandServiceConfig()
{
    LogLevel = LogSeverity.Verbose,
    DefaultRunMode = Discord.Commands.RunMode.Async,
});

await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

// Listen for messages
_discordClient.MessageReceived += async (e) =>
{
    // Ignore system messages
    if (!(e is SocketUserMessage msg)) return;

    // Ignore bot users
    if (msg.Source != MessageSource.User) return;

    // Ignore self messages
    if (msg.Author.Id == _discordClient.CurrentUser.Id) return;

    var context = new SocketCommandContext(_discordClient, msg);

    // Check that the command starts with our bot prefix, or that it was sent
    // by mentioning the bot
    int argPos = 0;
    if (msg.HasStringPrefix(commandPrefix, ref argPos)
        || msg.HasMentionPrefix(_discordClient.CurrentUser, ref argPos))
    {
        var result = await _commands.ExecuteAsync(context, argPos, _provider);

        if (!result.IsSuccess)
        {
            await context.Channel.SendMessageAsync(result.ErrorReason);
        }
    }
};
```

**DSharpPlus** *StartAsync in Worker.cs:*

```csharp
// Setup commands
_commands = _discordClient.UseCommandsNext(new CommandsNextConfiguration
{
    Services = _provider,
    StringPrefixes = new[] { commandPrefix },
});

_commands.RegisterCommands(Assembly.GetEntryAssembly());
```

## Summary

Overall, they're both capable libraries and quite easy to setup and configure.

Discord.Net:
- Great docs on the website
- Easy setup

DSharpPlus:
- Good docs on the website, but not as well organized
- Easy setup
- Much less boilerplate than Discord.Net
- Automatically generates a help command

[DiscordDeveloperDocs]: https://discord.com/developers/docs/intro
[DiscordNetGitHub]: https://github.com/discord-net/Discord.Net
[DiscordNetDocs]: https://discordnet.dev/index.html
[DiscordNetNuGet]: https://www.nuget.org/packages/Discord.Net
[DSharpPlusGitHub]: https://github.com/DSharpPlus/DSharpPlus
[DSharpPlusDocs]: https://dsharpplus.github.io/index.html
[DSharpPlusNuGet]: https://www.nuget.org/packages/DSharpPlus
[DSharpPlusCommandsNextNuGet]: https://www.nuget.org/packages/DSharpPlus.CommandsNext
[SourceCodeGitHub]: https://github.com/nullforce-public/discord-nullbot-net
