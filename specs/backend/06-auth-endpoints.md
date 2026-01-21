# Backend Spec: Authentication Endpoints

**File**: `specs/backend/06-auth-endpoints.md`  
**Task**: TASK-012, TASK-013  
**Dependencies**: TASK-010 (ASP.NET Identity), TASK-011 (JWT)

---

## 📋 Overview

Create RESTful authentication endpoints for user registration, login, token refresh, and logout.

---

## 🎯 Endpoints

### 1. POST /api/auth/register

**Purpose**: Register a new user account

**Request**:
```json
{
  "username": "string (3-50 chars, alphanumeric + underscore)",
  "email": "string (valid email)",
  "password": "string (8+ chars, uppercase, lowercase, number, special char)"
}
```

**Response (201 Created)**:
```json
{
  "user": {
    "id": "string (UUID)",
    "username": "string",
    "email": "string",
    "subscriptionTier": "Free",
    "createdAt": "2025-01-21T10:00:00Z"
  },
  "accessToken": "string (JWT)",
  "refreshToken": "string (opaque token)"
}
```

**Errors**:
- `400 Bad Request`: Validation failed (details in response)
- `409 Conflict`: Email or username already exists
- `429 Too Many Requests`: Rate limit exceeded

**Validation**:
```csharp
public class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .Length(3, 50)
            .Matches(@"^[a-zA-Z0-9_]+$")
            .WithMessage("Username can only contain letters, numbers, and underscores");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(255);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(8)
            .Matches(@"[A-Z]").WithMessage("Must contain uppercase letter")
            .Matches(@"[a-z]").WithMessage("Must contain lowercase letter")
            .Matches(@"[0-9]").WithMessage("Must contain number")
            .Matches(@"[\W]").WithMessage("Must contain special character");
    }
}
```

**Implementation Checklist**:
- [ ] Create `RegisterRequest` DTO
- [ ] Create `RegisterRequestValidator`
- [ ] Implement `AuthController.Register()` endpoint
- [ ] Check if email/username exists (return 409 if duplicate)
- [ ] Create user via ASP.NET Identity
- [ ] Generate JWT access token
- [ ] Generate and store refresh token (hashed)
- [ ] Return user + tokens
- [ ] Log registration in audit log
- [ ] Write unit tests (valid, invalid inputs, duplicates)
- [ ] Write integration test (full registration flow)

---

### 2. POST /api/auth/login

**Purpose**: Authenticate existing user

**Request**:
```json
{
  "email": "string",
  "password": "string"
}
```

**Response (200 OK)**:
```json
{
  "user": {
    "id": "string",
    "username": "string",
    "email": "string",
    "subscriptionTier": "string"
  },
  "accessToken": "string",
  "refreshToken": "string"
}
```

**Errors**:
- `400 Bad Request`: Missing email or password
- `401 Unauthorized`: Invalid credentials
- `429 Too Many Requests`: Rate limit exceeded (5 attempts/minute per IP)

**Implementation Checklist**:
- [ ] Create `LoginRequest` DTO
- [ ] Implement `AuthController.Login()` endpoint
- [ ] Verify credentials via ASP.NET Identity `SignInManager`
- [ ] If valid, generate JWT access token
- [ ] Generate and store new refresh token
- [ ] Invalidate old refresh tokens for this user
- [ ] Return user + tokens
- [ ] Log successful login in audit log
- [ ] Log failed login attempts
- [ ] Implement account lockout after 5 failed attempts
- [ ] Write unit tests
- [ ] Write integration test

---

### 3. POST /api/auth/refresh-token

**Purpose**: Get new access token using refresh token

**Request**:
```json
{
  "refreshToken": "string"
}
```

**Response (200 OK)**:
```json
{
  "accessToken": "string",
  "refreshToken": "string (new refresh token)"
}
```

**Errors**:
- `400 Bad Request`: Missing refresh token
- `401 Unauthorized`: Invalid or expired refresh token

