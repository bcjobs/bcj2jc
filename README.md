# BCJobs to Jobcast Job Push

Related to https://github.com/bcjobs/jobcentre-net/issues/1089

## Instructions
* Run this SQL in database:
```
ALTER TABLE [Koopla].[Jobs] ADD [Source]  NVARCHAR (50)  NULL;
```
* Deploy `dev` branch
* Once `dev` branch is deployed, merge it into `master`
