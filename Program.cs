// See https://aka.ms/new-console-template for more information
using Microsoft.Playwright;
using System;
using System.Threading.Tasks;
using System.Linq;

class Program
{
    public static async Task Main(string[] args)
    {
        // Initialise Playwright instance
        using var playwright = await Playwright.CreateAsync();

        // Launch Chromium browser instance with GUI (Headless = false means browser UI will be visible)
        await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false
        });

        // Create a new browser context
        var context = await browser.NewContextAsync();
        // Open a new tab in the browser context
        var page = await context.NewPageAsync();

    // Step A: Login
        await page.GotoAsync("https://demo.snipeitapp.com/login"); //Go to login Page
        await page.FillAsync("#username", "admin"); // Default demo login credentials
        await page.FillAsync("#password", "password");
        await page.ClickAsync("button[type='submit']");
        await page.WaitForURLAsync("https://demo.snipeitapp.com/"); // Wait until the page URL changes to the dashboard URL after successful login

        Console.WriteLine("Logged in successfully!"); 

    // Step B: Navigate to Assets & Create Asset

        await page.ClickAsync("text=New");
        await page.ClickAsync("//nav//li[7]//li[1]/a");

        //Navigate to Create Assets Page
        string AssetTag = await page.Locator("#asset_tag").InputValueAsync();
        Console.WriteLine($"Asset Tag: {AssetTag}"); //print on console
        await page.WaitForTimeoutAsync(3000); // wait until the page loads

        //Select Model Id/asset
        await page.Locator("#model_id").ClickAsync();
        await page.FillAsync("//input[@type='search']", "Macbook Pro 13\""); //fill the search bar
        await page.WaitForTimeoutAsync(3000);
        await page.Keyboard.PressAsync("Enter");
        await page.WaitForTimeoutAsync(3000);

        // Breaking down the Model string into Model Name and Model Number
        string model = await page.Locator("//span[@id='select2-model_select_id-container']").GetAttributeAsync("title"); //
        string ModelName = "";
        string ModelNumber = "";

        // Check if model variable is not null
        if (!string.IsNullOrEmpty(model))
        {
            // Step 1: Remove "Laptops - " prefix
            string afterPrefix = model.Replace("Laptops - ", "").Trim();

            // Step 2: Extract model number inside parentheses
            int startIndex = afterPrefix.IndexOf("(#");
            int endIndex = afterPrefix.IndexOf(")", startIndex);

            if (startIndex >= 0 && endIndex > startIndex)
            {
                // Extract model number inside (#... )
                ModelNumber = afterPrefix.Substring(startIndex + 2, endIndex - (startIndex + 2)).Trim();

                // Step 3: Extract model name before "(#..."
                ModelName = afterPrefix.Substring(0, startIndex).Trim();
            }
            else
            {
                // Fallback: if format is unexpected, just take the whole afterPrefix as modelName
                ModelName = afterPrefix;
            }
        }

        Console.WriteLine($"Model Name: {ModelName}"); //Print Model Name to cosnole
        Console.WriteLine($"Model Number: {ModelNumber}"); //Print Model Number to cosnole

        //Select: Status Option
        await page.Locator("#status_select_id").SelectOptionAsync("Ready to Deploy");
        //Store the option for validation purpose
        string status = await page.Locator("//span[@id='select2-status_select_id-container']").GetAttributeAsync("title");
        Console.WriteLine($"Status: {status}");
       
        // Locate and Check the User option is selected
        var locator = page.Locator("//label[input[@value='user'] and contains(normalize-space(.), 'User')]");
        bool isChecked = await locator.Locator("input").IsCheckedAsync();

        //RANDOM USER Selection
        // Click dropdown to load users
        await page.Locator("#select2-assigned_user_select-container").ClickAsync();
       
         await page.WaitForTimeoutAsync(3000);
        // Wait for the AJAX results to appear (adjust selector based on DOM)
        await page.WaitForSelectorAsync(".select2-results__option");

        // Get all option texts except the placeholder
        var options = await page.Locator(".select2-results__option:not(:first-child)").AllInnerTextsAsync();

        // Create a Random instance to generate random numbers
        var random = new Random();
        // Select a random user from the list of available options
        var randomUser = options[random.Next(options.Count)];

        // Step 1: Split into name and user ID parts
        string[] parts = randomUser.Split(new string[] { " - #" }, StringSplitOptions.None);
        string namePart = parts[0].Trim();    // "LastName, FirstName (bracketed part)"
        string userId = parts[1].Trim();      // "UserID"

        // Step 2: Split last name and first name
        string[] nameSplit = namePart.Split(',');
        string lastName = nameSplit[0].Trim();   // "LastName"
        string firstName = nameSplit[1].Trim();  // "First (bracketed part)"

        // Step 3: Remove the bracketed part from first name
        int bracketIndex = firstName.IndexOf('(');
        if (bracketIndex >= 0)
        {
            firstName = firstName.Substring(0, bracketIndex).Trim();
        }

        // Step 4: Combine into "FirstName LastName"
         string UserName = $"{firstName} {lastName}";

        await page.FillAsync("//input[@type='search']", $"{userId}"); //search userID
        await page.WaitForTimeoutAsync(3000); //wait until the user appears
        await page.Keyboard.PressAsync("Enter"); //press Enter
        await page.WaitForTimeoutAsync(3000); //wait until the user is selected

        // Output
        Console.WriteLine($"Full Name: {UserName}");
        Console.WriteLine($"User ID: {userId}");

        //Click the save button to create asset
        await page.ClickAsync("text=Save");

        Console.WriteLine("Asset created!");
        await page.WaitForTimeoutAsync(3000); //wait until asset is created

    // Step C: Search asset
        await page.ClickAsync("text=Assets");  //click on asset icon on navbar
        await page.FillAsync("//input[@type='search']", AssetTag); //search asset via asset tag
        await page.WaitForTimeoutAsync(3000); //wait until asset tag input
        await page.Keyboard.PressAsync("Enter"); 
        await page.WaitForTimeoutAsync(3000); //wait until the asset loads

        if (await page.Locator($"//mark[text()='{AssetTag}']").IsVisibleAsync()) //verify Asset Tag 
        {
            if (await page.Locator($"//a[@data-original-title='user' and contains(normalize-space(text()), '{firstName}')]").IsVisibleAsync()) //verify the user
            {
                Console.WriteLine("Asset found in list!");
            }
        }
        else
        {
            Console.WriteLine("Asset not found in list!");
            return;
        }


    // Step D: Validate details

        await page.ClickAsync($"//mark[text()='{AssetTag}']"); //navigate to asset details page via asset list
        await page.WaitForTimeoutAsync(3000);
        //Model details validation
        if (await page.Locator($"//div[@class='col-md-9']/a[contains(normalize-space(text()), '{ModelName}')]").IsVisibleAsync()) //validates ModelName
        {
            if (await page.Locator($"//div[@class='col-md-9' and contains(normalize-space(text()), '{ModelNumber}')]").IsVisibleAsync()) //validates ModelNumber
            {
                Console.WriteLine("Model details validated!");
            }
        }

        //Status validation
        if (await page.Locator($"//div[@class='col-md-9' and contains(normalize-space(), '{status}')]").IsVisibleAsync()) //validates status
        {
            Console.WriteLine("Status validated!");
        }

        //User details validation 
        if (await page.Locator($"//div/a[text()='{UserName}']").IsVisibleAsync()) //validates UserName
        {
            if (await page.Locator($"//li[i[contains(@class,'fa-id-card')] and contains(normalize-space(.), '{userId}')]").IsVisibleAsync()) //validates UserId
            {
                Console.WriteLine("User details validated!");
            }
        }

    // Step E: Validate history tab

        await page.Locator("a[href='#history']").ClickAsync(); //Navigate to History Tab
        await page.WaitForTimeoutAsync(3000);
        if (await page.Locator($"//a[@data-original-title='user' and contains(normalize-space(text()), '{UserName}')]").IsVisibleAsync()) //checks if the user is correct
        {
            if (await page.Locator($"//td[text()='checkout']").IsVisibleAsync()) // checks if checkout is process
            {
                Console.WriteLine("History validated!");
            }
        }
        await browser.CloseAsync();
    }
}