**Implementation Checklist**:
- [ ] Create `RefreshTokenRequest` DTO
- [ ] Implement `AuthController.RefreshToken()` endpoint
- [ ] Hash incoming refresh token (SHA256)
- [ ] Look up token in `RefreshTokens` table
- [ ] Verify token not expired and not revoked
- [ ] Get associated user
- [ ] Generate new JWT access token
- [ ] Generate new refresh token and store
- [ ] Revoke old refresh token
- [ ] Return new tokens
- [ ] Write unit tests
- [ ] Write integration test

---

### 4. POST /api/auth/logout

**Purpose**: Invalidate refresh token

**Request**: No body (uses Authorization header)

**Response (204 No Content)**: Empty

**Errors**:
- `401 Unauthorized`: Not authenticated

**Implementation Checklist**:
- [ ] Implement `AuthController.Logout()` endpoint
- [ ] Require authentication (`[Authorize]` attribute)
- [ ] Get user ID from JWT claims
- [ ] Revoke all active refresh tokens for user
- [ ] Return 204 No Content
- [ ] Write unit tests
- [ ] Write integration test

---

## 🧪 Testing Requirements

### Unit Tests

**File**: `tests/CrochetAI.Api.Tests/Controllers/AuthControllerTests.cs`

Test Cases:
1. Register with valid data → 201 Created
2. Register with duplicate email → 409 Conflict
3. Register with duplicate username → 409 Conflict
4. Register with invalid password → 400 Bad Request
5. Login with valid credentials → 200 OK + tokens
6. Login with invalid credentials → 401 Unauthorized
7. Login with non-existent email → 401 Unauthorized
8. Refresh token with valid token → 200 OK + new tokens
9. Refresh token with expired token → 401 Unauthorized
10. Logout → revokes refresh tokens

### Integration Tests

**File**: `tests/CrochetAI.IntegrationTests/AuthFlowTests.cs`

Test Cases:
1. Full registration → login → access protected endpoint
2. Register → login → refresh token → access protected endpoint
3. Register → login → logout → refresh token fails
4. Attempt 6 failed logins → account locked

---

## 🔒 Security Considerations

1. **Rate Limiting**: Auth endpoints MUST have strict rate limits
2. **Password Hashing**: NEVER store plain passwords (ASP.NET Identity handles this)
3. **Token Storage**: Store refresh tokens hashed, not plain text
4. **Token Rotation**: Always generate new refresh token on refresh
5. **Audit Logging**: Log all auth events (success and failure)
6. **Account Lockout**: Implement after failed login attempts
7. **HTTPS Only**: Auth endpoints MUST be HTTPS only
8. **No Stack Traces**: Never expose error details in production

---

## 📁 File Structure

```
backend/
├── Controllers/
│   └── AuthController.cs
├── Services/
│   ├── IAuthService.cs
│   └── AuthService.cs
├── DTOs/
│   ├── RegisterRequest.cs
│   ├── LoginRequest.cs
│   ├── RefreshTokenRequest.cs
│   └── AuthResponse.cs
├── Validators/
│   ├── RegisterRequestValidator.cs
│   └── LoginRequestValidator.cs
└── tests/
    ├── CrochetAI.Api.Tests/
    │   └── Controllers/
    │       └── AuthControllerTests.cs
    └── CrochetAI.IntegrationTests/
        └── AuthFlowTests.cs
```

---

## ✅ Definition of Done

This task is complete when:

- [ ] All 4 endpoints implemented
- [ ] All validators created
- [ ] Rate limiting active on auth endpoints
- [ ] All unit tests pass (10+ tests)
- [ ] All integration tests pass (4+ tests)
- [ ] Audit logging working
- [ ] Account lockout working after 5 failed attempts
- [ ] No security warnings from analyzers
- [ ] Swagger documentation generated
- [ ] Code reviewed (by Ralph or human)

---

**Next Steps**: After auth endpoints are complete, proceed to TASK-014 (Frontend Auth Context)
