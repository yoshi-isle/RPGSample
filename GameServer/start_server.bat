@echo off
echo Starting GameServer...
echo.
echo Make sure you have installed the required packages:
echo pip install aiohttp aiohttp-cors
echo.
echo Server will start on http://localhost:8080
echo WebSocket endpoint: ws://localhost:8080/ws
echo Health check: http://localhost:8080/health
echo.
echo Press Ctrl+C to stop the server
echo.

python server.py

pause
