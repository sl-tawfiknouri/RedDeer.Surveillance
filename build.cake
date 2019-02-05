#addin "Cake.FileHelpers"
using System.Text.RegularExpressions;
using System;
using Cake.Common.IO;

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var pullRequstId = EnvironmentVariable("ghprbPullId") ?? "none";
var isReleaseBuild="false";
string release ="";

var BranchName = EnvironmentVariable("BRANCH") ?? "default value";
var BuildNumber = EnvironmentVariable("BUILD_NUMBER") ?? "0";
if (pullRequstId=="none" && BranchName.ToLower().Contains("release"))
{
	isReleaseBuild="true";
}
Information($"This Build release build status is {isReleaseBuild}");
System.Environment.SetEnvironmentVariable("isReleaseBuild",isReleaseBuild);
Information($"{DateTime.Now} BranchName {BranchName}");
Information($"{DateTime.Now} BuildNumber {BuildNumber}");

var solutions = new [] 
{
	"./src/Surveillance.All.sln"
};

var testProjects = new [] 
{
	"src/Test Harness/TestHarness.Tests/TestHarness.Tests.csproj" ,
	"src/DataImport/DataImport.Tests/DataImport.Tests.csproj" ,
	"src/Surveillance/Surveillance.DataLayer.Tests/Surveillance.DataLayer.Tests.csproj" ,
	"src/Surveillance/Surveillance.Tests/Surveillance.Tests.csproj" ,
	"src/Utilities.Tests/Utilities.Tests.csproj"  ,
	"src/Surveillance.System.DataLayer.Tests/Surveillance.System.DataLayer.Tests.csproj" ,
	"src/ThirdPartySurveillanceDataSynchroniser/ThirdPartySurveillanceDataSynchroniser.Tests/ThirdPartySurveillanceDataSynchroniser.Tests.csproj" ,
	"src/DomainV2.Tests/DomainV2.Tests.csproj",
	"src/Surveillance.Specflow.Tests/Surveillance.Specflow.Tests.csproj" 
};

var publishProjects = new List<Tuple<string,string, string,string>>
{  
	new Tuple<string,string,string,string> ("src/ThirdPartySurveillanceDataSynchroniser/App","ThirdPartySurveillanceDataSynchroniser.App.csproj" ,"DataSynchronizerService.zip","netcoreapp2.1" ),
    new Tuple<string,string,string,string> ("src/DataImport/App","DataImport.App.csproj", "DataImport.zip","netcoreapp2.0"),
	new Tuple<string,string,string,string> ("src/Surveillance/App", "Surveillance.App.csproj","SurveillanceService.zip","netcoreapp2.0" ),
    new Tuple<string,string,string,string> ("src/Test Harness/App", "","TestHarness.zip","netcoreapp2.0" )
};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("SetVersion")
	.Does(() => 
	{
		var assemblyversionfile = "./src/Domain/Properties/AssemblyInfo.cs";
		Regex pattern = new Regex(@"^release-v(?<releaseNumber>\d{1,3}\.\d{1,3}\.\d{1,3})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
		Match match = pattern.Match(BranchName);

		release = match.Groups["releaseNumber"].Value;
		if(release=="") 
		{ 
			release="1.0.0";
		}

		release=release + "." + BuildNumber;
		Information($"This Build version is {release}");

		ReplaceRegexInFiles(assemblyversionfile, "(?<=AssemblyVersion\\(\")(.+?)(?=\"\\))", release);
		ReplaceRegexInFiles(assemblyversionfile, "(?<=AssemblyFileVersion\\(\")(.+?)(?=\"\\))", release);
		ReplaceRegexInFiles(assemblyversionfile, "(?<=AssemblyInformationalVersion\\(\")(.+?)(?=\"\\))", release);
	});

Task("Build")
	.Does(() =>
    {
		var settings = new DotNetCoreBuildSettings
		{
			Configuration = "Release",
		};

		foreach (var solution in solutions)
		{			
			DotNetCoreBuild(solution, settings);
		}
    });
	
Task("Test")    
    .Does(() =>
	{	
		var settings = new DotNetCoreTestSettings
		{
			Configuration = "Release",
			Filter = "TestCategory != IgnoreOnBuildServer",
			NoBuild = true,
			NoRestore = true,
			ResultsDirectory = "./testresults",
			Verbosity = DotNetCoreVerbosity.Normal,
			Logger = "trx;LogFileName=unit_tests.xml"
		};

		foreach (var testProject in testProjects)
		{			
			DotNetCoreTest(testProject, settings);
		}
	});

Task("Publish")
	.Does(() =>
    {
		if (pullRequstId=="none")
		{
			foreach (var publishProject in publishProjects)
			{	
				Information($"******* publish {publishProject.Item1}");	
                var settings = new DotNetCorePublishSettings
				{				
					Configuration = "Release",
					Runtime = "ubuntu-x64",
					NoBuild=true,
					NoRestore=true,
					Verbosity=DotNetCoreVerbosity.Normal
				};	

				DotNetCorePublish(System.IO.Path.Combine(publishProject.Item1,publishProject.Item2), settings);
				if (FileExists(publishProject.Item3))
				{
					DeleteFile(publishProject.Item3);
				}

				Information($"******* Zip {publishProject.Item1}");	

				Zip($"{publishProject.Item1}/bin/Release/{publishProject.Item4}/ubuntu-x64", publishProject.Item3);

				Information($"******* Finished Zip {publishProject.Item1}");	
			}
	    } 
	});

Task("NoPublish")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test");

Task("BuildOnly")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build");

Task("PublishNoTests")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Publish");

Task("Default")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Publish");

RunTarget(target);
