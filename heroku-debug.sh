#!/bin/bash

# Heroku Debug Script for B3cBonsai
# Run this script when you encounter "There's nothing here, yet." error

echo "🔍 Debugging Heroku deployment issues..."

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

echo "📱 Debugging app: $HEROKU_APP"
echo ""

# 1. Check application status
echo "1️⃣  Checking application status..."
heroku ps --app $HEROKU_APP

# 2. Check recent logs for errors
echo ""
echo "2️⃣  Checking recent logs..."
heroku logs --tail --app $HEROKU_APP --num 20

# 3. Check environment variables
echo ""
echo "3️⃣  Checking environment variables..."
heroku config --app $HEROKU_APP | grep -E "(ASPNETCORE|DATABASE_URL)"

# 4. Check if database is properly set up
echo ""
echo "4️⃣  Checking database status..."
if heroku pg:info --app $HEROKU_APP 2>/dev/null; then
    echo "   ✅ Database exists"
else
    echo "   ❌ No database found. Adding Heroku Postgres..."
    heroku addons:create heroku-postgresql:hobby-dev --app $HEROKU_APP
fi

# 5. Check if migrations need to be run
echo ""
echo "5️⃣  Checking if database migrations are needed..."
echo "   Running database update..."
heroku run dotnet ef database update --app $HEROKU_APP

# 6. Check build logs
echo ""
echo "6️⃣  Checking latest build..."
heroku builds --app $HEROKU_APP

echo ""
echo "🔧 Common fixes to try:"
echo ""
echo "   a) If you see database errors:"
echo "      heroku run dotnet ef database update --project B3cBonsai.DataAccess"
echo ""
echo "   b) If you see missing environment variables:"
echo "      heroku config:set ASPNETCORE_EmailSettings__Password=your_gmail_app_password"
echo ""
echo "   c) If build fails:"
echo "      heroku restart"
echo ""
echo "   d) If still not working:"
echo "      heroku logs --tail"
echo ""
echo "   e) Check your app URL:"
echo "      heroku open"
echo ""
echo "✅ Debug complete! Check the output above for any errors."
