import asyncio
import json
import logging
import time
from typing import Set
from aiohttp import web, WSMsgType, web_ws
import aiohttp_cors

# Configure logging
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

class GameServer:
    def __init__(self):
        self.clients: Set[web_ws.WebSocketResponse] = set()
        self.tick_count = 0
        self.running = False
        
    async def add_client(self, ws: web_ws.WebSocketResponse):
        """Add a new client to the game"""
        self.clients.add(ws)
        logger.info(f"Client connected. Total clients: {len(self.clients)}")
        
        # Send welcome message
        await self.send_to_client(ws, {
            "type": "welcome",
            "message": "Connected to game server",
            "tick": self.tick_count
        })
    
    async def remove_client(self, ws: web_ws.WebSocketResponse):
        """Remove a client from the game"""
        self.clients.discard(ws)
        logger.info(f"Client disconnected. Total clients: {len(self.clients)}")
    
    async def send_to_client(self, ws: web_ws.WebSocketResponse, data: dict):
        """Send data to a specific client"""
        try:
            await ws.send_str(json.dumps(data))
        except Exception as e:
            logger.error(f"Error sending to client: {e}")
            await self.remove_client(ws)
    
    async def broadcast(self, data: dict):
        """Send data to all connected clients"""
        if not self.clients:
            return
            
        # Create a copy of clients to avoid modification during iteration
        clients_copy = self.clients.copy()
        
        for ws in clients_copy:
            if ws.closed:
                await self.remove_client(ws)
            else:
                await self.send_to_client(ws, data)
    
    async def game_tick(self):
        """Main game tick - runs every 0.4 seconds"""
        self.tick_count += 1
        
        # Broadcast tick to all clients
        tick_data = {
            "type": "tick",
            "tick": self.tick_count,
            "timestamp": time.time(),
            "clients_count": len(self.clients)
        }
        
        await self.broadcast(tick_data)
        logger.info(f"Tick {self.tick_count} - {len(self.clients)} clients")
    
    async def start_game_loop(self):
        """Start the main game loop"""
        self.running = True
        logger.info("Game server started - tick every 0.4 seconds")
        
        while self.running:
            await self.game_tick()
            await asyncio.sleep(0.4)  # 0.4 second tick
    
    def stop(self):
        """Stop the game loop"""
        self.running = False
        logger.info("Game server stopped")

# Global game server instance
game_server = GameServer()

async def websocket_handler(request):
    """Handle WebSocket connections"""
    ws = web.WebSocketResponse()
    await ws.prepare(request)
    
    await game_server.add_client(ws)
    
    try:
        async for msg in ws:
            if msg.type == WSMsgType.TEXT:
                try:
                    data = json.loads(msg.data)
                    logger.info(f"Received from client: {data}")
                    
                    # Echo back any client messages with tick info
                    response = {
                        "type": "echo",
                        "original": data,
                        "tick": game_server.tick_count
                    }
                    await game_server.send_to_client(ws, response)
                    
                except json.JSONDecodeError:
                    logger.error(f"Invalid JSON received: {msg.data}")
                    
            elif msg.type == WSMsgType.ERROR:
                logger.error(f'WebSocket error: {ws.exception()}')
                break
                
    except Exception as e:
        logger.error(f"WebSocket handler error: {e}")
    finally:
        await game_server.remove_client(ws)
    
    return ws

async def health_check(request):
    """Health check endpoint"""
    return web.json_response({
        "status": "healthy",
        "tick": game_server.tick_count,
        "clients": len(game_server.clients),
        "uptime": time.time()
    })

async def init_app():
    """Initialize the web application"""
    app = web.Application()
    
    # Setup CORS
    cors = aiohttp_cors.setup(app, defaults={
        "*": aiohttp_cors.ResourceOptions(
            allow_credentials=True,
            expose_headers="*",
            allow_headers="*",
            allow_methods="*"
        )
    })
    
    # Add routes
    app.router.add_get('/ws', websocket_handler)
    app.router.add_get('/health', health_check)
    
    # Add CORS to all routes
    for route in list(app.router.routes()):
        cors.add(route)
    
    return app

async def main():
    """Main function to start the server"""
    app = await init_app()
    
    # Start the game loop in the background
    asyncio.create_task(game_server.start_game_loop())
    
    # Start the web server
    runner = web.AppRunner(app)
    await runner.setup()
    
    site = web.TCPSite(runner, '0.0.0.0', 8080)
    await site.start()
    
    logger.info("Game server running on http://0.0.0.0:8080")
    logger.info("WebSocket endpoint: ws://localhost:8080/ws")
    logger.info("Health check: http://localhost:8080/health")
    
    # Keep the server running
    try:
        await asyncio.Future()  # Run forever
    except KeyboardInterrupt:
        logger.info("Shutting down...")
        game_server.stop()
        await runner.cleanup()

if __name__ == '__main__':
    asyncio.run(main())
