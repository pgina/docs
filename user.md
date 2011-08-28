---
title: pGina User's Guide
layout: default
---

pGina User's Guide
===================

## Contents
* [Installation](#install)
* [How pGina Works](#howworks)
* [Selecting and Configuring Plugins](#selecting)
* [Ordering Plugins](#ordering)
* [Testing Plugins](#testing)

--------

<h2 id="install">Installation</h2>

TODO...

<h2 id="howworks">How pGina Works</h2>

pGina is a replacement for the default Windows credential provider (the part
of the system that manages logins).  Through plugins, pGina allows you to
configure many aspects of the login process from authentication and authorization
through to logging, drive mapping, and other login-time actions.

pGina manages the Windows login process by delegating the work to a set of zero or more
plugins.  Plugins have the job of deciding whether or not the user is who she
says she is (authentication), whether the user should be granted access 
(authorization), and taking other login-time actions such as logging, drive
mapping, or running scripts.  The process involves five main stages as illustrated
in the following diagram.

![pGina stages](images/pgina_stages.png)

After the user provides his/her credentials, pGina processes the above five
stages one at a time, in the order shown above.  The first three stages may
succeed or fail depending on the results of the plugins involved.  The last
two stages always succeed because they merely provide services and do not play
a part in determining whether a user is allowed access.  Each stage provides
a specific service as summarized below.

* __Authentication__ - Plugins involved in this stage validate that the user is
  who he/she says she is.  This might be done by validating the credentials against
  some external database or other source.
* __Authorization__ - This stage is intended to determine whether or not the 
  user (who is already authenticated) is allowed to access the resources that
  are being requested.  For example, a user might only be allowed to login if 
  they are a member of certain groups.
* __Gateway__ - This is similar to the Authorization stage in that it may fail,
  however, the intention is not to authorize users, but to provide post-authorization
  account management that may fail.  For example... *provide an example here*
* __System Session Helper__ - A plugin involved in this stage can perform
  arbitrary processing in the user's session.  The plugin(s) are executed in
  the context of the user's session as the service account that is running
  the pGina service.  Non-admin users cannot stop this helper (admin users
  can).
* __User Session Helper__ - A plugin involved in this stage can perform 
  arbitrary processing in the user's session.  The plugin(s) are executed in
  the context of the user's session as the user account.  Users can stop
  the helper.
  
Plugins can register to provide services for one or more of the above five stages.
They can also register to recieve notifications as various events occur.  For
example, a plugin that authenticates a user via a MySQL database as well as logs
information about the login to another database might provide
authentication services and provide a user session helper to do the logging.
Some plugins might only provide helpers without doing any validation, such as 
the ScriptRunner plugin which only provides a system session helper and a 
user session helper.

There can be zero or more plugins involved with each stage.  If any stage fails,
the login fails.  However, stages have different rules regarding when a stage
succeeds or fails.  

* __Authentication__ - At least one of the plugins involved in this stage must register
  success for the process to continue.
* __Authorization__ - All plugins involved in this stage must register success
  for the process to continue.
* __Gateway__ - All plugins involved in this stage must register success for 
 the process to continue.
* __System Session Helpers__ - Always succeeds.
* __User Session Helpers__ - Always succeeds.

It is of course possible that there can be more than one plugin involved in
each stage...

TODO... Talk about selecting plugins for stages (not how-to but the concept).

TODO... Talk about ordering (not how-to)

<h2 id="selecting">Selecting and Configuring Plugins</h2>

TODO...

<h2 id="ordering">Ordering Plugins</h2>

TODO...

<h2 id="testing">Testing Plugins</h2>

TODO...

