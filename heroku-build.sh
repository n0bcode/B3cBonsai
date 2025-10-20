#!/bin/bash

# Heroku Build Script for B3cBonsai
# This script publishes the application for Heroku deployment

echo "🔨 Building B3cBonsai for Heroku deployment..."

# Check if .NET SDK is installed
if ! command -v dotnet &> /dev/null; then
    echo "❌ .NET SDK is not installed. Please install .NET 8.0 SDK first:"
    echo "   https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check .NET SDK version
DOTNET_VERSION=$(dotnet --version)
echo "📦 Using .NET SDK version: $DOTNET_VERSION"

# Navigate to the web project directory
cd B3cBonsaiWeb

# Clean previous builds
echo "🧹 Cleaning previous builds..."
dotnet clean

# Restore dependencies
echo "📦 Restoring dependencies..."
dotnet restore

# Publish the application for production
echo "🚀 Publishing application..."
dotnet publish --configuration Release --output ../published --runtime linux-x64 --self-contained false

if [ $? -eq 0 ]; then
    echo "✅ Build completed successfully!"
    echo "📁 Published application to: ../published/"

    # List the published files
    echo "📋 Published files:"
    ls -la ../published/

    echo ""
    echo "🌐 Your application is ready for Heroku deployment!"
    echo "💡 Next steps:"
    echo "   1. Commit and push these changes to your Heroku remote"
    echo "   2. Heroku will automatically build and deploy your application"
    echo "   3. Check the logs with: heroku logs --tail"
else
    echo "❌ Build failed!"
    exit 1
fi
