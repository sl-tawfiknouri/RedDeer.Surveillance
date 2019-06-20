pipeline{
    agent { label 'Linux' }
    options { timestamps() }
    stages{
        stage('Build'){
	        environment {
                GITHUB_CREDENTIALS = credentials('reddeer-github-api')
                NUGET_PUBLISH_APIKEY = credentials('reddeer-nuget-deploy-key')
                NUGET_PUBLISH_REPOSITORY = "http://nexus.reddeer.local/repository/nuget-hosted/"
            }
	        steps{ sh "./build.sh" }
	    }
        stage('Publish'){
            steps{
                s3Upload consoleLogLevel: 'INFO',
                         dontWaitForConcurrentBuildCompletion: false,
                         entries: [[bucket: 'reddeer-releases/jenkins',
                                    excludedFile: '',
                                    flatten: false,
                                    gzipFiles: false,
                                    keepForever: false,
                                    managedArtifacts: true,
                                    noUploadOnFailure: true,
                                    selectedRegion: 'eu-west-1',
                                    showDirectlyInBrowser: false,
                                    sourceFile: '*.zip',
                                    storageClass: 'STANDARD',
                                    uploadFromSlave: true,
                                    useServerSideEncryption: false]],
                         pluginFailureResultConstraint: 'FAILURE',
                         profileName: 'JenkinsS3',
                         userMetadata: []
            }
        }
    }
}

