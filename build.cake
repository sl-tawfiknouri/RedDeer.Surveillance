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
	"src/Surveillance.Engine.DataCoordinator.Tests/Surveillance.Engine.DataCoordinator.Tests.csproj",
	"src/Surveillance.Engine.RuleDistributor.Tests/Surveillance.Engine.RuleDistributor.Tests.csproj",
	"src/Surveillance.Engine.Rules.Tests/Surveillance.Engine.Rules.Tests.csproj",
	"src/Surveillance/Surveillance.Tests/Surveillance.Tests.csproj" ,
	"src/Surveillance.System.DataLayer.Tests/Surveillance.Auditing.DataLayer.Tests.csproj" ,
	"src/Surveillance/Surveillance.DataLayer.Tests/Surveillance.DataLayer.Tests.csproj" ,
	"src/ThirdPartySurveillanceDataSynchroniser/ThirdPartySurveillanceDataSynchroniser.Tests/DataSynchroniser.Tests.csproj" ,
	"src/Surveillance.Specflow.Tests/Surveillance.Specflow.Tests.csproj",
	"src/DomainV2.Tests/Domain.Tests.csproj",
	"src/Utilities.Tests/Utilities.Tests.csproj"
};

var publishProjects = new List<Tuple<string,string, string,string>>
{  
	new Tuple<string,string,string,string> ("src/ThirdPartySurveillanceDataSynchroniser/App","DataSynchroniser.App.csproj" ,"DataSynchronizerService.zip","netcoreapp2.1" ),
    new Tuple<string,string,string,string> ("src/DataImport/App","DataImport.App.csproj", "DataImport.zip","netcoreapp2.0"),
	new Tuple<string,string,string,string> ("src/Surveillance/App", "Surveillance.App.csproj","SurveillanceService.zip","netcoreapp2.0" ),
    new Tuple<string,string,string,string> ("src/Test Harness/App", "","TestHarness.zip","netcoreapp2.0" )
};

var nugetPackageProjects = new List<string>
{
	"src/DomainV2/Domain.csproj",
	"src/DataImport/DataImport/DataImport.csproj",
	"src/DataSynchroniser.Api/DataSynchroniser.Api.csproj",
	"src/DataSynchroniser.Bmll/DataSynchroniser.Api.Bmll.csproj",
	"src/DataSynchroniser.Factset/DataSynchroniser.Api.Factset.csproj",
	"src/DataSynchroniser.Markit/DataSynchroniser.Api.Markit.csproj",
	"src/ThirdPartySurveillanceDataSynchroniser/ThirdPartySurveillanceDataSynchroniser/DataSynchroniser.csproj",
	"src/PollyFacade/PollyFacade.csproj",
	"src/Surveillance/Surveillance/Surveillance.csproj",
	"src/Surveillance/Surveillance.DataLayer/Surveillance.DataLayer.csproj",
	"src/Surveillance.System.Auditing/Surveillance.Auditing.csproj",
	"src/Surveillance.Engine/Surveillance.Engine.Rules.csproj",
	"src/Surveillance.Engine.DataCoordinator/Surveillance.Engine.DataCoordinator.csproj",
	"src/Surveillance.Engine.RuleDistributor/Surveillance.Engine.RuleDistributor.csproj",
	"src/Utilities/Utilities.csproj",
};

var nuspecProjects = new List<string>
{
	"src/Surveillance/Surveillance/Surveillance.nuspec"
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
			Information($"About to test {testProject}");
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

Task("Pack")
    .Does(() =>
	{
		if (!DirectoryExists("NugetPackages"))
		{
			CreateDirectory("NugetPackages");
		}

		CleanDirectory("NugetPackages");

	    foreach (var project in nuspecProjects)
	    {
	    	var packSettings = new DotNetCorePackSettings()
	        {
	            OutputDirectory = "NugetPackages"
	        };

        	var nupackSettings = new NuGetPackSettings() 
        	{
        		OutputDirectory = "NugetPackages"
        	}

	        NuGetPack(project, nupackSettings);
	    }
	});

Task("NoPublish")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Pack");

Task("BuildOnly")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Pack");

Task("PublishNoTests")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Publish")
	.IsDependentOn("Pack");

Task("Default")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Publish")
	.IsDependentOn("Pack");

RunTarget(target);
