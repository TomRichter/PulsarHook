@echo off

SET GAME_INSTALL_PATH=.\pulsar_link
SET GAME_EXECUTABLE_NAME=PULSAR_LostColony.exe

SET HOOK_REGISTRY_BUILD_DIR=.\HookRegistry\bin\Debug
SET HOOK_REGISTRY_PATH=%HOOK_REGISTRY_BUILD_DIR%\HookRegistry.dll
SET HOOK_REGISTRY_FILE=.\HookRegistry\active_hooks
SET HARMONY_PATH=%HOOK_REGISTRY_BUILD_DIR%\0Harmony.dll

REM ***************************************
REM DO NOT change anything below this line!
REM ***************************************
SET GAME_INSTALL_PATH="%GAME_INSTALL_PATH%"

REM Undo the old patch and apply the new patch:
.\Hooker\bin\Debug\Hooker.exe restore -d %GAME_INSTALL_PATH%
COPY /Y %HARMONY_PATH% %GAME_INSTALL_PATH%\PULSAR_LostColony_Data\Managed\0Harmony.dll
.\Hooker\bin\Debug\Hooker.exe hook -d %GAME_INSTALL_PATH% -h "%HOOK_REGISTRY_FILE%" -l "%HOOK_REGISTRY_PATH%"

REM Run the game from its own base folder:
cd %GAME_INSTALL_PATH%
%GAME_EXECUTABLE_NAME%