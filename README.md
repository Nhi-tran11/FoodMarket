# FoodMarketApp

A full-stack food marketplace web application built with React and .NET 10, featuring GraphQL APIs, Stripe payments, a referral code system, and email notifications.

---

## Features

- Browse and search food products
- Shopping cart with real-time order management
- Checkout with Stripe payment integration
- Referral code & discount system
- Order tracking and history
- Shipping details management
- Admin seeding on startup
- Automatic cleanup of expired pending orders (background service)
- Email notifications (SMTP / AWS SES)
- File upload support

---

## Tech Stack

### Backend
| Technology | Purpose |
|---|---|
| .NET 10 / C# | API server |
| HotChocolate 14 | GraphQL server |
| Entity Framework Core 10 | ORM & migrations |
| PostgreSQL (Npgsql) | Database |
| Stripe.net | Payment processing |
| MailKit / AWS SES | Email notifications |
| QRCoder | QR code generation |
| Docker | Database containerization |

### Frontend
| Technology | Purpose |
|---|---|
| React 18 | UI framework |
| Apollo Client 3 | GraphQL client |
| React Router 6 | Client-side routing |
| Stripe React SDK | Payment UI |
| React Context API | State management (cart, auth) |

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (v18+) and [pnpm](https://pnpm.io/)
- [Docker](https://www.docker.com/) (for PostgreSQL)

---

## Getting Started

### 1. Clone the repository

```bash
git clone <repo-url>
cd FoodMarketApp
```

### 2. Start the database

```bash
cd Backend
docker-compose up -d
```

This starts a PostgreSQL instance on port `5433` with:
- **Database:** `FoodMarketDb`
- **Username:** `foodmarketuser`
- **Password:** `foodmarketpass123`

### 3. Configure environment variables

Create a `.env` file inside the `Backend/` directory:

```env
STRIPE_SECRET_KEY=sk_test_...
STRIPE_PUBLISHABLE_KEY=pk_test_...
SMTP_USERNAME=your@email.com
SMTP_PASSWORD=your_smtp_password
```

### 4. Run the backend

```bash
cd Backend
dotnet run
```

On first run, EF Core migrations are applied automatically and an admin user is seeded.

- GraphQL playground: `http://localhost:5000/graphql`
- REST endpoints: `http://localhost:5000`

### 5. Run the frontend

```bash
cd frontend/food-market-frontend
npm install   # or: pnpm install
npm start
```

The React app runs at `http://localhost:3000`.

---

## Project Structure

```
FoodMarketApp/
├── Backend/
│   ├── Controllers/        # REST endpoints (file upload)
│   ├── Data/               # EF Core DbContext & admin seeder
│   ├── GraphQL/
│   │   ├── Mutations/      # GraphQL mutations
│   │   └── Queries/        # GraphQL queries
│   ├── Migrations/         # EF Core database migrations
│   ├── Model/              # Domain models
│   ├── Service/            # Business logic services
│   ├── appsettings.json
│   ├── docker-compose.yml
│   └── Program.cs
└── frontend/
    └── food-market-frontend/
        └── src/
            ├── components/ # React UI components
            └── context/    # Cart & auth context providers
```

---

## GraphQL API

The API is available at `/graphql`. Key operations include:

**Queries**
- `customers` – list/search customers
- `products` – list/search products
- `orderPendingByUserId` – get a user's active cart/order
- `shippingDetails`, `payments`, `referralCodes`

**Mutations**
- `registerCustomer`, `loginCustomer`
- `createProduct`, `updateProduct`, `deleteProduct`
- `createOrder`, `deletePendingOrder`
- `createPaymentIntent`, `confirmPayment`
- `applyReferralCode`, `generateReferralCode`
- `createShippingDetail`, `updateShippingDetail`

---

## Environment Variables Reference

| Variable | Description |
|---|---|
| `STRIPE_SECRET_KEY` | Stripe secret API key |
| `STRIPE_PUBLISHABLE_KEY` | Stripe publishable key |
| `SMTP_USERNAME` | SMTP email username |
| `SMTP_PASSWORD` | SMTP email password |

---

## License

MIT
This is  food marketplace web application using React (frontend) and .NET (backend), exposing a GraphQL API to manage menu, cart, and order workflows. Integrated Stripe for secure payments and implemented an end-to-end checkout process from order creation to payment confirmation. 
