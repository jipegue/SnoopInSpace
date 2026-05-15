Feature: Admin access


@api
@auth
@admin
Scenario: Anonymous user cannot access admin endpoint
	When I call GET "/admin"
	Then the response status code should be 401

Scenario: Standard user cannot access admin endpoint
	Given a registered user exists
	When I login with valid credentials
	And I call GET "/admin" with the JWT token
	Then the response status code should be 403

Scenario: Admin user can access admin endpoint
	Given the seeded admin user exists
	When I login as seeded admin
	And I call GET "/admin" with the JWT token
	Then the response status code should be 200
