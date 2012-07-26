---
title: pGina MySQL Logger Plugin Documentation
layout: default
rootPath: ../
---

pGina MySQL Logger Plugin Documentation
===================

* **Plugin Name:** MySQL Logger
* **Plugin Type:** Notification
* **Version:** 3.1.x

How it Works
----------------

The MySQL Logger plugin logs various events and/or sessions to a MySQL database.  These 
events include logon, logoff, lock, unlock, console connect/disconnect, and others.  
Along with the time of each event, the plugin also logs the host name, IP address, 
and machine name of the machine that generated the event.  

This plugin expects a specific table schema.  The table can be created using the configuration
utility (see below).

Configuration
-------------------

![MySQL Logger configuration](images/mysql_logger_config.png)

##Configuration

###Event Mode
Event mode logs individual events to the database, one event per entry. An example 
excerpt from an Event table is below:

<table class="repdb">
    <tr><th>TimeStamp</th> <th>Host</th> <th>Ip</th> <th>Machine</th> <th>Message</th></tr>
    <tr><td>2012-05-31 13:20:05</td> <td> PC-23 </td> <td> 192.168.1.123 </td> <td> PC-23 </td> <td> [0] Logon user: Bob Loblaw</td></tr>
    <tr><td>2012-05-31 13:35:35</td> <td> PC-23 </td> <td> 192.168.1.123 </td> <td> PC-23 </td> <td> [0] Session lock user: Bob Loblaw</td></tr>
    <tr><td>2012-05-31 13:37:35</td> <td> PC-28 </td> <td> 192.168.1.128 </td> <td> PC-28 </td> <td> [0] Logon user: Michael Bluth</td></tr>
    <tr><td>2012-05-31 13:45:57</td> <td> PC-23 </td> <td> 192.168.1.123 </td> <td> PC-23 </td> <td> [0] Session unlock user: Bob Loblaw</td></tr> 
    <tr><td>2012-05-31 13:57:74</td> <td> PC-23 </td> <td> 192.168.1.123 </td> <td> PC-23 </td> <td> [0] Logoff user: Bob Loblaw</td></tr>
</table>    

###Session Mode
Session mode logs each login session, allowing you to keep track of when computers
and people are logged into the computers. If you used the MySQL Logger from pGina 1.x/2.x,
this plugin replicates that behavior. 

A zero'd timestamp indicates the user has not yet logged out. In the event the system is
shut down automatically without invoking the MySQL Logger plugin (e.g. power loss), the
logout stamp will be set the next time someone logs in.  

An example excerpt from a Session table is below:

<table class="repdb">
    <tr><th>dbid</th> <th>loginstamp</th> <th>logoutstamp</th> <th>username</th> <th>machine</th> <th>ipaddress</th></tr>
    <tr><td>551</td> <td>2012-05-31 13:20:05</td> <td>2012-05-31 13:45:57</td> <td>Bob Loblaw</td> <td> PC-23 </td> <td> 192.168.1.123 </td></tr>
    <tr><td>552</td> <td>2012-05-31 13:37:35</td> <td>0000-00-00 00:00:00</td> <td>Michael Bluth</td> <td> PC-28</td> <td>192.168.1.128</td></tr>
</table>  

##Options
* **Host** -- The IP or fully-qualified hostname of the MySQL server.
* **Port** -- The port where the MySQL server process is listening.
* **Database** -- The database containing the log table.
* **Event Table** -- The table used to store events if event mode is enabled. 
* **Session Table** -- The table used to store sessions if session mode is enabled.
* **User** -- The username to use when connecting to the MySQL server.
* **Password** -- The password to use when connecting to the MySQL server.
* **Events** -- The events that will be logged if event mode is enabled.
* **Use Modified Name** -- The MySQL Logger will log the username entered by the user 
by default. If you would like to log the name after it has been modified (by the Modify 
Username or Single User Login plugins for example), check this. 

The "**Test...**" button initiates a test of the MySQL connection, and verifies that the log
table exists and is properly formatted.

The "**Create Table...**" button attempts to connect to the MySQL server and create the log table.

##Table Schema

In the event you do not wish to have the plugin create the tables for you, the
schema has been replicated below:

**Event Table**:

{% highlight sql %}
CREATE TABLE event_table (
   TimeStamp DATETIME,
   Host TINYTEXT,
   Ip VARCHAR(15),
   Machine TINYTEXT,
   Message TEXT 
)
{% endhighlight %}

**Session Table**:

{% highlight sql %}
CREATE TABLE session_table (
    dbid BIGINT NOT NULL AUTO_INCREMENT,
    loginstamp DATETIME NOT NULL,
    logoutstamp DATETIME NOT NULL,
    username TEXT NOT NULL,
    machine TEXT NOT NULL,
    ipaddress TEXT NOT NULL,
    INDEX (dbid)
)
{% endhighlight %}