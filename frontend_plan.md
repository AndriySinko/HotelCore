# Frontend Implementation Plan
TL;DR: Build a 4-step cleaning reservation wizard using React, TypeScript, Tailwind CSS, and Zustand for state management. Each step corresponds to a route and includes specific components and behaviors. Use mock APIs with React Query for data fetching.

## Tech Stack
- React + TypeScript (Vite)
- Tailwind CSS
- React Router v6
- Zustand (global wizard state)
- React Query (server state)
- React Hook Form (forms)

---

## Routes
- / -> redirect to /cleaning/step/1
- /cleaning/step/1
- /cleaning/step/2
- /cleaning/step/3
- /cleaning/step/4

Do NOT block navigation between steps.

---

## Global State (Zustand)
Store:
- selectedRooms: string[]
- cleaningMode: 'immediate' | 'scheduled'
- scheduledDate: string | null
- scheduledTime: string | null
- paymentMethod: 'card' | 'cash'
- paymentDetails: { name: string; token?: string; last4?: string }
- reservation: object | null

Include:
- setters
- reset()

---

## Project Structure

frontend/
  src/
    pages/cleaning/
      Step1RoomSelection.tsx
      Step2Schedule.tsx
      Step3Payment.tsx
      Step4Complete.tsx

    components/
      AppShell/
      Stepper/
      ReservationInfo/
      RoomList/
      Payment/
      DateTime/
      UI/

    store/
      wizardStore.ts

    api/
      client.ts
      cleaning.ts
      payments.ts

    hooks/
    styles/
    utils/
    types/

    App.tsx
    main.tsx
    routes.tsx

  index.html
  package.json
  tsconfig.json
  vite.config.ts
  tailwind.config.js
  postcss.config.js

---

## Shared Layout
All pages use:

AppShell
  Header (Back to dashboard, user info)
  Stepper
  Content container
  Footer actions (Back / Next)

---

## Pages

### Step 1 — Room Selection
Components:
- ReservationInfo
- RoomList (RoomItem with checkbox)
- Button

Behavior:
- Select/deselect rooms
- Continue → Step 2

---

### Step 2 — Schedule Cleaning
Components:
- ReservationInfo
- RadioCard (immediate vs scheduled)
- DatePickerField
- TimeSlotGrid

Behavior:
- Select mode
- If scheduled → pick date + time
- Continue → Step 3

---

### Step 3 — Payment
Components:
- ReservationInfo
- PriceSummary
- PaymentMethodCard
- PaymentForm (if card)

Behavior:
- Switch payment method
- Input card details (basic validation only)
- Continue → Step 4

---

### Step 4 — Complete
Components:
- Success message
- Summary
- Action buttons

Behavior:
- Navigate to dashboard or restart

---

## Reusable UI Components
- Button (primary, secondary)
- Card
- Input
- RadioCard
- Stepper
- Toast / InfoBanner
- Loader / Skeleton

---

## API Layer
Implement mock API (no real backend):

- getReservation()
- getTimeSlots(date)
- createCleaning(payload)
- processPayment(payload)

Use React Query for all async calls.

---

## Styling
- Tailwind CSS
- Consistent spacing and layout
- Mobile-first

---

## Rules
- Do NOT over-engineer
- Keep components reusable
- Use mock data where needed
- Do NOT add features not specified
- Keep logic simple and readable

---

## Implementation Order
1. Project setup (Vite + Tailwind + Router)
2. Layout (AppShell, Stepper)
3. Static pages (match UI)
4. Zustand store
5. Interactions
6. Mock API + React Query
7. Basic validation + polish