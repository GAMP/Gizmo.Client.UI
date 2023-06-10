# **Gizmo.Client.UI**

Web based client UI.

# Prerequisites

### Make sure you have installed the following prerequisites on your development machine:

- Git (https://git-scm.com/downloads)
- .NET 6.0 SDK (https://dotnet.microsoft.com/download/dotnet/6.0)
- Node.js (https://nodejs.org/en/download/)
- npm (https://www.npmjs.com/get-npm)

# Run the project

### Follow the steps below to run the project:

- Clone the repository using the following command `git clone https://github.com/GAMP/Gizmo.Client.UI.git -b dev --recurse-submodules`
- #### Visual Studio 2022
  - Open the solution - **Gizmo.Client.UI.sln**
  - Set the startup project to **Gizmo.Client.UI.Host.Web** or **Gizmo.Client.UI.Host.WPF**
- #### Visual Studio Code
  - Open the Gizmo.Client.UI.Host.Web folder
  - Open the terminal and run the follofing command `dotnet watch`

# Style guide (Gizmo.Client.UI\src\scss)

## Adding a new theme

### Follow the steps below to customize the theme:

- Open the **'~\themes'** folder
- Copy the current theme folder and rename it to **custom_theme_name**
- Change the value of the **$theme-attr-name** variable in the **'~\themes\custom_theme_name\variables.scss'** file to **custom_theme_name**
- Add the **main.scss** from the **custom_theme_name** folder to the **'~\main.scss'** file as an import.
- Customize any styles in the **custom_theme_name** folder

## Adding a new style

### To add a new style without changing the theme, follow these steps:

- Use the **'~\external.scss'** file

# JavaScript guide (Gizmo.Client.UI\src\js)

## Adding a new JavaScript function

### Follow the steps below to add new JavaScript function:

- To create new JavaScript functions, use the **'~\external.js'** file and the **ExternalFunctions** class.
- To use these functions, call them using the syntax **'ExternalFunctions.functionName()'**.

## Invokeing Client.UI functions with JavaScript

### Follow the steps below to invoke Client.UI functions with JavaScript:

- To invoke Client.UI functions with JavaScript, use the **'~\api.js'** file and the functions from the **ClientAPI** class.
- To use these functions, call them using the syntax **'ClientAPI.functionName()'**.
