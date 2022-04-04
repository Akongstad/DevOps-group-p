# Minitwit

This is a fork of a Twitter Clone which was developed at an unknown time by unknown students at the IT University of Copenhagen (ITU).

The initial project was a full stack Flask application written in Bash and Python2 with an SQlite database.

**In this fork we refactored the project to the three following containerized microservices:**

- C# backend using the ASP.NET web framework and EF Core ORM
- ReactJS SPA frontend
- PostgreSQL database

**upon which we added monitoring and logging stacks to be served as a multi-container Docker application through a reverse proxy webserver with an auto-renewing SSL certificate.**

Branches
---
As is described in [CONTRIBUTE.md](https://github.com/Akongstad/DevOps-group-p/blob/main/CONTRIBUTE.md), our organization uses a Trunk Based Development branching model. 
Hence we have two centralized branches which are always debugged, tested and in a working state. 
Features, patches and bug-fixes are always developed on dedicated temporary branches which are deleted after being merged to its relative centralized branch.

### main 
The **main** branch lives on github and is the alpha of our centralized workflow.
While we develop features and patches on temporary branches, everything worthwhile is eventually merged to main.

**On main we:**

- Store our working source code
- Make releases
- Run workflows
- Build and push docker images

### production
The **production** branch lives on our production server is our main branch's lean and automated counterpart.
It is developed in a manner such that our multi-container application can be rebuilt and updated on the production server to include updated Docker images and/or changes in configuration files with a single command, minimizing downtime withoud using load-balancers. 

**On production we:**

- Store configuration-files
- Pull and run docker images
