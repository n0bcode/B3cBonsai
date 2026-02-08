#!/bin/bash

# Function to show usage
usage() {
    echo "Usage: $0 {db|web|all|down|logs}"
    echo "  db    - Start only the PostgreSQL database container"
    echo "  web   - Start the Web Application container (and its dependencies)"
    echo "  all   - Start all containers"
    echo "  down  - Stop and remove all containers"
    echo "  logs  - View logs for all containers (follow mode)"
    exit 1
}

# Check if docker compose is available
if command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE="docker-compose"
else
    DOCKER_COMPOSE="docker compose"
fi

# Main logic based on argument
case "$1" in
    db)
        echo "Starting PostgreSQL database..."
        $DOCKER_COMPOSE up -d db
        ;;
    web)
        echo "Starting Web Application..."
        $DOCKER_COMPOSE up -d server
        ;;
    all)
        echo "Starting all services..."
        $DOCKER_COMPOSE up -d
        ;;
    down)
        echo "Stopping all services..."
        $DOCKER_COMPOSE down
        ;;
    logs)
        echo " Following logs..."
        $DOCKER_COMPOSE logs -f
        ;;
    *)
        usage
        ;;
esac
