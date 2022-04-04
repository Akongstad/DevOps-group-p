Backend
---

#### Simulation API

Is hosted at https://minitwit.online/api

|                       | GET | POST | JSON                                | Notes                            |
|-----------------------|-----|------|-------------------------------------|----------------------------------|
| /api/register         |     | x    | {"username":"","pwd":"","email":""} |                                  |
| /api/msgs             | x   |      | {"content":""}                      | Get requests require JSON field  |
| /api/msgs/<username>  | x   | x    | {"content":""}                      | Get requests require JSON field  |
| /api/fllws/<username> | x   | x    | {"follow\|\|unfollow":"<username>"} | Get requests require JSON field  |
| /api/latest           | x   |      | {}                                  | Does not work                    |

