# BCJobs to Jobcast Job Push

Related to https://github.com/bcjobs/jobcentre-net/issues/1089

## Setup
* Run this SQL in database:
```
ALTER TABLE [Koopla].[Jobs] ADD [Source]  NVARCHAR (50)  NULL;
```
* Deploy `dev` branch using Visual Studio publishing
* Once `dev` branch is deployed, merge it into `master`
* Install dotnet core runtime:
  * https://dotnet.microsoft.com/download/dotnet-core/2.2
* Set up a scheduled task:
  * Action: `Start a program`
  * Program/script: `dotnet`
  * Arguments: `"{path to folder}\bcj2jc.dll" "https://www.bcjobs.ca/rss/jobs.xml?source=Jobcast&medium=Parner&campaign=SearchEngines" "{connection string}"`
