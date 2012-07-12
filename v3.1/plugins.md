---
layout: default
title: pGina Plugin Development
rootPath: ../
---

Developing pGina Plugins
=========================

Tutorial: Hello pGina
------------------------

To learn how to create a pGina plugins, we'll start with a tutorial that 
demonstrates the implementation of a simple authentication plugin.
Along the way, you'll be introduced to the primary concepts and tools
behind pGina plugin development.

### Tools

Minimally, you'll need the following:

 * Visual Studio 2010 (or later)
 * .NET 4.0 framework (usually installed along with VS 2010).  

### Download the pGina source code

The first step is to download the pGina source code.  You can do so by downloading
a zip archive from [GitHub][pgina-github], or cloning the repository using 
git.  We recommend the latter because you can always update to the latest 
plugin SDK by doing a `git pull`.

### Setting up Visual Studio

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

In this example, we'll create an authentication plugin.  This plugin will successfully
authenticate any user that has "hello" in the username, and "pGina" in the 
password.

To create an authentication plugin, we need to implement the interface 
`IPluginAuthentication`.  Let's create a class in the default namespace 
for this plugin:

{% highlight csharp %}
namespace pGina.Plugin.HelloPlugin
{
    public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication
    {

    }
}
{% endhighlight %}

You'll probably want to change the name of the file to `PluginImpl.cs` to
match this class name.  

Next, we'll implement the required interface members, starting with `Name`.
This property should provide a human readable name for the plugin.

{% highlight csharp %}
public string Name
{
    get { return "Hello"; }
}
{% endhighlight %}

The `Description` property should provide a short (one sentence) description
of the plugin.

{% highlight csharp %}
public string Description
{
    get { return "Authenticates all users with 'hello' in the username and 'pGina' in the password"; }
}
{% endhighlight %}

The `Uuid` property must return a unique ID for this plugin.  You can generate
a new Guid using Visual Studio ( select "Tools" -> "Create GUID" ).

{% highlight csharp %}
private static readonly Guid m_uuid = new Guid("CED8D126-9121-4CD2-86DE-3D84E4A2625E"); 

public Guid Uuid
{
    get { return m_uuid; }
}
{% endhighlight %}

Note that the above is just an example.  You should generate a GUID and replace
the string above with that GUID.

The `Version` property should return the version number for your plugin.  The
best way to do this is to query for it using reflection.  For example:

{% highlight csharp %}
public string Version
{
    get
    {
        return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
    }
}
{% endhighlight %}

To change the version number, modify `Properties\AssemblyInfo.cs`.

Next, we implement the `Starting` and `Stopping` methods.  These are executed
at startup/shutdown of the pGina service.  They're intended for 
initialization/cleanup tasks for things that persist across logins.  Note that 
they are **not** intended
as intialization/cleanup for each login.  For our plugin, we don't need them
to do anything, so we leave them empty.

{% highlight csharp %}
public void Starting() { }

public void Stopping() { }
{% endhighlight %}

Finally, we get to the meat of our plugin, the `AuthenticateUser` method.  This
is called by the pGina service at the appropriate time during the authentication
stage of a login.  The parameter, a `SessionProperties` object contains information
about the user including the username and password.  For our plugin, we need to
simply verify that the username contains the word `hello` and that the password
contains `pGina`.  If that is the case, we return a successful result, if not we
return failure.  We return the result in a `BooleanResult` object.

{% highlight csharp %}
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
{% endhighlight %}

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

Your plugin should log information about its progress and activites.  Logging
support is provided via [Apache log4net][log4net]. Adding logging to a plugin
is simple.  The first step is to create a logger object.  You can do this in the
`Starting` method, or the constructor.  We recommend that you do not statically
initialize this object.  For example, to initialize your logger in the constructor,
use the following code:

{% highlight csharp %}
private ILog m_logger;

public PluginImpl()
{
    m_logger = LogManager.GetLogger("pGina.Plugin.HelloPlugin");
}
{% endhighlight %}

To log messages using the logger, you can use any of the standard [log4net][log4net]
logging functions.  For example:

{% highlight csharp %}
if (userInfo.Username.Contains("hello") && userInfo.Password.Contains("pGina"))
{
    // Successful authentication
    m_logger.InfoFormat("Successfully authenticated {0}", userInfo.Username);
    return new BooleanResult() { Success = true };
}
// Authentication failure
m_logger.ErrorFormat("Authentication failed for {0}", userInfo.Username);
return new BooleanResult() { Success = false, Message = "Incorrect username or password." };
{% endhighlight %}
        
For more about log4net, vist the [log4net web site][log4net].

