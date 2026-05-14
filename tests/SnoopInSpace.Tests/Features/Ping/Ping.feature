Feature: Ping


@api
@ping
Scenario: API responds successfully
	When I call GET "/ping"
	Then the response status code should be 200