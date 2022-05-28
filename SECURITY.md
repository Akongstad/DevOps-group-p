# Security

This file has been made on the occasion of the weekly assignment of week 9.

### A. Risk Identification

**1. Identify assets**
   1. the web application

**2. Identify threat sources**
   1. outdated/ vulnerable components ([OWASP](https://owasp.org/Top10/A06_2021-Vulnerable_and_Outdated_Components/ "OWASP: Vulnerable and outdated components"))
   2. Insecure Design ([OWASP - Insecure Design](https://owasp.org/Top10/A04_2021-Insecure_Design/ "OWASP: Insecure Design"))
   3. Security Misconfiguration ([OWASP - Security Misconfiguration](https://owasp.org/Top10/A05_2021-Security_Misconfiguration/ "OWASP: Security Misconfiguration"))
   4. Security logging and monitoring ([OWASP - Security Logging](https://owasp.org/Top10/A09_2021-Security_Logging_and_Monitoring_Failures/ "OWASP: Security Logging and Monitoring Failures"))
   5. Insecure passwords
   6. Active attack on server (e.g. DDoS)

**3. Construct risk scenarios**
1. (_Outdated Components_) - A component/ dependency is outdated, the outdated version may have known security issues or exploits that can make the system vulnerable
2. (_Security Misconfiguration_) -  If the server sends a detailed error log back to the user that may contain sensitive data.
3. (_Insecure Design_) - An adversary might register using a fake email or another person's email, resulting in said person not being able to register.
4. (_Logging_) - If the database contained actual personal data and not just data the simulation sent, then a scenario could be someone getting access to the database and either erasing it, 
or harvesting the data, which would be sensitive data, which would be sensitive data about users.
5. (_Insecure passwords_) - If passwords are insecure, attackers could easily get unlimited access to our application and user data.
6. (_Active attack on server_) - An active attack on the server could cause a Denial of Service scenario, resulting in loss of users and data.

### B. Risk Analysis

**1. Determine likelihood (Certain, Likely, Possible, Unlikely, Rare)**

1. Outdated Component: _Possible_
2. Security Misconfiguration: _Rare_
3. Insecure Design: _Likely_
4. Security Logging and Monitoring: _Unlikely_
5. Insecure passwords: _Rare_

**2. Determine Impact**

1. Outdated Component: _Catastrophic_
2. Security Misconfiguration: _Critical_
3. Insecure Design: _Marginal_ - a person might not be able to log in to the service if someone else already has used the given email.
4. Security Logging: _Catastrophic_ - some data might be lost. "Sensitive" data would be exposed (if the database actually contained info about real users)
5. Insecure passwords: _Critical_
6. Active attack on server: _Catastrophic_ - As mentioned above, this can resuly in loss of data and users.

**3. Use a Risk Matrix to prioritize risk of scenarios**

| Likelihood   |  Negligible | Marginal           | Critical                                           | Catastrophic     |
|--------------|-------------|--------------------|----------------------------------------------------|------------------|
| **Certain**  |             |                    |                                                    |                  |
| **Likely**   |             | Insecure Design    |                                                    |                  |
| **Possible** |             |                    |                                                    | Component        |
| **Unlikely** |             |                    |                                                    | Security Logging |
| **Rare**     |             |                    | Insecure passwords,<br/> Security Misconfiguration | Active attack    |

**4. Discuss what are you going to do about each of the scenarios**
1. _Outdated Components_: Activate `dependabot` with alerts such that we are notified as soon as a vulnerability is found in dependencies. Properly maintain source code in modular manner such that replacing and updating dependencies has marginal effect.
2. _Security Misconfiguration_: Implement layered design such that security critical microservices have no way of direct contact with users.
3. _Insecure Design_: Actively review Pull Requests and submitted code during meetings.
4. _Security Logging_: Implement layered design
5. _Insecure passwords_: Take sensible precautions (e.g. never using clear-text passwords, use randomly generated passwords, use encrypted protocols)
6. _Active attack on server_: Look into load-balancing and blacklisting. Implement a firewall preventing people from continuously pinging and performing DDOS attacks on our server.

### C. Pen-test Your System

In this part of the exercise it was required to test our own system for vulnerabilities, and fix one of these vulnerabilities. We chose to use `wmap` for this.
In the logs it was discovered that the server received requests from all around the world, e.g. Nevada. Except of acting as a DOS attack on our own system, eventually crashing the ELK stack. The pen test did not yield any system vulnerabilities
