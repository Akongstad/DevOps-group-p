# Microservices

Frontend
---

> ReactJS Single Page Application initialized with Create React App.

Backend
---

The backend service consists of two controllers, each with their own exposed endpoint.

> RESTful web API built with ASP.NET and EF Core ORM

### API
The first controller is dedicated to an ongoing simulation which continuously sends generated user data to our application.
The controller requires no authentication for sending user data and is therefore a vulnerability.
Hence measures should be taken to make sure it is not exploited.

It's hosted at

> https://minitwit.online/api/

and its specified endpoints are the following:

|                       | GET | POST | JSON                                | Notes                            |
|-----------------------|-----|------|-------------------------------------|----------------------------------|
| /api/register         |     | x    | {"username":"","pwd":"","email":""} |                                  |
| /api/msgs             | x   |      | {"content":""}                      | Get requests require JSON field  |
| /api/msgs/<username>  | x   | x    | {"content":""}                      | Get requests require JSON field  |
| /api/fllws/<username> | x   | x    | {"follow\|\|unfollow":"<username>"} | Get requests require JSON field  |
| /api/latest           | x   |      | {}                                  | Does not work                    |

### APIV2
The second controller is dedicated to all implemented functionality for the frontend.
It's the main implementation of our application and bears the full weight of our demands of this multi-container application.

> https://minitwit.online/apiv2/

and its specified endpoints are the following

|                       | GET | POST | JSON                                | Notes                            |
|-----------------------|-----|------|-------------------------------------|----------------------------------|
| /api/register         |     | x    | {"username":"","pwd":"","email":""} |                                  |
| /api/msgs             | x   |      | {"content":""}                      | Get requests require JSON field  |
| /api/msgs/<username>  | x   | x    | {"content":""}                      | Get requests require JSON field  |
| /api/fllws/<username> | x   | x    | {"follow\|\|unfollow":"<username>"} | Get requests require JSON field  |
| /api/latest           | x   |      | {}                                  | Does not work                    |

Prometheus
---

Grafana
---

Elasticsearch
---

Filebeat
---

Kibana
---

Db
---

Letsencrypt
---

