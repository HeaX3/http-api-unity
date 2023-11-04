# HTTP Api Client Template

Template to easily write HTTP REST API clients.

## Getting Started

To create a new API client:

1. Inherit the HttpApi class
2. Create a DontDestroyOnLoad(gameObject) instance carrying a component of your class
3. Start interacting with the API client from your code

## Best Practices

- **Explicit method arguments**: Use explicit method arguments for your endpoints and map them to the request body in
  the
  client logic instead of accepting generic json bodies and leaving the formatting to the API consumer.
- **Explicit return types**: Parse request responses early and return explicit data instead of generic json. Throw
  verbose
  exceptions when the response doesn't meet expectations.

## Considerations

- This template assumes that all requests format the body as application/json by default. Change the `Content-Type`
  default header to override this behaviour.