#!/bin/bash

# Heroku Quick Fix Script for B3cBonsai
# Run this script to quickly fix common Heroku deployment issues

echo "🚀 Quick Fix for Heroku deployment issues..."

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

echo "📱 Fixing app: $HEROKU_APP"
echo ""

# 1. Add PostgreSQL if not exists
echo "1️⃣  Ensuring PostgreSQL database exists..."
if ! heroku addons --app $HEROKU_APP | grep -q "heroku-postgresql"; then
    echo "   Adding Heroku Postgres..."
    heroku addons:create heroku-postgresql:hobby-dev --app $HEROKU_APP
else
    echo "   ✅ PostgreSQL already exists"
fi

# 2. Set essential environment variables
echo ""
echo "2️⃣  Setting essential environment variables..."
heroku config:set ASPNETCORE_UsePostgreSql="true" --app $HEROKU_APP
heroku config:set ASPNETCORE_UseCloudinaryStorage="true" --app $HEROKU_APP
heroku config:set ASPNETCORE_AllowedHosts="*" --app $HEROKU_APP

# 3. Run database migrations
echo ""
echo "3️⃣  Running database migrations..."
heroku run dotnet ef database update --app $HEROKU_APP

# 4. Check app status
echo ""
echo "4️⃣  Checking application status..."
heroku ps --app $HEROKU_APP

# 5. Show recent logs
echo ""
echo "5️⃣  Recent application logs:"
heroku logs --app $HEROKU_APP --num 10

echo ""
echo "✅ Quick fix completed!"
echo ""
echo "🌐 Your app should be available at:"
echo "   https://$HEROKU_APP.herokuapp.com"
echo ""
echo "🔍 If you still see 'There's nothing here, yet.':"
echo "   - Wait a few minutes for the app to fully start"
echo "   - Run: heroku logs --tail"
echo "   - Run: ./heroku-debug.sh for detailed debugging"
echo ""
echo "📧 Don't forget to set your email password in Heroku dashboard!"
echo "   Dashboard: https://dashboard.heroku.com/apps/$HEROKU_APP/settings"
