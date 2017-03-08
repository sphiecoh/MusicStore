var target          = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////
var isLocalBuild        = !AppVeyor.IsRunningOnAppVeyor;
var sourcePath          = Directory("./src/NancyMusicStore");
var testsPath           = Directory("test");
var buildArtifacts      = Directory("./artifacts/packages");

Task("Publish")
.IsDependentOn("RunTests")
    .Does(() =>
{
    var settings = new DotNetCorePublishSettings
     {
        // Framework = "netcoreapp1.0",
         Configuration = "Release",
         OutputDirectory = buildArtifacts
         
     };
     var projects = GetFiles("./**/project.json");

	foreach(var project in projects)
	{
        DotNetCorePublish(project.GetDirectory().FullPath, settings);
	    
    }
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .Does(() =>
{
	var projects = GetFiles("./**/project.json");

	foreach(var project in projects)
	{
        var settings = new DotNetCoreBuildSettings 
        {
            Configuration = configuration
            // Runtime = IsRunningOnWindows() ? null : "unix-x64"
        };

	    DotNetCoreBuild(project.GetDirectory().FullPath, settings); 
    }
});

Task("RunTests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var projects = GetFiles("./test/**/project.json");

    foreach(var project in projects)
	{
        var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration
        };
        
         DotNetCoreTest(project.GetDirectory().FullPath, settings);
    }
});

Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

Task("Restore")
    .Does(() =>
{
    var settings = new DotNetCoreRestoreSettings
    {
        Sources = new [] { "https://api.nuget.org/v3/index.json" }
    };

    DotNetCoreRestore("./src", settings);
    //DotNetCoreRestore(testsPath, settings);
});

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);