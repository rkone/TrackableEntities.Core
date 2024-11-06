Feature: ManyToMany Relations
	In order to update entity relations
	As a Web API client
	I want to save many-to-many relations to the database

@entity_relations
Scenario: Attach Existing Territory to Employee
	When I GET the employee by id 1
	And I GET the territory by id "02116"
	And I add territory "02116" to employee 1
	And I submit a PUT to update the employee
	And I GET employee 1 to the results
	Then the employee should have the territory

@entity_relations
Scenario: Attach New Territory to Employee
	Given the following new employees
	| EmployeeId | LastName | FirstName |
	| 314 | Suyama   | Michael   |
	And the following new territories
	| TerritoryId | TerritoryDescription |
	| 1 | Seattle |
	When I submit POSTs to create employees
	And I add territory "1" to employee 314
	And I submit a PUT to update the employee
	And I GET employee 314 to the results
	Then the employee should have the territory

@entity_relations
Scenario: Modify Employee Territory
	When I GET the employee by id 1	
	And I modify territory "01581" from employee 1 to have description "1Seattle"
	And I submit a PUT to update the employee
	And I GET employee 1 to the results
	Then the employee should have territory "01581" with description "1Seattle"

@entity_relations
Scenario: Remove Territory from Employee
	When I GET the employee by id 1	
	And I remove territory "01581" from employee 1
	And I submit a PUT to update the employee
	And I GET employee 1 to the results
	Then the employee should not have the territory

