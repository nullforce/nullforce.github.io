# nullforce-blog

This repo contains the source code and input for my blog at
https://nullforce.github.io.

The static files are generated with Statiq and committed to the GitHub Pages
repository at `nullforce/nullforce.github.io`.

## Made With

- .NET 9.0
- [Statiq](https://www.statiq.dev/)
- [CleanBlog Theme](https://github.com/statiqdev/CleanBlog)

## Preview

The blog can be previewed by running:

```powershell
dotnet run -- preview
```

## Deploy

Locally, a deployment can be done by running:

```powershell
dotnet run -- deploy
```

### Cloning

Clone the repository as usual, then fetch the submodules with:

```powershell
git submodule init
git submodule update
```
