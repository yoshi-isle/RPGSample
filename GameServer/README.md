# Game Server

A simple Python aiohttp WebSocket game server with a 0.4-second tick system.

## Features

- WebSocket connections for real-time communication
- Game tick every 0.4 seconds
- Client connection management
- Health check endpoint
- Docker containerization
- CORS support

## Quick Start

### Using Docker Compose (Recommended)

1. Start the server:
```bash
docker-compose up -d
```

2. The server will be available at:
   - WebSocket: `ws://localhost:8080/ws`
   - Health check: `http://localhost:8080/health`

### Local Development

1. Install dependencies:
```bash
pip install -r requirements.txt
```

2. Run the server:
```bash
python server.py
```

## API Endpoints

### WebSocket: `/ws`
- Connect to receive game ticks every 0.4 seconds
- Send JSON messages to the server (will be echoed back)

### HTTP: `/health`
- Returns server status and current tick count

## Message Format

### Server to Client (Tick):
```json
{
  "type": "tick",
  "tick": 123,
  "timestamp": 1642780800.0,
  "clients_count": 5
}
```

### Server to Client (Welcome):
```json
{
  "type": "welcome",
  "message": "Connected to game server",
  "tick": 123
}
```

### Server to Client (Echo):
```json
{
  "type": "echo",
  "original": { "your": "message" },
  "tick": 123
}
```

## Testing

You can test the WebSocket connection using a simple JavaScript client:

```javascript
const ws = new WebSocket('ws://localhost:8080/ws');

ws.onopen = function() {
    console.log('Connected to game server');
};

ws.onmessage = function(event) {
    const data = JSON.parse(event.data);
    console.log('Received:', data);
};

// Send a test message
ws.send(JSON.stringify({ action: 'ping', data: 'hello' }));
```

## Docker Commands

- Start: `docker-compose up -d`
- Stop: `docker-compose down`
- View logs: `docker-compose logs -f`
- Rebuild: `docker-compose up --build -d`
