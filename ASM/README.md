# FastFood Online

Project ASM web application for ordering food online. Built with ASP.NET Core MVC (.NET 8).

## Features
- **Guest**: Browse Menu, Filter Categories, Search Foods, View Details, View Combos.
- **Customer**: Register/Login (Local + Google), Manage Profile, Shopping Cart, Checkout (COD/Payment Sim), Order History & Tracking.
- **Admin**: Dashboard, Manage Categories (Placeholder), Secure Layout.

## Setup Instructions

### 1. Database
- Update connection string in `appsettings.json` if needed (Default: `(localdb)\\mssqllocaldb`).
- Run Migrations:
  ```bash
  dotnet ef database update --project FastFoodOnline.Web
  ```

### 2. Google Authentication
- Register a project on [Google Cloud Console](https://console.cloud.google.com/).
- Get ClientId and ClientSecret.
- Update `appsettings.json`:
  ```json
  "Authentication": {
    "Google": {
      "ClientId": "YOUR_CLIENT_ID",
      "ClientSecret": "YOUR_CLIENT_SECRET"
    }
  }
  ```
- Or run commands:
  ```bash
  dotnet user-secrets set "Authentication:Google:ClientId" "<client-id>"
  dotnet user-secrets set "Authentication:Google:ClientSecret" "<client-secret>"
  ```

### 3. Run Application
- Run `dotnet run --project FastFoodOnline.Web`
- Default Users:
  - **Admin**: `admin@local.test` / `Password@123`
  - **Customer**: Register a new account or use Google Login.

## ERD Overview
- **Category** --(1:n)--> **Food**
- **Combo** --(n:n)--> **Food** (via ComboItem)
- **User** --(1:n)--> **Order**
- **Order** --(1:n)--> **OrderItem**
- **Cart** --(1:n)--> **CartItem**
