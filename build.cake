#addin "Cake.FileHelpers"
using System.Text.RegularExpressions;
using System;
using Cake.Common.IO;

///////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var pullRequstId = EnvironmentVariable("ghprbPullId") ?? "none";
var isReleaseBuild="false";
string release ="";

var BranchName = EnvironmentVariable("GIT_BRANCH") ?? "default value";
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
	"src/TestHarness/TestHarness.Tests/TestHarness.Tests.csproj" ,
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
	"src/Infrastructure.Network.Tests/Infrastructure.Network.Tests.csproj",
	"src/Domain.Core.Tests/Domain.Core.Tests.csproj",
	"src/Domain.Surveillance.Tests/Domain.Surveillance.Tests.csproj",
	"src/Surveillance.Reddeer.ApiClient.Tests/Surveillance.Reddeer.ApiClient.Tests.csproj",
	"src/SharedKernel.Files.Tests/SharedKernel.Files.Tests.csproj",
	"src/DataImport/ETL/RedDeer.Etl.SqlSriptExecutor.Lambda.Tests/RedDeer.Etl.SqlSriptExecutor.Lambda.Tests.csproj",
	"src/DataImport/ETL/RedDeer.Etl.SqlSriptExecutor.Tests/RedDeer.Etl.SqlSriptExecutor.Tests.csproj"
};

var publishProjects = new List<Tuple<string,string, string,string>>
{  
	new Tuple<string,string,string,string> ("src/ThirdPartySurveillanceDataSynchroniser/App","DataSynchroniser.App.csproj" ,"DataSynchronizerService.zip","netcoreapp3.1" ),
    new Tuple<string,string,string,string> ("src/DataImport/App","DataImport.App.csproj", "DataImport.zip","netcoreapp3.1"),
	new Tuple<string,string,string,string> ("src/Surveillance/App", "Surveillance.App.csproj","SurveillanceService.zip","netcoreapp3.1" ),
    new Tuple<string,string,string,string> ("src/TestHarness/App", "","TestHarness.zip","netcoreapp3.1" ),
    new Tuple<string,string,string,string> ("src/Surveillance.Api.App", "Surveillance.Api.App.csproj","SurveillanceApi.zip","netcoreapp3.1" )
};

var nuspecProjects = new List<string>
{
	"src/Surveillance/Surveillance/Surveillance.nuspec",
	"src/DataImport/DataImport/DataImport.nuspec",
	"src/ThirdPartySurveillanceDataSynchroniser/ThirdPartySurveillanceDataSynchroniser/DataSynchroniser.nuspec",
	"src/DomainV2/Domain.nuspec",
	"src/PollyFacade/PollyFacade.nuspec",
	"src/Surveillance.System.DataLayer/Surveillance.Auditing.DataLayer.nuspec",
	"src/Surveillance/Surveillance.DataLayer/Surveillance.DataLayer.nuspec"
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

Task("ValidateBranch")
	.Does(()=>
	{
		var validBranchNames = new Regex(@"^(?:origin\/)?(release|uat|master|default|sur-[\d]{1,}|mo-[\d]{1,}|plat-[\d]{1,}|r[a-z]{1,3}?-[\d]{1,}|dan{1,3}?-[\d]{1,}|rc-v[\d\.]+)");//RM-123, sur-123, plat-123, RDPB-12345, DAN-xxx, rc-v1.1.1
		if (!validBranchNames.IsMatch(BranchName.ToLowerInvariant()))
		{
			throw new Exception($"Invalid branch name '{BranchName}'. Have you forgotten the Jira number prefix?");
		}
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
			Logger = "trx"
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
					NoBuild=false,
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
			Information($"******* Pack Creating Directory NugetPackages");	
			CreateDirectory("NugetPackages");
		}

		Information($"******* Pack Cleaning Directory NugetPackages");	
		CleanDirectory("NugetPackages");

		Information($"******* Pack looping through nuget package projects");
	    foreach (var project in nuspecProjects)
	    {
		    var nuGetPackSettings = new NuGetPackSettings
			{
				OutputDirectory = "NugetPackages",
				IncludeReferencedProjects = true,
	       		ArgumentCustomization = args => args.Append("-Prop Configuration=" + "release" + " -NoDefaultExcludes")
			};

			Information($"******* Pack Called for {project}");	
			NuGetPack(project, nuGetPackSettings);
	    }

		var packages = GetFiles("NugetPackages/*.nupkg");
   		foreach (var pack in packages)
   		{
	    	var iterItem = $"NugetPackages/{pack.GetFilename().ToString()}";
	    	Information($"******* Pack loop pushing {iterItem}");
			NuGetPush(iterItem, new NuGetPushSettings {
			     Source = "http://nexus.reddeer.local/repository/nuget-hosted/",
			     ApiKey = "a6ab623c-7cbc-3fc3-b9be-3236be4fdfa2",
			     Verbosity = NuGetVerbosity.Detailed
			 });
  		}
	});

Task("NoPublish")
	.IsDependentOn("ValidateBranch")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test");

Task("BuildOnly")
	.IsDependentOn("ValidateBranch")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test");

Task("PublishNoTests")
	.IsDependentOn("ValidateBranch")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Publish");

Task("Default")
	.IsDependentOn("ValidateBranch")
	.IsDependentOn("SetVersion")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Publish");

RunTarget(target);
