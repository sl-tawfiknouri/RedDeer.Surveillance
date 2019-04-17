# Surveillance

## Purpose
* Import trade files
* Enrich trade file data via calls to external data providers (e.g. BMLL)
* Execute pre-configured rules to identify potential breaches
* Provide API services to other apps

## Surveillance Service
### Dependencies
#### Client Service 
|Configuration Setting|Example value|
|--|--|
|ClientServiceUrlAndPort|http://{baseurl}:8080|
|SurveillanceUserApiAccessToken|{any}|

#### Queues
|Configuration Setting|Example value|
|--|--|
|DataSynchronizerQueueName| {env}-surveillance-{client}-data-synchronizer-request |
| ScheduledRuleQueueName | {env}-surveillance-{client}-scheduledrule |
| ScheduleRuleDistributedWorkQueueName | {env}-surveillance-{client}-scheduledruledistributedwork|
| CaseMessageQueueName | {env}-surveillance-{client}-casemessage|
| TestRuleRunUpdateQueueName | {env}-surveillance-{client}-scheduledrule|
| UploadCoordinatorQueueName | {env}-surveillance-{client}-uploadcoordinator |

#### Database
|Configuration Setting|Example value|
|--|--|
|AuroraConnectionString|server=0.0.0.0; port=0000;uid=any;pwd='any';database={env}_{dbname}; Allow User Variables=True|

#### Pre-requisite configuration settings
| AutoScheduleRules | true |


## Data Importer Service
### Dependencies
#### Queues
|Configuration Setting|Example value|
|--|--|
| ScheduledRuleQueueName | {env}-surveillance-{client}-scheduledrule |
| ScheduleRuleDistributedWorkQueueName | {env}-surveillance-{client}-scheduledruledistributedwork |
| CaseMessageQueueName | {env}-surveillance-{client}-casemessage |
| DataImportS3UploadQueueName | {env}-surveillance-{client}-ftp |
| UploadCoordinatorQueueName | {env}-surveillance-{client}-uploadcoordinator |
#### Client Service
|Configuration Setting|Example value|
|--|--|
|ClientServiceUrlAndPort|http://{baseurl}:8080|
|SurveillanceUserApiAccessToken|{any}|

#### Surveillance DB
|Configuration Setting|Example value|
|--|--|
|AuroraConnectionString|server=0.0.0.0; port=0000;uid=any;pwd='any';database={env}_{dbname}; Allow User Variables=True|

#### Pre-requisite configuration settings
|Configuration Setting|Example value|
|--|--|
| DataImportTradeFileUploadDirectoryPath | {dirname_1} |
| DataImportEquityFileUploadDirectoryPath | {dirname_2} |
| DataImportAllocationFileUploadDirectoryPath | {dirname_3} |
| DataImportTradeFileFtpDirectoryPath | {dirname_1}\\FtpTrade |
| DataImportEquityFileFtpDirectoryPath | {dirname_2}\\FtpEquity |
| DataImportAllocationFileFtpDirectoryPath | {dirname_3}\\FtpAllocation |  
| DataImportEtlFileUploadDirectoryPath | {dirname_4} |
| DataImportEtlFileFtpDirectoryPath | {dirname_4}\\FtpEtl |


## Data Synchronizer Service
### Dependencies
#### BMLL (Firefly Backbone)
|Configuration Setting|Example value|
|--|--|
| BmllServiceUrlAndPort | http://{baseurl}:5000 |

#### FactSet (via Client Service)
|Configuration Setting|Example value|
|--|--|
| ClientServiceUrlAndPort |http://{baseurl}:8080 |
| SurveillanceUserApiAccessToken | {any} |

#### Surveillance DB
|Configuration Setting|Example value|
|--|--|
|AuroraConnectionString|server=0.0.0.0; port=0000;uid=any;pwd='any';database={env}_{dbname}; Allow User Variables=True|

#### Queues
|Configuration Setting|Example value|
|--|--|
| ScheduleRuleDistributedWorkQueueName | {env}-surveillance-{client}scheduledruledistributedwork |
| DataSynchronizerQueueName | {env}-surveillance-{client}-data-synchronizer-request |
  
### Depended On By
#### ClientService
_via direct access to Surveillance Db and via queues_
##### Surveillance Db
|Configuration Setting|Example value|
|--|--|
| SurveillanceConnectionString | server=0.0.0.0; port=0000;uid=any;pwd='any';database={env}_{dbname}; Allow User Variables=True |
| SurveillanceSqlCommandTimeout | 30 |
| AwsSurveillanceQueueName | {environment}-surveillance-{client}-casemessage |
| AwsSurveillanceTestRuleRunRequestQueueName | {environment}-surveillance-{client}-scheduledruledistributedwork |
| AwsSurveillanceTestRuleRunUpdateQueueName | {environment}-surveillance-{client}-scheduledruleupdates |

##### Queues
|Configuration Setting|Example value|
|--|--|
| ScheduleRuleDistributedWorkQueueName | {environment}-surveillance-{client}-scheduledruledistributedwork |
| CaseMessageQueueName | {environment}-surveillance-{client}-casemessage |
| TestRuleRunUpdateQueueName | {environment}-surveillance-{client}-scheduledrule-dlq |


## Surveillance GraphQL API Service
### Dependencies
#### No service dependencies

#### Queues
|Configuration Setting|Example value|
|--|--|
| NONE | |

#### Database
|Configuration Setting|Example value|
|--|--|
|SurveillanceApiConnectionString|server=0.0.0.0; port=0000;uid=any;pwd='any';database={env}_{dbname}; Allow User Variables=True|

#### Pre-requisite configuration settings
| AutoScheduleRules | true |
| Secret-Key-Jwt | JWT secret key! |
| IpRateLimiting |   "IpRateLimiting": {
    "EnableEndpointRateLimiting": false,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "10s",
        "Limit": 100
      },
      {
        "Endpoint": "*",
        "Period": "15m",
        "Limit": 8000
      },
      {
        "Endpoint": "*",
        "Period": "12h",
        "Limit": 130000
      },
      {
        "Endpoint": "*",
        "Period": "7d",
        "Limit": 1500000
      }
    ]
  },|
  | IpRateLimitPolicies |   "IpRateLimitPolicies": {
    "IpRules": []
  } |
  | SurveillanceApiUrl | https://localhost:1234 |

Note - SurveillanceApiUrl needs to be HTTPS | There must be a secret key jwt provided or the application will crash


==================== Surveillance analysis components ====================

To build, set up surveillance, then relay, then test harness.
Data stream runs from (upstream -> downstream) test harness; relay; surveillance service.

==================== A W S ====================

If you're unable to access AWS queues and are not on EC2 you will be in need of a credentials file. For a windows machine the file needs to be in a default location, one of which is 'C:\Users\your.username\.aws\credentials'. A credentials file consists of profiles which are defined by closed square brackets then followed by two lines of text. Below is an example of a plain text credentials files with two profiles. Paste in the relevant access and secret keys.

[development]
aws_access_key_id =
aws_secret_access_key =
[default]
aws_access_key_id =
aws_secret_access_key =

==================== END ====================
