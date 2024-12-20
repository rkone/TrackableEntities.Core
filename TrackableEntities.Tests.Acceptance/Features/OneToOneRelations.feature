﻿Feature: OneToOne Relations
	In order to update entity relations
	As a Web API client
	I want to save one-to-one relations to the database

@ignore @entity_relations
Scenario: Create Customer Setting
	Given the following new customers
	| CustomerId | CustomerName |
	| ACMEC      | Acme Company |
	When I submit POSTs to create customers
	And I add a customer setting
	And I submit a PUT to update the customer
	Then the request should return the new customer