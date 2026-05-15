Feature: Login


@api
@auth
@login
Scenario: Login succeeds with valid credentials
	Given a registered user exists
	When I login with valid credentials
	Then the response status code should be 200
	And a JWT token should be returned
	And a refresh token should be returned

Scenario: Login fails with invalid credentials
	Given a registered user exists
	When I login with an invalid password
	Then the response status code should be 401

Scenario: Login fails when email does not exists
	When I login with an unknown email
	Then the response status code should be 401
