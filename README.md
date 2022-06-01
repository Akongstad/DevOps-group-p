Workflow Status:
---

![sonar cloud](https://github.com/Akongstad/DevOps-group-p/actions/workflows/sonar%20cloud.yml/badge.svg)  
![release schedule](https://github.com/Akongstad/DevOps-group-p/actions/workflows/release_schedule.yml/badge.svg)  
![cluster-deploy](https://github.com/Akongstad/DevOps-group-p/actions/workflows/deploy-deploy.yml/badge.svg)
![codeyml](https://github.com/Akongstad/DevOps-group-p/actions/workflows/codeql.yml/badge.svg)
![.Net](https://github.com/Akongstad/DevOps-group-p/actions/workflows/dotnet.yml/badge.svg)

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

> Merge to main requires 2 approvals

### production (Deprecated. See deployment submodule instead)
The **production** branch lives on our production server and is our main branch's lean and automated counterpart.
It is developed in a manner such that our multi-container application can be rebuilt and updated on the production server to include updated Docker images and/or changes in configuration files with a single command, minimizing downtime withoud using load-balancers. 

**On production we:**

- Store configuration-files
- Pull and run docker images

> Merge to production requires 1 approvals

Environments
---
TBD-MiniTwit has a development environment for developing and testing code before it is deployed to the kubernetes cluster which has the production code.

### Development
**Prerequisites**
- Install docker: https://docs.docker.com/get-docker/

**Steps to run**

**1.** Clone the repository.
```bash
git clone https://github.com/Akongstad/DevOps-group-p.git
```
**2.** Cd into the minitwit folder: 
```bash
cd DevOps-group-p/minitwit
```
**3.** Create and polulate the .local folder: 
```bash
mkdir .local && cd .local
```
Create database password file (replace MySecretPassword with your desired password)
```bash
Touch db_password.txt && echo "MySecretPassword" > db_password.txt
```
Create database connection string file. (MySecretPassword should be replaced with the content of  db_password.txt)
```bash
touch connection_string.txt && echo "Host=db;Database=Minitwit;Username=sa;Password=MySecretPassword;" > db_password.txt
```
Create jwt key file. (MySecretKeyshould be replaced with your desired key)
```bash
touch jwt_key.txt && echo "MySecretKey" > jwt_key.txt
```
Go back to the minitwit folder
```bash
cd ..
```

**4.** Run the docker compose-file: 
  
  **a**. Run the full development system(Includes logging with elk and monitoring with prometheus/grafana)
```bash
chmod u+x setup_elk.sh

./setup_elk.sh
```
Run the application
```bash
docker-compose -f compose.dev.yaml up --build
```
  **b**. Run the base development system(frontend, backend, database)
```bash
docker-compose -f compose.test.dev.yaml up --build
```

### Production
See deployment submodule for instruction on how to deploy and update the MiniTwit-TBD cluster
