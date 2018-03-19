@echo off
REM This script sets up a link to the binaries of PULSAR.
REM Change the PULSAR_INSTALL_PATH value below before running this script.

REM Change the following location to the PULSAR installation folder!
REM Don't forget to include the final slash
SET PULSAR_INSTALL_PATH=C:\Program Files (x86)\Steam\steamapps\common\PULSARLostColony\

REM ***************************************
REM DO NOT change anything below this line!
REM ***************************************

REM folder of the current repo
SET REPO_PATH=%~dp0

SET REPO_LIB_PATH="%REPO_PATH%pulsar_link"
SET PULSAR_INSTALL_PATH="%PULSAR_INSTALL_PATH%"

IF EXIST %REPO_LIB_PATH% GOTO lib_exists
IF NOT EXIST %PULSAR_INSTALL_PATH% GOTO pulsar_wrong_path
REM No problems, just proceed
GOTO make_junction

:lib_exists
echo The folder already exists, press a key to delete it
echo -- %REPO_LIB_PATH%
pause
REM Just remove the library folder and continue
rmdir /s /q %REPO_LIB_PATH%

:make_junction
mklink /j %REPO_LIB_PATH% %PULSAR_INSTALL_PATH%
goto :eof

:pulsar_wrong_path
echo The path leading to the PULSAR binaries does not exist!
echo -- %PULSAR_INSTALL_PATH%
pause
