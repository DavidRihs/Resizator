@echo off
setlocal
set target=%1
set outputDir="%~2\resized"
echo %target%
echo %outputDir%
if not exist %outputDir% mkdir %outputDir% 
for %%i in ("%~2\*.jpg") do (
    echo Resizing "%%i"...
    ffmpeg.exe -hide_banner -loglevel error -y -i "%%i" -vf "scale='if(gt(iw,ih),min(%target%,iw),-1):if(gt(iw,ih),-1,min(%target%,ih))" "%outputDir%\%%~nxi"
)
echo All images resized and saved to "%outputDir%"