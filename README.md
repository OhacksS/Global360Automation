# SnipeIT APP Automation Script (.NET 9.0 + Playwright)

## Description

This project automates asset management tasks on the [SnipeIT demo site](https://demo.snipeitapp.com/login) using **.NET 9.0**, **PowerShell 7.5.2**, and the latest version of **Microsoft Playwright** for browser automation.

The automation script performs these key steps in a single `Program.cs` file, organized from Step A to Step E:

- **Step A:** Login to the SnipeIT demo site as an admin user.
- **Step B:** Create a new **Macbook Pro 13"** asset with status "Ready to Deploy" and assign it to a random user.
- **Step C:** Find the newly created asset in the asset list to verify it was successfully created.
- **Step D:** Navigate to the asset page and validate relevant asset details.
- **Step E:** Validate the details in the **History** tab on the asset page.

To see the complete working of the solution, I have prepared a video that you will find in this Repository in the name 'Demo Video.mp4'.

### Challenges faced during Development

1. Handling dynamic UI elements and selectors that occasionally change or load asynchronously.
   - In order to overcome this challenge, I created dynamic Xpath expression and stored it in a variable for flexible use. 
2. Ensuring the automation script waits efficiently for page elements without relying on fixed delays.
   - I used Playwright’s explicit wait methods such as `WaitForSelectorAsync` and `IsVisibleAsync` to wait for elements dynamically. This replaced fixed delays (`Thread.Sleep`), making the automation more reliable and faster.
3. Managing random user assignment and unique asset naming to avoid duplication on repeated test runs.
   - For managing random user assignment and unique asset naming, I generated random selections from available user lists and appended unique identifiers to asset names. This prevented duplication and ensured that each test run operated on distinct data, maintaining test independence and data integrity.
---

## Table of Contents

- [Description](#description)
- [Challenges faced during Development](#challenges-faced-during-development)
- [Installation](#installation)
- [How to Run](#how-to-run)
- [Output Screenshot](#output-screenshot)
- [Project Structure](#project-structure)
- [Author](#author)

---

## Installation

1. **Prerequisites:**

   - [.NET 9.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)
   - [PowerShell 7.5.2](https://learn.microsoft.com/en-us/powershell/scripting/install/installing-powershell)
   - Git [(to clone the repository)](https://github.com/OhacksS/Global360Automation.git)

Install the above prerequisites tools and run the below steps in Windows Powershell.

2. **Clone the repository:**

   ```bash
   git clone https://github.com/OhacksS/Global360Automation.git
   cd Global360Automation
   ```

3. **Add Playwright package to your project**
   ```bash
   dotnet add package Microsoft.Playwright
   ```
4. **Restore dependencies:**
    ```bash
    dotnet restore
    ```

5. **Install Playwright browsers**
    ```bash
    pwsh bin/Debug/net9.0/playwright.ps1 install
    ```
---

## How to Run

Execute the following command to run the automation script:
  
 ```bash
dotnet run
```

The script will launch a Chromium browser and perform all automation steps, including login, asset creation, verification, and history validation.

---
## Output Screenshot
![Automation Script Running](./Snipeitapp-console-output.png)

---
## Project Structure
 ├── Program.cs      # All automation steps (A to E) implemented in this single file<br>
 ├── SnipeitAutomation.csproj  # Targets .NET 9.0 and uses Microsoft.Playwright v1.54.0 for browser automation.<br>
 ├── README.md       # Project documentation

--- 
## Author
Omkar Sankpal <br>
Email: omkarsankpal45@gmail.com

---



