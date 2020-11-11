@setlocal
@set _drop=drop
@set _log=%_drop%\Debug.log
@set _rc=0

@if "%PA_BT_ORG_URL%" equ "" goto :usage

dotnet clean OrgServiceSample.csproj
dotnet build --configuration debug --output %_drop% OrgServiceSample.csproj

@if exist %_log% del %_log%

@echo Launching exe...
@%_drop%\OrgServiceSample.exe %PA_BT_ORG_URL% %PA_BT_ORG_SPN_ID%
@if %ERRORLEVEL% NEQ 0 (set _rc=1)

@echo .
@echo debug.log: =========================
@type %_log%

@echo EXE finished with exit code: %_rc%
@exit /b %_rc%

:usage
    @echo "Must specify 3 pipeline/env variables: PA_BT_ORG_URL, PA_BT_ORG_SPN_ID, PA_BT_ORG_SPNKEY"
    @echo "alternatively, for username/password authN: PA_BT_ORG_USER, PA_BT_ORG_PASSWORD"
    @exit /b 1
