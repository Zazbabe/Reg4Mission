# Resursportalen

Resursportalen is a web application built with **ASP.NET Core MVC** that aims to simplify how municipalities match people with commission-based assignments such as **kontaktperson**, **ledsagare**, and **avlösare** within LSS and other municipal services.

The platform is designed to help municipalities **find suitable candidates efficiently**, while allowing individuals to **create profiles and present their qualifications and interests**.

This project is developed as part of a **software development education program**.

---

## Project Purpose

Municipalities often need to recruit and match individuals to assignments where personal suitability, interests, and availability are important.

Resursportalen aims to:

- Centralize candidate information
- Simplify the search and matching process
- Provide a structured platform for municipalities to manage assignments
- Allow individuals to present their profiles and interests
- Support role-based access for different types of users

---

## Tech Stack

### Backend
- ASP.NET Core MVC
- C#
- ASP.NET Identity (roles and authentication)
- Entity Framework Core

### Frontend
- Razor Views
- Tailwind CSS

### Database
- SQL Server

### Tools
- Visual Studio
- Git / GitHub

---

## Current Features

### Authentication & Roles
- ASP.NET Identity authentication
- Role management
- AdminController for administrative tasks
- Ability to create and manage roles

### User Profiles
- Profile page for users
- Edit profile functionality
- Profile image preview
- Personal information fields (name, phone, interests, etc.)

### Admin Functionality
- Role creation
- Role management through the admin controller

### UI & Layout
- Tailwind-based responsive design
- Razor view structure
- MVC architecture

---

## Planned Features

### Candidate Search
- Search and filter candidates
- View detailed candidate profiles
- Municipality tools for candidate discovery

### Assignment Management
- Create assignments
- Match candidates to assignments such as:
  - Kontaktperson
  - Ledsagare
  - Avlösare

### Matching Support
- Tools to help municipalities find suitable candidates
- Candidate interest tracking

### GDPR & Data Management
- Consent tracking
- Data export
- Data anonymization or deletion

### Additional Features
- Notifications
- Profile view tracking
- Improved admin tools
- Role-based UI

---

## Project Structure
Controllers/
AdminController.cs
ProfileController.cs

Views/
Profile/
Index.cshtml
Edit.cshtml

wwwroot/
css/
js/

Models/
Data/
Services/ (planned)


---

## Development Workflow

Feature development is done using Git branches.

Example workflow:
git checkout -b feature/profile-view-mode
git add .
git commit -m "feat: add profile view page and edit page"
git push -u origin feature/profile-view-mode


Pull requests are then reviewed and merged into the `main` branch.

---

## Current Development Focus

The current development phase focuses on:

- Expanding profile functionality
- Improving UI structure
- Building backend logic for profile management
- Implementing search and matching features

---

## Author

Developed as part of a **software development education program**.
