---
title: pGina Local Machine Plugin Documentation
layout: default
---

pGina Local Machine Plugin Documentation
===================

* **Plugin Name:** Local Machine
* **Plugin Type:** Authentication, Authorization, Gateway
* **Latest Version:** 3.0.2.0

How it Works
--------------------

The local machine plugin manages authentication and authorization for accounts
that exist on the machine itself.  It also is responsible for creating local
accounts (possibly temporary ones) when a user is authorized to log in, but
does not have a local account.  Much of the functionality provided by this
plugin was formerly part of the core in pGina 2.x and earlier.

The local machine plugin can execute in any or all of the three main pGina stages
(authentication, authorization, and gateway).

### Authentication Stage

In the authentication stage, the local machine plugin attempts to authenticate
the user's credentials against an existing local account.  If the local user 
account does not exist, or the credentials do not match, the
plugin registers failure for this stage.

It can be configured to always attempt to authenticate, or to only do so 
if the user has not already been authenticated by a plugin that was 
executed earlier within this stage.

Note that you probably always want to make sure that the local machine plugin
is enabled in the authentication stage.  If not, you risk being unable to
log into the machine if for some reason the alternate authentication methods
fail (such as a network issue).

### Authorization Stage

The local machine plugin authorizes users based on group membership.  It can be 
configured such that a user must be a member of the administrator group to be
authorized, and/or the user must be a member of one of a set of other local
groups.

The plugin can also be configured to only apply these rules to accounts that
were authenticated by this plugin and not by others.  Or alternatively, it can
apply these authorization rules to all authenticated users.

### Gateway Stage

If enabled in the gateway stage, the local machine plugin ensures that the
authenticated (and authorized) user has a local account.  If not, one is created.
It also makes sure that the local account has the appropriate group membership.
You can configure the plugin to add the user to a set of mandantory groups.

We recommend that you have this plugin enabled in the gateway stage if you are using
non-local account logins such as LDAP or MySQL Authentication.

The plugin can also be configured such that the local account should be scheduled
for removal or have it's password scrambled upon logoff.  This is implemented in the
following way.  Upon successful completion of this stage, it records the username
as a successful login.  A background thread wakes up every so often to 
check if the user has logged off.  If so, the plugin tries to delete the 
account/profile and/or scramble the password.

Configuration
--------------------

![LocalMachine Plugin Configuration](images/local_machine_config.png)

* **Always authenticate local users** -- When this is checked, the plugin will always
attempt to authenticate the user against a local account.  If this is not checked, 
the plugin will only attempt to authenticate when the user has not already been
authenticated by a plugin that has executed earlier within the authentication stage.
* **Mirror groups from local user** -- Load all groups from the local account into
pGina so that subsequent stages will see that the user is a member of those groups.
* **Authorize all authenticated users** -- When this is checked, the plugin will 
attempt to authorize all users.  Otherwise, the plugin will only authorize
users that were authenticated successfully by this plugin (and not any other plugin).
* **Require local administrator group membership** -- Only authorize users that are
members of the administrator group.
* **Require membership in one of the following local groups** -- Only authorize users
that are members of one of the groups listed below this checkbox.
* **Failure to create or join local groups should prevent login** -- When this is 
checked, the plugin will register failure if it is unable to create or join the
local groups that are requested.
* **Scramble password after logout** -- When this is checked, the plugin will attempt
to scramble the password of the local account after logout.
* **Remove account and profile after logout** -- When this is selected, the plugin
will attempt to remove the account and its profile after logout.
* **Mandantory groups** -- The local account is added to these local groups if not
already a member.