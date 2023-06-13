# **Gizmo.Client.UI**

Web based client UI.

## We have two types of UI that can be used:

- Web browser based UI from the **Gizmo.Client.UI.Host.Web** folder
- Windows app based UI from the **Gizmo.Client.UI.Host.WPF** folder

# Prerequisites

### Make sure you have installed the following prerequisites on your development machine:

- Git (https://git-scm.com/downloads)
- .NET 6.0 SDK (https://dotnet.microsoft.com/download/dotnet/6.0)
- Node.js (https://nodejs.org/en/download/)
- npm (https://www.npmjs.com/get-npm)

# Run the project in debug mode

### Follow the steps below to run the project:

- Clone the repository using the following command `git clone https://github.com/GAMP/Gizmo.Client.UI.git -b dev --recurse-submodules`
- #### Visual Studio 2022
  - Open the solution - **Gizmo.Client.UI.sln**
  - Set the startup project to **Gizmo.Client.UI.Host.Web** or **Gizmo.Client.UI.Host.WPF**
  - Press **F5** or button to run the project
- #### Visual Studio Code
  - Install the **ms-dotnettools.csdevkit** extension
  - Select the **SOLUTION EXPLORER** tab
  - Right click on the **Gizmo.Client.UI.Host.Web** or **Gizmo.Client.UI.Host.WPF** folder and select **Debug** option
  - Choose the **Start New instance**

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
- To use JavaScript libraries add to the **Gizmo.Client.UI\src\webpack\package.json** as a npm packages in the **dependencies** section.

## Invokeing Client.UI functions with JavaScript

### Follow the steps below to invoke Client.UI functions with JavaScript:

- To invoke Client.UI functions with JavaScript, use the **'~\api.js'** file and the functions from the **ClientAPI** class.
- To use these functions, call them using the syntax **'ClientAPI.functionName()'**.

# Publishing the project

## It can be published in two ways:

### 1. Publishing the project as a Gizmo.Client skin

- Open the **'~\Gizmo.Client.UI.Host.WPF'** folder
- Open the terminal and run the follofing command `dotnet publish -c Release -o C:\client_skinname_folder`

### 2. Publishing the project as a standalone application

- Open the **'~\Gizmo.Client.UI.Host.Web'** folder
- Open the terminal and run the follofing command `dotnet publish -c Release -o C:\client_app_folder`
