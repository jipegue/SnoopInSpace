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