rem https://docs.microsoft.com/en-us/nuget/quickstart/create-and-publish-a-package-using-visual-studio?tabs=netcore-cli
rem this is a SDK project, right click on QuickJSON when in release build and Pack
copy bin\release\*.nupkg \code\examples\packages
