# Minitwit

This is a fork of a Twitter Clone which was developed at an unknown time by unknown students at ITU.

The initial project was full stack Flask application written in Python2 and Bash.

In this fork we refactored the project to:

- C# backend using EF Core ORM and the ASP.NET web framework
- ReactJS SPA frontend
- Containerized development and production environments using Docker and Docker-Compose

### Simulation API

Is hosted at https://minitwit.online/api

|                       | GET | POST | JSON                                | Notes                            |
|-----------------------|-----|------|-------------------------------------|----------------------------------|
| /api/register         |     | x    | {"username":"","pwd":"","email":""} |                                  |
| /api/msgs             | x   |      | {"content":""}                      | Get requests require JSON field  |
| /api/msgs/<username>  | x   | x    | {"content":""}                      | Get requests require JSON field  |
| /api/fllws/<username> | x   | x    | {"follow\|\|unfollow":"<username>"} | Get requests require JSON field  |
| /api/latest           | x   |      | {}                                  | Does not work                    |



