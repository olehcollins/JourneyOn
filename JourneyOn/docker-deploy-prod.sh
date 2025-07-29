#!/bin/bash

# Production deployment script for JourneyOn

set -e

echo "🏭 Building JourneyOn for production..."

# Build the Docker image for production
docker build -t journeyon-api:prod .

echo "✅ Production build completed!"

echo "🚀 Starting production container..."

# Stop any existing production container
docker stop journeyon-api-prod 2>/dev/null || true
docker rm journeyon-api-prod 2>/dev/null || true

# Start production container
docker run -d \
  --name journeyon-api-prod \
  -p 80:8080 \
  -e ASPNETCORE_ENVIRONMENT=Production \
  --restart unless-stopped \
  --memory=512m \
  journeyon-api:prod

echo "✅ Production container started!"

echo "📊 Container status:"
docker ps | grep journeyon-api-prod

echo ""
echo "🌐 Production API is running on port 80"
echo "📋 To view logs: docker logs -f journeyon-api-prod"
echo "🛑 To stop: docker stop journeyon-api-prod"
