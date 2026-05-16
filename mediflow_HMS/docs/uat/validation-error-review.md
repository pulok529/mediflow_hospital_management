# Validation and Error Message Review

## Frontend
- Standardized API error fallback messages for common HTTP statuses
- Added mandatory-field validation before submission on high-traffic reception forms
- Added loading-state buttons and inline error surfaces

## Backend
- Existing FluentValidation and problem details pipeline retained
- AI review endpoints preserve not-found and auth behavior

## UAT Verification Cases
1. Submit empty patient registration form -> clear mandatory-field message
2. Submit empty appointment booking form -> clear mandatory-field message
3. Invalid credentials -> readable login failure message
4. Expired token path -> unauthorized message and re-login flow
