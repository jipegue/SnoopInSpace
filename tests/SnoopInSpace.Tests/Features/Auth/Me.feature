Feature: Me


@api
@auth
@me
Scenario: Anonymous user cannot access user endpoint
	When I call GET "/me"
	Then the response status code should be 401

Scenario: Authenticated user can access current user endpoint
	Given a registered user exists
	When I login with valid credentials
	And I call GET "/me" with the JWT token
	Then the response status code should be 200
	And the current user email should match the registered user email
