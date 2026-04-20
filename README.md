# IdentityProvider - OAuth2 Login System

Sistema de autenticação e autorização baseado em **OAuth2/OpenID Connect** usando [OpenIddict](https://github.com/openiddict).

## Arquitetura

### Estrutura da Solução

```
OpenIdDict-IdentityProvider/
├── IdentityProvider.sln
└── src/
    ├── IdentityProvider.Domain/               ← DLL: Domain (sem dependências)
    ├── IdentityProvider.Application/          ← DLL: Application (depende de Domain)
    ├── IdentityProvider.Infrastructure/       ← DLL: Infrastructure (depende de Domain + Application)
    ├── IdentityProvider.Server/               ← Web API (depende de todas)
    └── zirku-app/                  ← React Frontend
```

### Diagrama de Dependências (Clean Architecture)

```
┌─────────────────┐
│  IdentityProvider.Server   │  (Presentation - Web API)
│                 │
│  ┌───────────┐  │
│  │Controllers│  │  references
│  │  Config   │  │  ┌──────────────────┐
│  └─────┬─────┘  │  │ IdentityProvider.Server     │
└────────┼────────┘  └──────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  IdentityProvider.Infrastructure  (Data Access & Identity)         │
│                                                         │
│  ┌────────────┐  ┌──────────────────────────────┐      │
│  │ UserStore  │  │ OpenIddictConfiguration      │      │
│  │ DB Seeder  │  │ (EF Core, Quartz, SQLite)    │      │
│  └─────┬──────┘  └──────────────────────────────┘      │
└────────┼────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  IdentityProvider.Application  (Business Logic & DTOs)             │
│                                                         │
│  ┌────────────┐  ┌──────────────────────────────┐      │
│  │ DTOs       │  │ IDatabaseSeeder              │      │
│  └────────────┘  └──────────────────────────────┘      │
└────────┼────────────────────────────────────────────────┘
         │
         ▼
┌─────────────────────────────────────────────────────────┐
│  IdentityProvider.Domain  (Core Entities & Interfaces)             │
│                                                         │
│  ┌────────────┐  ┌──────────────────────────────┐      │
│  │ User       │  │ IUserStore                   │      │
│  └────────────┘  └──────────────────────────────┘      │
└─────────────────────────────────────────────────────────┘
```

### Regras de Dependência

| Camada | Depende de | Packages NuGet |
|---|---|---|
| **Domain** | Nenhuma | Nenhum |
| **Application** | Domain | Nenhum |
| **Infrastructure** | Domain, Application | EF Core, OpenIddict, Quartz |
| **Server** | Todas | OpenIddict.AspNetCore, Swashbuckle |

### Fluxo de Autenticação

```
┌─────────────────────────────────────────────────────────────┐
│                    IdentityProvider.Server (Port 44319)                │
│                                                             │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────┐ │
│  │ OAuth2 Endpoints│  │ Auth API        │  │ Resource API│ │
│  │ /connect/*      │  │ /api/auth/*     │  │ /api/*      │ │
│  └─────────────────┘  └─────────────────┘  └─────────────┘ │
│                                                             │
│  ┌─────────────────────────────────────────────────────────┐│
│  │ Static Files: wwwroot/login/ (React Login Pages)        ││
│  └─────────────────────────────────────────────────────────┘│
└─────────────────────────────────────────────────────────────┘
                              │
                         OAuth2 PKCE Flow
                              │
┌─────────────────────────────────────────────────────────────┐
│                    zirku-app (Port 3000) - React            │
│                                                             │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────┐ │
│  │ Home        │  │ Callback    │  │ Dashboard (Protected)│ │
│  └─────────────┘  └─────────────┘  └─────────────────────┘ │
└─────────────────────────────────────────────────────────────┘
```

## Projetos

### IdentityProvider.Domain
Biblioteca de classes que contém as entidades e interfaces do domínio.

| Ficheiro | Descrição |
|---|---|
| `Entities/User.cs` | Entidade User (Username, PasswordHash, SubjectId, Scopes) |
| `Interfaces/IUserStore.cs` | Interface para operações de utilizador |

### IdentityProvider.Application
Biblioteca de classes que contém DTOs e interfaces de aplicação.

| Ficheiro | Descrição |
|---|---|
| `DTOs/LoginRequest.cs` | DTO para pedido de login |
| `DTOs/RegisterRequest.cs` | DTO para pedido de registo |
| `Interfaces/IDatabaseSeeder.cs` | Interface para seed de dados |

### IdentityProvider.Infrastructure
Biblioteca de classes que contém implementações concretas.

| Ficheiro | Descrição |
|---|---|
| `Data/UserStore.cs` | Implementação In-Memory de `IUserStore` |
| `Data/DatabaseSeeder.cs` | Implementação de `IDatabaseSeeder` |
| `Identity/OpenIddictConfiguration.cs` | Configuração do OpenIddict Core |

### IdentityProvider.Server
Web API que serve como ponto de entrada da aplicação.

| Componente | Descrição |
|---|---|
| **Controllers** | `AuthController`, `AuthorizationController`, `ResourceController` |
| **Config** | CORS, Kestrel, Swagger |
| **wwwroot/login/** | Páginas React para login/registo |

### zirku-app
Frontend React com autenticação OAuth2 PKCE:

| Página | Descrição |
|---|---|
| **Home** | Página inicial com botão de login |
| **Callback** | Handler do callback OAuth2 |
| **Dashboard** | Página protegida que mostra user info e chama API |

## Utilizadores Demo

| Username | Password | Scopes |
|---|---|---|
| Alice | password | openid, offline_access, api1, api2 |
| Bob | password | openid, api1 |

## Execução

### Pré-requisitos
- .NET 8 SDK
- Node.js (para React App)
- Certificado de desenvolvimento HTTPS do ASP.NET Core

### 1. Iniciar o Server

```bash
dotnet run --project src/IdentityProvider.Server
```

O Server inicia em `https://localhost:44319` e serve as páginas de login em `https://localhost:44319/login`.

### 2. Iniciar a React App

```bash
cd zirku-app
npm install
npm start
```

A React App inicia em `http://localhost:3000`.

### 3. Testar o Fluxo

1. Abra `http://localhost:3000`
2. Clique "Sign In with IdentityProvider"
3. Será redirecionado para a página de login do Server
4. Insira as credenciais (ex: Alice / password)
5. Será redirecionado de volta para a React App autenticado
6. Aceda ao Dashboard para ver os dados do utilizador e a resposta da API protegida

## Endpoints

| Endpoint | Método | Descrição |
|---|---|---|
| `/connect/authorize` | GET/POST | Endpoint de autorização OAuth2 |
| `/connect/token` | POST | Endpoint de token OAuth2 |
| `/connect/introspect` | POST | Endpoint de introspecção de tokens |
| `/api/auth/login` | POST | Login (retorna info do utilizador) |
| `/api/auth/register` | POST | Registo de novo utilizador |
| `/api/profile` | GET | Recurso protegido (requer Bearer token) |
| `/login` | GET | Páginas de login React |
| `/swagger` | GET | Documentação Swagger UI |

## Tecnologias

- **ASP.NET Core 8** - Framework web
- **OpenIddict** - Biblioteca OAuth2/OpenID Connect
- **Entity Framework Core** - ORM com SQLite
- **Quartz.NET** - Agendamento de tarefas
- **React 18** - Frontend
- **oidc-client-ts** - Cliente OAuth2/OIDC para JavaScript
- **PKCE** - Proof Key for Code Exchange (segurança para SPAs)
- **Swashbuckle** - Documentação Swagger

## Notas de Segurança

Este é um projeto de **demonstração**. Em produção:
- Nunca armazene chaves de criptografia no código-fonte
- Use Azure KeyVault ou similar para armazenar chaves
- Certificados de desenvolvimento não devem ser usados em produção
- Implemente autenticação real de utilizadores (com base de dados)
- Use HTTPS em produção
- Configure CORS adequadamente