Storing Plugin Settings in the Registry
---------------------------------------

If your plugin requires configurable settings, they should be stored in the
registry, as a sub-key of the main pGina registry key.  Support for this is
provided via the `pGinaDynamicSettings` class.  This class utilizes the C#
`DynamicObject` class and the `dynamic` type.  Settings can be queried and
set as if they are properties of the object.  

To use `pGinaDynamicSettings` we recommend that you instantiate the object and
immediately set the defaults for all of your settings.  It makes sense to do
this in a static initializer.  For example:

{% highlight csharp %}
private static dynamic m_settings;
internal static dynamic Settings { get { return m_settings; } }

static PluginImpl()
{
    m_settings = new pGina.Shared.Settings.pGinaDynamicSettings(m_uuid);

    m_settings.SetDefault("Foo", "Bar");
    m_settings.SetDefault("DoSomething", true);
    m_settings.SetDefault("ListOfStuff", new string[] { "a", "b", "c" });
    m_settings.SetDefault("Size", 1);
}
{% endhighlight %}

The `SetDefault` method will initalize a setting in the registry if it does not
already exist.  If the registry setting exists, the method has no effect.  Be
sure to instantiate the `pGinaDynamicSettings` object using the `Guid` of your
plugin.  This will ensure that your settings are stored in the approprate 
registry key (usually: `HKLM\SOFTWARE\pGina3\Plugins\{guid}` where `{guid}` is 
replaced with the Guid of your plugin).

The supported data types for settings are `string`, `bool`, `string[]`, and
`int`.  It is highly recommended that you set defaults as soon as your object
is created.  This will avoid runtime exceptions when trying to access a 
non-existent registry value.

To set/read the settings, you simply treat them as properties of the object.
For example:

{% highlight csharp %}
bool okToGoAhead = Settings.DoSomething;
if (okToGoAhead)
{
    Settings.Foo = "Baz";
}
{% endhighlight %}

Note that when reading a property it must be able to determine the data type
at run-time.  If you try to read the setting in a context that is ambiguous,
you may recieve a run-time exception.  Your best bet is to assign the setting
to a local variable with the appropriate data type (as shown above).

Creating a Plugin Configuration Dialog
--------------------------------------

To provide a dialog to the user for configuration of your plugin via the 
pGina configuration UI, you implement the `IPluginConfiguration` interface.

{% highlight csharp %}
public class PluginImpl : pGina.Shared.Interfaces.IPluginAuthentication,
    pGina.Shared.Interfaces.IPluginConfiguration
{% endhighlight %}

This requires you to implement the method `Configure`.  This method should
initalize and display your dialog, and will be called by the pGina configuration
UI when the user requests to configure your plugin.  

Create a Windows form (use "Project" -> "Add windows form..."), and set up
your dialog.  Then make sure to invoke your windows form within the `Configure`
method.  For example, if my Windows form was called `Configuration`, I'd use
the following code:

{% highlight csharp %}
public void Configure()
{
    Configuration myDialog = new Configuration();
    myDialog.ShowDialog();
}
{% endhighlight %}

Authorization and Gateway Plugins
---------------------------------

To have your plugin support the authorization stage, implement the
`IPluginAuthorization` interface.  This requires the implementation of the following
method:

{% highlight csharp %}
BooleanResult AuthorizeUser(SessionProperties properties) { ... }
{% endhighlight %}

This method should return a `BooleanResult` with `Success` set to `true` if
the user is authorized by this plugin, otherwise `Success` should be set to
`false` and an appropriate message provided in the `Message` property.

To support the gateway stage, implement the `IPluginAuthenticationGateway`
interface.  This requires you to implement the following method:

{% highlight csharp %}
BooleanResult AuthenticatedUserGateway(SessionProperties properties) { ... }
{% endhighlight %}

The gateway stage is intended for any last minute post-authorization actions that
may be necessary.  For example, a user's group membership might be modified
(e.g. LDAP plugin), or
the user's username might be modified (e.g. the single user plugin).  Generally,
this stage should not fail, except under exceptional circumstances.  You
should almost always return a `BooleanResult` with `Success` set to `true` unless
for some reason the login should be denied.  Usually in the gateway stage, the
login should not be denied.  The only situation that might warrant a failure for
this stage is if an error of some kind occurs, however, even in that
situation, it often makes sense to log the error and return a successful result.

`Session Properties`
--------------------------

The `SessionProperties` object is provided as a parameter to each of the three
methods corresponding to the three stages (authentication, authorization, and
gateway).  The most obvious use of this is to query the user information (by
retrieving the `UserInformation` object), such
as username, password, and group membership.  However, it is actually a general
purpose storage object, and can be used to store
any information that a plugin may need to persist across stages.  In fact, it
is recommended that if you have any persistent state that needs to be passed
between stages, you should use this object rather than using instance fields.

