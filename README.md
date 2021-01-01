# Gitlab automatic review environments for IIS

![.NET](https://github.com/ucosoftware/Gitlab-automatic-review-environments-for-IIS/workflows/.NET/badge.svg)

If you have IIS server with webdeploy and want to create automatic review environments on it 

* Change token and folder in MainController.cs
* Publish the app
* Create site in IIS and put published code in it
* Add review and stop_review stages, see gitlab docs https://docs.gitlab.com/ee/ci/environments/#example-of-setting-dynamic-environment-urls
* Install curl.exe and sed.exe

See .gitlab-ci.yml Example

* Line 7: Create site in IIS using code you published (change some.domain to domain name of your site and change token)
* Line 8: Replacing site name in webdeploy publish profile (you need to have one)
* Publishing site using webdeploy
* Line 27: Delete site on branch delete (change some.domain to domain name of your site)
