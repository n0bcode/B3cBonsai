#!/bin/bash

# Heroku Database Migration Script for B3cBonsai
# Run this script to apply database migrations on Heroku

echo "🗄️  Running database migrations on Heroku..."

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

# Get the app name from Heroku remote
HEROKU_APP=$(git remote get-url heroku 2>/dev/null | sed -n 's/.*\/\([^\/]*\)\.git/\1/p')

if [ -z "$HEROKU_APP" ]; then
    echo "❌ No Heroku remote found. Please add Heroku remote first:"
    echo "   heroku git:remote -a your-app-name"
    exit 1
fi

echo "📱 Applying migrations to app: $HEROKU_APP"

# Run database migrations
echo "🔄 Running Entity Framework migrations..."
heroku run dotnet ef database update --app $HEROKU_APP

# Check if migration was successful
if [ $? -eq 0 ]; then
    echo ""
    echo "✅ Database migrations completed successfully!"
    echo ""
    echo "🌐 You can now check your app at:"
    echo "   https://$HEROKU_APP.herokuapp.com"
    echo ""
    echo "🔍 If you still see errors, run the debug script:"
    echo "   ./heroku-debug.sh"
else
    echo ""
    echo "❌ Database migration failed. Check the errors above."
    echo ""
    echo "🔧 Common solutions:"
    echo "   1. Check if PostgreSQL add-on is properly attached"
    echo "   2. Verify DATABASE_URL environment variable"
    echo "   3. Check if all required environment variables are set"
    echo ""
    echo "🔍 Run the debug script for more details:"
    echo "   ./heroku-debug.sh"
fi
