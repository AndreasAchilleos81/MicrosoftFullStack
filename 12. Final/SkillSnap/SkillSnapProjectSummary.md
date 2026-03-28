🌟 SkillSnap — Peer‑Review Project Summary
🧭 Project Overview
SkillSnap is a full‑stack portfolio management application designed to help users showcase their skills and projects in a clean, modern interface. The system allows authenticated users to create, update, and manage their personal project entries while ensuring secure access, efficient data handling, and a smooth user experience across the entire application.

The project integrates ASP.NET Core Web API, Entity Framework Core, and a Blazor WebAssembly frontend, forming a cohesive, production‑style full‑stack solution.

🔑 Key Features
1. Authentication & Access Control
Secure registration and login flow

JWT‑based authentication

Protected API endpoints

UI logic that restricts editing to authenticated users

Unauthorized actions correctly blocked

2. CRUD Functionality
Users can:

Add new projects

Edit existing projects

Delete projects

View project lists with real‑time updates

All CRUD operations are fully wired from Blazor → API → database.

3. Caching & State Management
API‑level caching to reduce redundant database calls

Cache invalidation on create/update/delete

Logging of cache hits/misses for verification

Blazor state refreshes after operations to ensure UI consistency

4. UI/UX Enhancements
Clean, modern layout aligned with Blazor WASM template aesthetics

Improved spacing, typography, and component structure

Placeholder images and sample data for better readability

Mobile‑friendly layout adjustments

5. Copilot‑Assisted Improvements
Throughout development, GitHub Copilot was used to:

Suggest refactoring opportunities

Improve naming conventions and code clarity

Generate helper methods and comments

Identify redundant logic and unused services

Validate secure endpoint patterns and HttpClient token usage

🛠️ Development Process
The project was built iteratively, following a milestone‑driven approach:

Backend foundation

Set up API, EF Core models, and authentication

Implemented CRUD endpoints and caching

Frontend integration

Connected Blazor components to API

Added forms, validation, and state updates

Security validation

Ensured only authenticated users can modify data

Verified token handling and route protection

Refactoring with Copilot

Cleaned up code structure

Improved readability and maintainability

UX polishing

Updated layouts, spacing, and visuals

Added image previews and card‑based design

Final testing

Verified full application flow end‑to‑end

Ensured caching and state logic behaved correctly

🚧 Known Issues / Future Improvements
Although the application is fully functional, several enhancements could be added:

More advanced project filtering and search

User profile customization (bio, avatar, social links)

Drag‑and‑drop image uploads

Dark mode support

More granular caching strategies

Automated integration tests

📄 Submission Readiness
This project meets all requirements listed in the Capstone Part 5 instructions :

Full application flow validated

CRUD, authentication, and state logic confirmed

Caching behavior verified

Code refactored with Copilot

UX polished

Peer‑review documentation prepared