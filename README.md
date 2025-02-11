## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Provider Relationships Web

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_apis/build/status%2Fdas-provider-pr-web?repoName=SkillsFundingAgency%2Fdas-provider-pr-web&branchName=main)](https://sfa-gov-uk.visualstudio.com/Digital%20Apprenticeship%20Service/_build/latest?definitionId=3749&repoName=SkillsFundingAgency%2Fdas-provider-pr-web&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-provider-pr-web&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-provider-pr-web)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/4368171030/Solution+Architecture+-+PR#Initial-view-of-solution-architecture-for--EP%2FPP)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)

## About

This is part of the provider portal. Here provider users can invite new employers to come onboard or request for permissions from existing employers. 

## 🚀 Installation

### Pre-Requisites

* A clone of this repository
* A storage emulator like Azurite
* Visual studio 2022 or higher or similar IDE 

### Dependencies

* DfE Signin for user authentication
* The Outer API [das-apim-endpoints](https://github.com/SkillsFundingAgency/das-apim-endpoints/tree/master/src/ProviderPR) should be available either running locally or accessible in an Azure tenancy.


### Config

Following config files are required to be loaded in local storage emulator
* [SFA.DAS.ProviderPR.Web](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-provider-pr-web/SFA.DAS.ProviderPR.Web.json)
* [SFA.DAS.Employer.GovSignIn](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-shared-config/SFA.DAS.Provider.DfeSignIn.json)
* [SFA.DAS.Encoding](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-shared-config/SFA.DAS.Encoding.json)

AppSettings.Development.json file
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true;",
  "ConfigNames": "SFA.DAS.ProviderPR.Web,SFA.DAS.Provider.DfeSignIn,SFA.DAS.Encoding",
  "EnvironmentName": "LOCAL",
  "Version": "1.0",
  "cdn": {
    "url": "https://das-test-frnt-end.azureedge.net"
  },
  "ResourceEnvironmentName": "LOCAL",
  "StubProviderAuth": ""
}  
```

## Technologies

* .NetCore 8.0
* REDIS
* NUnit
* Moq
* FluentAssertions
