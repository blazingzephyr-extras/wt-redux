@echo off

if '%4'=='x86' set /a isWin=true
if '%4'=='x64' set /a isWin=true

echo TargetDir: %1
echo SolutionDir: %2
echo Configuration: %3
echo Platform: %4

if defined isWin xcopy /s/y "%2\fnalibs3\%4\" "%1\"

echo FNA native libs successfully copied to the destination folder
exit 0
