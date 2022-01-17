Title: Deploy the Statiq Blog to GitHub Pages
Published: 01/16/2022 23:13
Tags:
  - statiq
  - github pages
  - github actions
Categories:
  - Statiq
---
# Let's Publish the Site

I'm publishing the blog to [GitHub Pages](https://pages.github.com/) for
a few reasons:

- Price: It's free
- The source code lives there
- GitHub Actions makes the CI/CD easy
- Statiq supports it via a `deploy` parameter
- They fit a dev focused blog

## Summary

We'll:
- Use a single repository for the source and content
- Use a GitHub Action to build and publish the static files to the `gh-pages`
branch

> Note: I chose to use the GITHUB_TOKEN provided by GitHub Actions, so I'm
limited to pushing changes to the same repository where the Actions Workflow
lives.

## Push the Code to GitHub

GitHub Pages requires a user page to use `<username>.github.io` for the
repository name, so that's what we'll do.

> Note: Since I've already created a local repository, I just need to add a
remote.

1. Create a GitHub repo named `nullforce.github.io`.
2. Add the remote to the local repo:

```powershell
git remote add origin git@github.com:nullforce/nullforce.github.io.git
```

3. Push the code:

```powershell
git push -u origin main
```

## Tell Statiq.Web where to Publish

Tell Statiq to publish the content to the `gh-pages` branch by
adding these settings to `settings.yaml`:

*./settings.yaml:*

```yaml
GitHubOwner: nullforce
GitHubName: nullforce.github.io
GitHubToken: "=> Config.FromSetting<string>(\"GITHUB_TOKEN\")"
```

> Note: Statiq defaults to deploying to the `gh-pages` branch.

> Note: You can provide a personal access token to Statiq to publish to a
GitHub Pages repo from your local dev box.

## GitHub Actions

We'll create a GitHub Action that builds our site whenever a push occurs on the
`main` branch. Create the following `deploy.yml` file in the `.github/workflows`
directory.

*./.github/workflows/deploy.yml:*

```yaml
name: Deploy Site
on:
  push:
    branches:
    - main

jobs:
  build:
    runs-on: windows-latest
    steps:
    - uses: actions/setup-dotnet@v1
      with:
        dotnet-version: '6.0.101'
    - uses: actions/checkout@main
      with:
        submodules: recursive
    - run: dotnet run -- deploy
      env:
        GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
```

> Note: My repository is setup with permissive rights to the `GITHUB_TOKEN`, if
yours isn't you may have to specify the permissions for the build job.

## Tell GitHub not to Expect Jekyll

GitHub expects a Jekyll site and we need to tell it not to do so. Create an
empty `.nojekyll` file in the `input` directory to tell GitHub we're not using
Jekyll.

*./input/.nojekyll:*

```text
```

## Finally

Push the changes to the remote repo and the GitHub Action should trigger and run
`dotnet run -- deploy`. If everything was done correctly, we'll have a running
site at `https://<username>.github.io`.