You can store objects in the `SessionProperties` object using the provided 
methods listed below.

{% highlight csharp %}
public void AddTracked<T>(string name, T val) { ... }
public void AddTrackedSingle<T>(T val) { ... }

public T GetTracked<T>(string name) { ... }
public T GetTrackedSingle<T>() { ... }
{% endhighlight %}

You can store an object associated with a key (a `string`) using `AddTracked`, or
you can store a single instance of a class using `AddTrackedSingle`.  Of course,
your plugin should not add a tracked single of the class `UserInformation`, because
that would clobber the `UserInformation` object that is provided by pGina core.

A unique `SessionProperties` object will be provided for each login.  If your plugin
is only involved in a single stage, then there is no need to store anything in
this object.  However, if your plugin is involved in multiple stages, then it
makes sense to store any persistent state related to a given login within this
object.  

Note that if you need to make a connection to a remote data source and you'd like
that connection to persiste between stages, you should make use of the `SessionProperties`
object along with the `IStatefulPlugin` interface (see below).

### Getting Information about Plugin Activity

It is often the case that a plugin needs to know what other plugins have executed
previously in the login chain, and the result of those plugins.  This information
is stored in the `SessionProperties` object and is in a tracked single of type
`PluginActivityInformation`.   You can query for the result of a given plugin
via the methods `GetAuthenticationResult`, `GetAuthorizationResult`, or 
`GetGatewayResult`.   However, use caution because if a plugin has not executed 
yet, these will throw an exeception.  To be safe, you should first use 
`GetAuthenticationPlugins`, `GetAuthorizationPlugins`, or `GetGatewayPlugins`
to get a list of plugins that have executed and iterate through the list.

For example, to count the number of successful authentications in the 
authentication stage so far, you could use the following code:

{% highlight csharp %}
PluginActivityInformation pluginInfo = sessionProps.GetTrackedSingle<PluginActivityInformation>();

int nSuccess = 0;
foreach( Guid pluginId in pluginInfo.GetAuthenticationPlugins() )
{
    BooleanResult result = pluginInfo.GetAuthenticationResult( pluginId );
    if( result.Success )
        nSuccess++;
}

// nSuccess has the number of plugins that have registered success in the authentication
// stage so far.
{% endhighlight %}


The `IStatefulPlugin` Interface
--------------------------------

If your plugin has state that needs to persist between stages of a
login, and/or makes connections to resources that need to be relased at the
end of a login chain (such as making a connection to a remote data source), you 
should implement the `IStatefulPlugin` interface.  This interface requires
two methods:

{% highlight csharp %}
void BeginChain(SessionProperties props) { ... }
void EndChain(SessionProperties props) { ... }
{% endhighlight %}

`BeginChain` is called prior to the authentication stage and should be used 
for initialization and set-up.  You should store any state in the provided
`SessionProperties` object (see above).  For example, one might initialize a connection
to a remote data source here and store a reference to the connection within
the `SessionProperties` object.

`EndChain` is called at the end of a login chain regardless of the success or
failure of the login.  This should be used to clean up any resources that 
are held by the plugin.  For example, one might terminate the connection with
a remote data source here.

Notification Plugins
--------------------------

To implement a notification plugin, you should implement the `IPluginEventNotifications`
interface.  This interface requires the following method:

{% highlight csharp %}
void SessionChange(SessionChangeDescription changeDescription, SessionProperties properties) { ... }
{% endhighlight %}

This method is called when any of the standard Windows terminal events occurs.  The
first parameter provides a description of the event.  This class is 
[documented in the MSDN documentation][sessionChangeMsdn], and provides two
main properties: `Reason` and `SessionId`.  Of primary importance is the `Reason`
property.  The [MSDN documentation][changeReasonMsdn] describes the possible
values for this property.  Based on the value of the `Reason` property, you can
take the action that is appropriate for your plugin.

[pgina-github]: https://github.com/pgina/pgina "pGina repo on GitHub"
[example-code]: ExamplePluginImpl.cs "Example plugin source code."
[log4net]: http://logging.apache.org/log4net/ "Log4Net web site"
[sessionChangeMsdn]: http://msdn.microsoft.com/en-us/library/system.serviceprocess.sessionchangedescription.aspx "MSDN OnSessionChange"
[changeReasonMsdn]: http://msdn.microsoft.com/en-us/library/system.serviceprocess.sessionchangedescription.reason.aspx "MSDN SessionChangeReason"
