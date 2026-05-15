Feature: Refresh token


@api
@auth
@refresh
Scenario: User can refresh access token
    Given a registered user exists
    When I login with valid credentials
    And I refresh the access token
    Then the response status code should be 200
    And a new JWT token should be returned
    And a new refresh token should be returned

Scenario: Old refresh token cannot be reused
    Given a registered user exists
    When I login with valid credentials
    And I refresh the access token
    And I try to reuse the previous refresh token
    Then the response status code should be 401
    
Scenario: Invalid refresh token is rejected
    When I refresh the access token with an invalid refresh token
    Then the response status code should be 401

