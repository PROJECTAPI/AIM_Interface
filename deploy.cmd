set DEPLOY_FOLER=C:\inetpub\wwwroot\ResourceSchedulingApi
md %DEPLOY_FOLER%
del /F /Q %DEPLOY_FOLER%\*
xcopy /Y /E bin\Debug\* %DEPLOY_FOLER%