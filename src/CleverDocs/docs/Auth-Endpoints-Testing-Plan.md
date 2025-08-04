# Authentication Endpoints Testing Plan

## Overview
This document outlines a comprehensive testing strategy for the `LoginV1Endpoint` and `RegisterV1Endpoint` authentication endpoints in the CleverDocs API.

## Endpoints Under Test

### 1. LoginV1Endpoint (`POST /login`)
- **Location**: `CleverDocs.Api/Auth/Endpoints/LoginV1Endpoint.cs:25`
- **Purpose**: Authenticate existing users and return JWT tokens
- **Input**: `LoginUserDto` (Email, Password)
- **Output**: `AccessTokensDto` or error responses

### 2. RegisterV1Endpoint (`POST /register`)
- **Location**: `CleverDocs.Api/Auth/Endpoints/RegisterV1Endpoint.cs:26`
- **Purpose**: Create new user accounts and return JWT tokens
- **Input**: `RegisterUserDto` (Email, FirstName, LastName, Password, ConfirmPassword)
- **Output**: `AccessTokensDto` or error responses

## Test Categories

### A. Unit Tests

#### 1. Validation Tests

**LoginV1Endpoint Validation Tests:**
- **Valid Login Data**
  - Email: valid format, under 255 characters
  - Password: 6-100 characters
  - Expected: Validation passes

- **Invalid Email Tests**
  - Empty email → ValidationProblem (400)
  - Invalid email format → ValidationProblem (400)
  - Email over 255 characters → ValidationProblem (400)

- **Invalid Password Tests**
  - Empty password → ValidationProblem (400)
  - Password under 6 characters → ValidationProblem (400)
  - Password over 100 characters → ValidationProblem (400)

**RegisterV1Endpoint Validation Tests:**
- **Valid Registration Data**
  - All fields populated correctly
  - Passwords match
  - Expected: Validation passes

- **Invalid Email Tests** (same as login)

- **Invalid Name Tests**
  - Empty FirstName → ValidationProblem (400)
  - Empty LastName → ValidationProblem (400)
  - FirstName under 2 characters → ValidationProblem (400)
  - LastName under 2 characters → ValidationProblem (400)
  - FirstName over 100 characters → ValidationProblem (400)
  - LastName over 100 characters → ValidationProblem (400)

- **Invalid Password Tests** (same as login)

- **Password Confirmation Tests**
  - Empty ConfirmPassword → ValidationProblem (400)
  - Passwords don't match → ValidationProblem (400)

#### 2. Business Logic Tests

**LoginV1Endpoint Business Logic:**
- **Email Existence Check**
  - Non-existent email → Problem (409) "A user with this email address does not exist"
  - Existing email → Proceed to authentication

- **Authentication Results**
  - Valid credentials → Ok (200) with AccessTokensDto
  - Invalid credentials with Validation error → ValidationProblem (400)
  - Invalid credentials with Server error → Problem (500)

**RegisterV1Endpoint Business Logic:**
- **Email Uniqueness Check**
  - Existing email → Problem (409) "A user with this email address already exists"
  - New email → Proceed to registration

- **Registration Results**
  - Valid data → Ok (200) with AccessTokensDto
  - Registration failure with Validation error → ValidationProblem (400)
  - Registration failure with Server error → Problem (500)

### B. Integration Tests

#### 1. End-to-End API Tests

**Setup Requirements:**
- Test database with clean state
- Configured authentication services
- Valid JWT configuration

**Login Integration Tests:**
1. **Successful Login Flow**
   - Create test user in database
   - POST valid credentials to /login
   - Verify 200 response with valid AccessTokensDto
   - Verify JWT tokens are valid and contain correct claims

2. **Failed Login Scenarios**
   - POST with non-existent email → 409 response
   - POST with wrong password → appropriate error response
   - POST with invalid data → 400 validation response

**Registration Integration Tests:**
1. **Successful Registration Flow**
   - POST valid registration data to /register
   - Verify 200 response with valid AccessTokensDto
   - Verify user is created in database
   - Verify JWT tokens are valid and contain correct claims

2. **Failed Registration Scenarios**
   - POST with existing email → 409 response
   - POST with invalid data → 400 validation response
   - POST with database error conditions → 500 response

#### 2. Database Integration Tests
- Verify user creation in database during registration
- Verify password hashing and storage
- Verify user lookup during login
- Test transaction rollback on failures

### C. Security Tests

#### 1. Authentication Security
- **Password Security**
  - Verify passwords are hashed (never stored in plain text)
  - Test password complexity requirements
  - Verify password confirmation validation

- **JWT Token Security**
  - Verify tokens contain appropriate claims
  - Test token expiration
  - Verify token signature validation
  - Test refresh token functionality

#### 2. Input Validation Security
- **SQL Injection Prevention**
  - Test malicious SQL in email and password fields
  - Verify parameterized queries usage

- **XSS Prevention**
  - Test script injection in name fields
  - Verify proper encoding of user input

- **Rate Limiting** (if implemented)
  - Test multiple failed login attempts
  - Test registration attempt limits

### D. Performance Tests

#### 1. Load Testing
- Test endpoint performance under normal load
- Test database connection handling
- Test JWT token generation performance

#### 2. Stress Testing
- Test behavior under high concurrent requests
- Test memory usage during load
- Test database connection pool limits

## Test Data Requirements

### Valid Test Data
```json
// Login
{
  "email": "test@example.com",
  "password": "SecurePass123"
}

// Registration
{
  "email": "newuser@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "password": "SecurePass123",
  "confirmPassword": "SecurePass123"
}
```

### Invalid Test Data Sets
- **Boundary Testing**: Min/max length values
- **Format Testing**: Invalid email formats, special characters
- **Security Testing**: Injection attempts, malicious payloads

## Test Environment Setup

### Dependencies to Mock/Stub
- `IAuthService` - for unit tests
- `IValidator<T>` - for specific validation scenarios
- Database context - for isolated unit tests

### Integration Test Requirements
- Test database (in-memory or dedicated test DB)
- Authentication configuration
- Logging configuration for test output

## Expected Response Formats

### Success Responses (200 OK)
```json
{
  "accessToken": "eyJ...",
  "refreshToken": "eyJ...",
  "expiresIn": 3600
}
```

### Error Responses
- **400 Bad Request**: ValidationProblem with field errors
- **409 Conflict**: Problem details for business rule violations
- **500 Internal Server Error**: Problem details for server errors

## Automation Strategy

### Test Framework
- **Unit Tests**: xUnit with Moq for mocking
- **Integration Tests**: ASP.NET Core Test Host
- **API Tests**: HttpClient with test server

### CI/CD Integration
- Run all tests on pull requests
- Include code coverage reporting
- Performance regression testing

### Test Categories for CI
- **Fast Tests**: Unit tests and validation tests
- **Slow Tests**: Integration and database tests
- **Security Tests**: Dedicated security test suite

## Metrics and Coverage

### Code Coverage Targets
- Unit Tests: 90%+ coverage
- Integration Tests: Critical paths covered
- Edge Cases: All error conditions tested

### Performance Benchmarks
- Login endpoint: < 200ms average response time
- Registration endpoint: < 500ms average response time
- Under load: < 1s response time at 95th percentile

## Test Maintenance

### Regular Updates Required
- Update tests when validation rules change
- Update security tests for new threat vectors
- Update performance benchmarks as system evolves

### Documentation Updates
- Keep test scenarios updated with business requirements
- Document new edge cases discovered
- Update security test cases with industry best practices