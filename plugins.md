---
layout: default
title: pGina Plugin Development
---

Developing pGina Plugins
=========================

Tutorial: Hello pGina
------------------------

To learn how to create a pGina plugin, we'll start with a simple example.
We'll develop a simple
pGina plugin that authenticates all users that have a username that includes
the word `hello` and has `pGina` in the password.  This is 
intended to introduce you to the concepts and tools behind pGina plugin 
development.

### Tools

Minimally, you'll need the following:

 * Visual Studio 2010 (or later)
 * .NET 4.0 framework (usually installed along with VS 2010).  

### Download the pGina source code

The first step is to download the pGina source code.  You can do so by downloading
a zip archive from [GitHub][pgina-github], or cloning the repository using 
git.  We recommend the latter because you can always update to the latest 
plugin SDK by doing a `git pull`.

### Creating your Visual Studio solution

Open Visual Studio and create a new project.  In the new project dialog, select
the "Class Library" template under "Visual C#" -> "Windows".  Make sure to select
".NET Framework 4" in the drop-down list at the top.  Select the `Plugins` directory
of the pGina distribution as the location, and make sure to select "Create new
solution."  The name and solution name should be a short version of the name
of your plugin without any spaces.  For this example, we'll use the name 
`HelloPlugin`.

This will create a solution with a single project and a simple C# file.  Before
we jump into the code, let's configure the build settings.  Select
"Project" -> "HelloPlugin Properties...".

It makes things easiest if you set your build directory to a common location for all
plugins.  Currently, all plugins build to `Plugins\bin`.  To update your build
settings so that the output directory is set to this directory, click on the "Build" tab,
select "All Configurations" from the "Configuration" list, and set "Output path" to
`..\..\bin`.

We like to have all plugins use a consistent naming scheme for the output file
names.  This is someting like the following: `pGina.Plugin.PluginName`.  Select
the "Application" tab, and set the "assembly name" to `pGina.Plugin.HelloPlugin`.
We should also use an isolated namespace for our plugin, so under "default 
namespace", use `pGina.Plugin.HelloPlugin`.

Save and do a quick build ("Build"->"Build Solution").  Verify that your plugin's
`dll` appears in the `Plugins\bin` directory.  You should see 
`pGina.Plugin.HelloPlugin.dll`.

Next, we need to add references to the pGina SDK dll's and the log4net dll.  
Select "Project" -> "Add reference...".  Select the "Browse" tab, and browse
to `Plugins\SDK`, and select `pGina.Shared.dll`, `Abstractions.dll`,
and `log4net.dll` (`pGina.Core.dll` is not necessary for plugin development).

You're now ready to start developing your plugin!

### Implementing the Plugin

In this example, we'll create an authentication plugin.  We'll start with
a class in the default namespace for this plugin:

    namespace pGina.Plugin.HelloPlugin
    {
        public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication
        {

        }
    }

You'll probably want to change the name of the file to `PluginImpl.cs` to
match this class name.  

Next, we'll implement the required interface members, starting with `Name`.
This property should provide a human readable name for the plugin.

    public string Name
    {
        get { return "Hello"; }
    }

The `Description` property should provide a short (one sentence) description
of the plugin.

    public string Description
    {
        get { return "Authenticates all users with 'hello' in the username and 'pGina' in the password"; }
    }

The `Uuid` property must return a unique ID for this plugin.  You can generate
a new Guid using Visual Studio ( select "Tools" -> "Create GUID" ).

    public Guid Uuid
    {
        get { return new Guid("CED8D126-9121-4CD2-86DE-3D84E4A2625E"); }
    }

Note that the above is just an example.  You should generate a GUID and replace
the string above with that GUID.

The `Version` property should return the version number for your plugin.  The
best way to do this is to query for it using reflection.  For example:

    public string Version
    {
        get
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

To change the version number, modify `Properties\AssemblyInfo.cs`.

Next, we implement the `Starting` and `Stopping` methods.  These are executed
at startup/shutdown of the pGina service.  They're intended for 
initialization/cleanup tasks for things that persist across logins.  Note that 
they are **not** intended
as intialization/cleanup for each login.  For our plugin, we don't need them
to do anything, so we leave them empty.

    public void Starting() { }
    
    public void Stopping() { }

Finally, we get to the meat of our plugin, the `AuthenticateUser` method.  This
is called by the pGina service at the appropriate time during the authentication
stage of a login.  The parameter, a `SessionProperties` object contains information
about the user including the username and password.  For our plugin, we need to
simply verify that the username contains the word `hello` and that the password
is not empty.  If that is the case, we return a successful result, if not we
return failure.  We return the result in a `BooleanResult` object.

    public BooleanResult AuthenticateUser(SessionProperties properties)
    {
        UserInformation userInfo = properties.GetTrackedSingle<UserInformation>();

        if (userInfo.Username.Contains("hello") && userInfo.Password.Contains("pGina"))
        {
            // Successful authentication
            return new BooleanResult() { Success = true };
        }
        // Authentication failure
        return new BooleanResult() { Success = false, Message = "Incorrect username or password." };
    }

The `BooleanResult` object contains two properties: `Success` and `Message`.
You do not always need to set the `Message` property, but you always want to
set the `Success` property.   We recommend that you always set the `Message` 
property when the authentication fails.

That's it!  You've implemented a simple pGina plugin.  The full source for this
plugin can be downloaded from this link [ExamplePluginImpl.cs][example-code].

### Testing your plugin

Execute the pGina configuration utility, under the "Plugin Configuration" tab, 
make sure to add the plugin build directory in the pGina distribution (`Plugins\bin`),
and enable the plugin by checking the checkbox for the authentication stage.
Then, under the "Simulation" tab, test your plugin by trying out a few logins.

There's much more to learn about plugins, but this should give you a starting
point.  In the sections below, we'll dive into some more advanced plugin concepts.

Adding Logging to Your Plugin
---------------------------------------

Coming soon...

Storing Plugin Settings in the Registry
---------------------------------------

Coming soon...

Creating a Plugin Configuration Dialog
--------------------------------------

Coming soon...

Authorization and Gateway Plugins
---------------------------------

Coming soon...

[pgina-github]: https://github.com/pgina/pgina "pGina repo on GitHub"
[example-code]: ExamplePluginImpl.cs "Example plugin source code."
