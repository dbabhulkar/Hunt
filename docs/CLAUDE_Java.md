# CLAUDE.md

## Project Overview
Enterprise Java application built using Spring Boot 3.5 and Hibernate 6.6.

Primary goals:
- Maintain clean layered architecture
- Follow secure coding practices
- Preserve backward compatibility
- Support modernization and migration from legacy Hibernate implementations

---

# Tech Stack

## Backend
- Java 21
- Spring Boot 3.x
- Hibernate 6.6
- Spring Data JPA
- Maven

## Database
- MySQL 8.x
- Flyway for schema migrations

## APIs
- REST APIs using Spring MVC
- OpenAPI / Swagger

## Security
- Spring Security
- JWT authentication
- OWASP secure coding controls

## Testing
- JUnit 5
- Mockito
- Testcontainers
- Spring Boot Test

---

# Architecture Rules

Use layered architecture:

Controller
Service
Repository
Entity
DTO
Mapper

Rules:
- Controllers contain no business logic
- Business rules belong only in services
- Database logic belongs in repositories
- Never expose entities directly from APIs
- Use DTOs for request/response
- Use constructor injection only
- Avoid field injection

---

# Hibernate 6.6 Standards

Use Jakarta packages:

import jakarta.persistence.*;

Never use deprecated Hibernate 4/5 APIs:
- Do not use createCriteria()
- Do not use org.hibernate.Criteria
- Do not use legacy Query APIs
- Replace with:
  - JPA Criteria API
  - TypedQuery
  - HQL compatible with Hibernate 6.6

Use:
- SessionFactory.getCurrentSession()
- Typed queries
- Fetch joins to avoid N+1

Entity Rules:
- Use LAZY loading by default
- Explicitly define fetch strategy
- Use @Version for optimistic locking
- Use proper cascade types
- Avoid EAGER unless required

Deletion Rules:
- Prefer soft delete when business requires retention
- Validate child relationships before delete
- Use transactional delete methods

---

# Coding Standards

Follow:
- SOLID principles
- DRY
- Clean Code conventions

Naming:
- Classes: PascalCase
- Methods: camelCase
- Constants: UPPER_CASE

Packages:
com.company.project
  controller
  service
  repository
  entity
  dto

Rules:
- Keep methods under 40 lines where possible
- Extract reusable logic into helper services
- Do not duplicate repository logic
- Add JavaDoc for public methods

Logging:
Use SLF4J

log.info()
log.warn()
log.error()

Never use:
System.out.println()

---

# Security Rules

Follow OWASP guidelines.

Always:
- Use prepared parameters (never concatenate SQL)
- Validate all inputs
- Sanitize request payloads
- Use @Valid on DTOs
- Protect against:
  - SQL Injection
  - XSS
  - CSRF
  - Broken authentication

Never:
- Hardcode secrets
- Store credentials in source code
- Log passwords, tokens, PII

Secrets:
Use:
- Environment variables
- Vault / Secret Manager

Authorization:
Use method security:
@PreAuthorize

---

# Testing Standards

Minimum coverage:
- Services: Unit tests required
- Repositories: Integration tests required
- Controllers: API tests required

Use:
- JUnit 5
- Mockito
- Testcontainers for DB tests

Test naming:

shouldCreateClaim()
shouldDeleteClaimWhenValid()
shouldThrowExceptionWhenClaimMissing()

---

# Build Commands

## Clean Build
mvn clean install

## Run Tests
mvn test

## Integration Tests
mvn verify

## Run Application
mvn spring-boot:run

## Package
mvn clean package

---

# Deployment Commands

## Build Artifact
mvn clean package -DskipTests

## Docker Build
docker build -t claims-service .

## Run Container
docker run -p 8080:8080 claims-service

## Kubernetes Deploy
kubectl apply -f k8s/

---

# Database Commands

## Run Flyway Migration
mvn flyway:migrate

## Validate Schema
mvn flyway:validate

---

# Refactoring Guidance for Claude

When generating code:
- Preserve existing business logic
- Minimize unnecessary changes
- Prefer incremental refactoring
- Show compatibility risks before changing APIs
- For Hibernate migration:
  - Identify deprecated APIs
  - Suggest Hibernate 6.6 replacement
  - Update query syntax if required
  - Verify transaction handling

Before submitting code:
- Check compilation
- Check tests pass
- Check security impacts
- Check performance impacts

---

# Pull Request Checklist

Before PR:
- Tests pass
- No deprecated Hibernate APIs
- No security violations
- No hardcoded secrets
- Migrations included if schema changed
- Documentation updated

---

# Claude Commands

Use these prompts internally when assisting:

Examples:

"Refactor repository code for Hibernate 6.6 compliance"

"Generate JPA Criteria replacement for deprecated Hibernate Criteria"

"Review this service for OWASP security issues"

"Add unit tests for this Spring service"

"Check this query for Hibernate 6 compatibility"
