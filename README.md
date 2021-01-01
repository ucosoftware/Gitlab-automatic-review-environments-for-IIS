# Gitlab automatic review environments for IIS

**How to**

* Change token and folder in MainController.cs
* Publish the app
* Create site in IIS and put published code in it
* Add review and stop_review stages, see gitlab docs https://docs.gitlab.com/ee/ci/environments/#example-of-setting-dynamic-environment-urls
* Install curl.exe and sed.exe

See .gitlab-ci.yml Example 
