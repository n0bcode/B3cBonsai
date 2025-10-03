using Microsoft.Playwright;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;

namespace B3cBonsai.Tests
{
    public class LoginTests
    {
        [Fact]
        public async Task SuccessfulLogin()
        {
            using var playwright = await Playwright.CreateAsync();
            await using var browser = await playwright.Chromium.LaunchAsync(new() { Headless = false });
            var page = await browser.NewPageAsync();

            await page.GotoAsync("https://localhost:7224/Identity/Account/Login");

            // Retry mechanism for navigating to the login page
            const int maxRetries = 5;
            const int delayMilliseconds = 5000; // 5 seconds

            for (int i = 0; i < maxRetries; i++)
            {
                try
                {
                    await page.GotoAsync("https://localhost:7224/Identity/Account/Login", new PageGotoOptions { WaitUntil = WaitUntilState.Load, Timeout = 10000 });
                    break; // If navigation is successful, break the loop
                }
                catch (Microsoft.Playwright.PlaywrightException ex)
                {
                    if (i < maxRetries - 1)
                    {
                        Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}. Retrying in {delayMilliseconds / 1000} seconds...");
                        await Task.Delay(delayMilliseconds);
                    }
                    else
                    {
                        throw; // Re-throw the exception if all retries fail
                    }
                }
            }

            await page.FillAsync("input[name='Input.Email']", "employee.staff@gmail.com");
            await page.FillAsync("input[name='Input.Password']", "Abc@123");

            await page.ClickAsync("button[type='submit']");

            await page.WaitForURLAsync("https://localhost:7224/");

            var welcomeText = await page.TextContentAsync("a[title='Manage']");
            welcomeText.Should().Contain("Hello employee.staff@gmail.com!");
        }
    }
}
