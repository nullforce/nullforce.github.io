Title: Getting Started with Statiq
Published: 01/14/2022 16:00
Tags:
  - statiq
Categories:
  - Statiq
---
# Getting Started

I'll cover the creating a Statiq Web blog with .NET 6 and doing some basic
configuration.

### Prerequisites

- .NET 6 (any supported .NET Core should work, but I'm covering 6.0 here)
- git

### 1. Create a .NET Console Application

```powershell
dotnet new console --name MySite
```

### 2. Install Statiq.Web

```powershell
dotnet add package Statiq.Web --prerelease
```

> Note: Since Statiq Web is still in prerelease, we'll have to include the `--prerelease`
flag.

### 3. Bootstrap the Blog

For a basic blog, this code should get you up and running.

*./Program.cs:*

```csharp
using Statiq.App;
using Statiq.Web;

await Bootstrapper
    .Factory
    .CreateWeb(args)
    .RunAsync();
```
> Note: This C# code may look a bit sparse if you're not familiar with .NET 6.0,
as much of the typical boilerplate has been removed.

### 4. Adding a Theme

The basic Statiq.Web doesn't provide a lot of functionality out of the box, so
we're going to install the Clean Blog Theme.

```powershell
git submodule add https://github.com/statiqdev/CleanBlog.git theme
```

> Note: I'm just adding a theme via a git submodule, but you could also copy the
theme files to a `theme` directory.

> Note: git submodules should not be added to a `.gitignore` file.

#### What does this do?

Adding the CleanBlog theme provides:
- A default layout and styling
- A Statiq.Framework `archive` for the blog posts
- Support for tags metadata in the front matter
- Searching if `GenerateSearchIndex` is set to `true`

### 5. Add a Blog Post

In the `posts` directory create a new file named `example.md`.

*./input/posts/example.md:*

```text
Title: An example
Published: 1/1/2022
---
This is my example blog post content.
```

> Note: An excerpt will be created from the first paragraph. If you want to
control where the excerpt ends, you can add a comment line `<!-- more -->`
in the file.

> Note: If you have multiple posts in a day, you can add a time after the
Published date, such as `Published: 1/1/2022 16:00`.

### 6. Run it

Run the console app with the `preview` verb and open `localhost:5080` in your
web browser.

```powershell
dotnet run -- preview
```


# Customizing your Blog

I'll cover some aspects that you should configure for your own blog.

### Create a Settings File

You should create a settings file in the root of your project. I'm using
`settings.yaml` here, but there are other options.

*./settings.yaml:*

```yaml
SiteTitle: nullforce
Copyright: Copyright © 2022 nullforce. All rights reserved.

GenerateSearchIndex: true
LinksUseHttps: true
Image: ./images/site-image.jpg
```

### Overriding the Clean Blog Theme

To override any SASS/CSS create an `_overrides.scss` in the `./input/scss`
directory.

*./input/scss/_overrides.scss:*

### More Settings from CleanBlog

You'll want to review the `README.md` on the CleanBlog repository.

See https://github.com/statiqdev/CleanBlog
