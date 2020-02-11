@echo off
docker build -t luizclucas/igdm:latest .
docker push luizclucas/igdm:latest
pause
exit