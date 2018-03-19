@echo off

SET GAME_INSTALL_PATH=.\pulsar_link
SET GAME_EXECUTABLE_NAME=PULSAR_LostColony.exe

SET HOOK_REGISTRY_PATH=.\HookRegistry\bin\Debug\HookRegistry.dll
SET HOOK_REGISTRY_FILE=.\HookRegistry\active_hooks

REM ***************************************
REM DO NOT change anything below this line!
REM ***************************************
SET GAME_INSTALL_PATH="%GAME_INSTALL_PATH%"

REM Undo the old patch and apply the new patch:
.\Hooker\bin\Debug\Hooker.exe restore -d %GAME_INSTALL_PATH%
.\Hooker\bin\Debug\Hooker.exe hook -d %GAME_INSTALL_PATH% -h "%HOOK_REGISTRY_FILE%" -l "%HOOK_REGISTRY_PATH%"

REM Run the game from its own base folder:
cd %GAME_INSTALL_PATH%
%GAME_EXECUTABLE_NAME%