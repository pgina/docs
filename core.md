---
layout: default
title: pGina Core Developer Documentation
---

pGina Core Developer Documentation
===================

## Contents
* [The pGina Architecture](#arch)
* [The Credential Provider](#cp)
* [The GINA](#gina)
* [The pGina Service](#service)

--------

<h2 id="arch">The pGina Architecture</h2>

pGina's architecture is based on three main components:  the credential provider
or GINA, the pGina service, and the plugins.  The image below shows the 
components and their relationship to eachother.

![pGina components](images/pgina_components.png)

The credential provider (CP) or GINA is the component that augments or replaces
the default Windows authentication functionality.  In Windows Vista or later,
this is accomplished by implementing a CP, in prior versions of Windows, we
need to create a GINA.  More details on each of these options is provided below,
suffice it to say for now that this component plugs-in to the Windows
authentication system and can configure parts of what is requested (username/
password), and what is displayed (logo, MOTD, etc.).

<h2 id="cp">The Credential Provider</h2>

TODO...

<h2 id="gina">The GINA</h2>

TODO...

<h2 id="service">The pGina Service</h2>

TODO...

