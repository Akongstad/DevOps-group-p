# Quality Assurance

Software Quality Definitions
---

The following definitions help us develop metrics for monitoring our software methods of analyzing logs.

#### Robustness

Our software is a system that interacts with users. 
It receives user data and responds with new data that provides value for the user.
Our software is designed to interact with specifically formatted data.

> Robustness can be defined as the predictability of our software's response to unexpected data.

#### Functionality

Our software has functionality that provides value for the user.
The quality of said functionality is the answer to a simple question:

> How well does our software provide its functionality?

#### Security

Our software handles sensitive user data.

> Security can be defined as the unlikelyhood of adversaries accessing our data.

Metrics
---

#### Robustness
Robustness metrics should give us an idea of how our application and its services are performing with respect to its input.

**Input**
- Container network traffic (requests/responses)
- Host network traffic (requests/response)

**Performance**
- Container response time
- Container memory usage
- Container CPU load
- Host memory usage
- Host CPU load

**Failures**
- Rate of container failures (Failures per week)
- Days passed since container failure


#### Functionality
- Frontend response time
- API response time
- DB query response time
- Server Error (5xx Status Codes) count
- Client Error (4xx Status Codes) count

#### Security
*Out of scope*

Monitoring
---

The following tactics are implemented to make sure our definitions of software quality are fulfilled.

#### Frontend Monitoring
- Frontend response time

#### Application Monitoring
- DB query response time
- API mean response time
- API endpoint call count
- API endpoint error count

#### Server Monitoring
- Container response time
- Container memory usage
- Container CPU load
- Host memory usage
- Host CPU load

#### Network Monitoring
- Container network traffic (requests/responses)
- Host network traffic (requests/response)
- Server Error (5xx Status Codes) count
- Client Error (4xx Status Codes) count
