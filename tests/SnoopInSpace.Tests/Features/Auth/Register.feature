Feature: Register


@api
@auth
@register
Scenario: Register succeeds with valid credentials
	When I register with valid credentials
	Then the response status code should be 201

Scenario: Register fails when email already exists
	Given a registered user already exists
	When I register with the same email
	Then the response status code should be 409

Scenario: Register replay with same idempotency key returns same result
  Given an idempotency key "register-001"
  When I register with valid credentials
  And I register again with the same idempotency key
  Then the response status code should be 201
  And the replayed register response should match the original response

Scenario: Register replay with same idempotency key and different payload fails
  Given an idempotency key "register-002"
  When I register with valid credentials
  And I register again with a different payload and the same idempotency key
  Then the response status code should be 409