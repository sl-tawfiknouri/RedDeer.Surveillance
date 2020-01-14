# AWS Lambda Empty Function Project

This starter project consists of:
* Function.cs - class file containing a class with a single function handler method

Install Amazon.Lambda.Tools Global Tools if not already installed.
```
    dotnet tool install -g Amazon.Lambda.Tools
```

If already installed check if new version is available.
```
    dotnet tool update -g Amazon.Lambda.Tools
```

Execute unit tests
```
    cd "RedDeer.Etl.SqlSriptExecutor.Lambda/test/RedDeer.Etl.SqlSriptExecutor.Lambda.Tests"
    dotnet test
```

Deploy function to AWS Lambda
```
    cd "RedDeer.Etl.SqlSriptExecutor.Lambda/src/RedDeer.Etl.SqlSriptExecutor.Lambda"
    dotnet lambda deploy-function
```
