# AiImageWebAPI
A cross-platform **.NET 8 MAUI + Blazor Hybrid App** that connects to multiple AI image-processing backends  
(**Stability AI**, **OpenAI**, **Remove.bg**, and **Real-ESRGAN**) through a **unified REST API pipeline**.

| Project | Description |
|----------|--------------|
| **AiImageCore** | Shared logic: models, DTOs, services, and reusable utilities |
| **AiImageApi** | ASP.NET Core Web API that handles authentication and AI image processing |
| **AiImageHybridApp** | Cross-platform .NET MAUI + Blazor Hybrid frontend |

## Tech stack
- **Language / Framework**: .NET 8 (C#), FastAPI (Python 3.11 planned integration)
- **Database**: None (stateless pipeline; future support for SQLite / PostgreSQL)
- **Authentication**: Basic JWT planned for multi-user workflow
- **Tools & DevOps**: Docker · Swagger / OpenAPI · GitHub Actions (CI/CD)
- **AI Integrations**: Stability AI · OpenAI · Real-ESRGAN · Remove.bg · ImageSharp

## AI Integration

- **Stability AI** – image generation
- **OpenAI** – text/image prompts
- **Real-ESRGAN** – image upscaling
- **Remove.bg** – background removal

## Authentication

- JWT-based auth (planned)
- Token generated via `/login`, validated via `/secure`

> See `/docs/setup-guide.md` for local development setup.
