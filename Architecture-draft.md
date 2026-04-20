# 🔐 Fluxo de Autenticação — Web Only (Entidades e Endpoints)

Este documento descreve as entidades e endpoints necessários para implementar o fluxo de autenticação Web, baseado em:
- Username único  
- Password parcial (5 posições aleatórias de 10)  
- Avaliação de risco  
- MFA condicional via OTP (email/SMS)  
- Sem suporte para mobile apps nativas  

---

# 🧱 Entidades

## 1. User
Representa o utilizador.

**Campos:**
- `Id`
- `Username` (único)
- `PasswordHash`
- `Email`
- `PhoneNumber`
- `IsActive`
- `CreatedAt`

---

## 2. UserSecurityState
Estado de segurança associado ao utilizador.

**Campos:**
- `UserId`
- `FailedLoginAttempts`
- `LastLoginAt`
- `LastPasswordChangeAt`
- `IsLocked`
- `LockoutEndAt`

---

## 3. LoginAttempt
Registo de tentativas de login para avaliação de risco.

**Campos:**
- `Id`
- `UserId`
- `IpAddress`
- `UserAgent`
- `Location`
- `Timestamp`
- `RiskScore` (low/medium/high)
- `WasSuccessful`

---

## 4. OtpCode
Código OTP para MFA.

**Campos:**
- `Id`
- `UserId`
- `Code` (hash)
- `Channel` (email/sms)
- `ExpiresAt`
- `UsedAt`
- `Purpose`

---

## 5. PartialPasswordRequest
Guarda as posições pedidas da password.

**Campos:**
- `Id`
- `UserId`
- `RequestedPositions` (ex.: `[2,4,6,7,10]`)
- `ExpiresAt`
- `UsedAt`

---

# 🌐 Endpoints

## 1. POST /auth/identify
Identifica o utilizador e devolve as posições da password parcial.

**Input:**
- `username`

**Output:**
- `userId`
- `requestedPasswordPositions`
- `partialPasswordRequestId`

---

## 2. POST /auth/validate-password
Valida os caracteres fornecidos e calcula o risco.

**Input:**
- `partialPasswordRequestId`
- `characters` (mapa posição → caractere)

**Output:**
- `riskLevel` (low/medium/high)
- `requiresMfa` (true/false)
- `loginSessionId`

---

## 3. POST /auth/send-otp
Envia o OTP para email ou telemóvel.

**Input:**
- `loginSessionId`
- `channel` (email/sms)

**Output:**
- `otpSent: true`

---

## 4. POST /auth/validate-otp
Valida o OTP e conclui o login.

**Input:**
- `loginSessionId`
- `otpCode`

**Output:**
- `authenticated: true`
- `authorizationCode` ou `tokens`

---

## 5. POST /auth/logout
Termina a sessão e revoga tokens.

---

# ✔️ Resumo
**Entidades:**  
User, UserSecurityState, LoginAttempt, OtpCode, PartialPasswordRequest  

**Endpoints:**  
/auth/identify, /auth/validate-password, /auth/send-otp, /auth/validate-otp, /auth/logout
