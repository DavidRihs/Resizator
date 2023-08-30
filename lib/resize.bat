@echo off
setlocal
set percent=%1
set outputDir=%~2\resized
if not exist "%outputDir%" mkdir "%outputDir%" 
for %%i in ("%~2\*.jpg") do (
    echo Resizing "%%i"...
    ffmpeg -hide_banner -loglevel error -y -i "%%i" -vf "scale=iw*%percent%:ih*%percent%" "%outputDir%\%%~nxi"
)
echo All images resized and saved to "%outputDir%"