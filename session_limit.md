---
title: pGina Sesion Limit Plugin Documentation
layout: default
---

pGina Session Limit Plugin Documentation
===================

* **Plugin Name:** Session Limit
* **Plugin Type:** Notification
* **Latest Version:** 3.0.2.0

How it Works
------------------

The Session Limit plugin will automatically log off users after a specified period of time.

It works by tracking user logins (by recieving notification events).  It manages a cache of
usernames and login times.  It manages a worker thread that wakes up periodically and checks
for any users who have been logged on to the machine for a period of time greater than
the limit.  If any are found, it forces a log off for those users without warning.

Configuration 
-------------------

![Session Limit Config](images/session_limit_config.png)

* **Global time limit** -- The number of minutes before users are logged off.  If this is set
to zero, it is the same as disabling this plugin.

