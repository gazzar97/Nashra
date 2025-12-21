📘 Nashra – Business Requirements Document (BRD)
1. Document Information
Item	Value
Product Name	Nashra
Product Type	B2B SaaS – Sports Data API
Initial Market	Egypt
Initial Sport	Football (Soccer)
Architecture	Modular Monolith
Technology	.NET 9, Minimal APIs
Document Version	1.0
Status	MVP Definition
2. Executive Summary

Nashra is a Software-as-a-Service (SaaS) platform that provides historical sports data via APIs for businesses and developers.

The platform targets sports media, analytics companies, fantasy platforms, and betting-related services, starting with Egyptian football and expanding regionally and internationally.

The MVP focuses on read-only historical data, prioritizing accuracy, performance, and developer experience.

3. Business Objectives
Primary Objectives

Provide reliable historical football data via APIs

Enable easy and fast integration for developers

Establish a scalable foundation for future sports & regions

Prepare the platform for monetization

Success Metrics (KPIs)

API uptime ≥ 99.9%

Average response time ≤ 300ms

API consumer onboarding < 15 minutes

Data completeness ≥ 98%

4. In-Scope (MVP)
Sports Coverage

Football (Soccer)

Competitions

Egyptian Premier League

Data Types

Leagues

Seasons

Teams

Players

Matches

Match Statistics

Platform Capabilities

API authentication via API Key

Rate limiting

API versioning

OpenAPI documentation

Usage tracking

5. Out of Scope (MVP)

Live match data

Real-time updates

User-facing UI dashboard

Payments & subscriptions (foundation only)

Admin UI (CLI / internal endpoints only)

6. Stakeholders
Role	Responsibility
Product Owner	Define roadmap & priorities
Engineering	Build & maintain platform
API Consumers	Use APIs
Data Providers	Supply sports data
7. Functional Requirements
FR-01: League Data API

Retrieve leagues by country

Support pagination

Return stable identifiers

FR-02: Season Data API

Retrieve seasons by league

Support historical seasons

FR-03: Team Data API

Retrieve teams by league and season

Include metadata (name, stadium, founded year)

FR-04: Player Data API

Retrieve players by team

Support historical squads

FR-05: Match Data API

Retrieve fixtures and results

Filter by date, team, season

FR-06: Match Statistics API

Retrieve match-level statistics

Return structured, extensible stats

FR-07: API Authentication

API Key required for all endpoints

Keys must be revocable

FR-08: Rate Limiting

Limit requests per API key

Return standard rate limit headers

8. Non-Functional Requirements
Performance

Average response ≤ 300ms

Caching enabled for historical data

Security

API key validation middleware

No anonymous access

Secure storage of keys

Scalability

Modular boundaries aligned with future microservices

Horizontal scaling readiness

Reliability

Graceful error handling

Standardized error responses

9. Assumptions & Constraints
Assumptions

Historical data is immutable

Read-heavy workloads

Customers are technical users

Constraints

MVP must ship fast

No UI dependencies

Budget-conscious infrastructure

10. Risks & Mitigations
Risk	Mitigation
Data inconsistency	Validation & auditing
API abuse	Rate limiting
Scope creep
