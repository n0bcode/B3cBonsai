#!/bin/bash

# Heroku Deployment Script for B3cBonsai
# This script helps deploy the application to Heroku

echo "🚀 Deploying B3cBonsai to Heroku..."

# Check if Heroku CLI is installed
if ! command -v heroku &> /dev/null; then
    echo "❌ Heroku CLI is not installed. Please install it first:"
    echo "   https://devcenter.heroku.com/articles/heroku-cli"
    exit 1
fi

# Check if logged in to Heroku
if ! heroku whoami &> /dev/null; then
    echo "❌ Please login to Heroku first:"
    echo "   heroku login"
    exit 1
fi

# Check if there's a Heroku remote
HEROKU_APP=$(git remote get-url heroku 2>/dev/null | sed -n 's/.*\/\([^\/]*\)\.git/\1/p')

if [ -z "$HEROKU_APP" ]; then
    echo "❌ No Heroku remote found."
    echo "📋 Please add your Heroku app as a remote first:"
    echo "   heroku git:remote -a your-app-name"
    exit 1
fi

echo "📱 Deploying to Heroku app: $HEROKU_APP"

# Check if we're on the main branch or if we need to specify a branch
CURRENT_BRANCH=$(git branch --show-current)

echo "🌿 Current branch: $CURRENT_BRANCH"

# Add all files
echo "📦 Adding files to git..."
git add .

# Commit changes
echo "💾 Committing changes..."
git commit -m "Deploy B3cBonsai to Heroku - $(date)"

if [ $? -ne 0 ]; then
    echo "⚠️  No changes to commit or commit failed"
fi

# Push to Heroku
echo "🚀 Pushing to Heroku..."
git push heroku $CURRENT_BRANCH:main

if [ $? -eq 0 ]; then
    echo "✅ Deployment successful!"
    echo ""
    echo "🌐 Your application should be available at:"
    echo "   https://$HEROKU_APP.herokuapp.com"
    echo ""
    echo "📋 Useful commands:"
    echo "   heroku logs --tail    # View live logs"
    echo "   heroku open          # Open app in browser"
    echo "   heroku ps            # Check dyno status"
    echo ""
    echo "🔧 If you need to run database migrations:"
    echo "   heroku run dotnet B3cBonsaiWeb.dll --migration"
else
    echo "❌ Deployment failed!"
    echo "🔍 Check the logs above for more details"
    exit 1
fi
