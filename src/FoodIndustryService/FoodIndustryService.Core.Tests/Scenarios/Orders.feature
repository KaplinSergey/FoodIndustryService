Feature: Orders
	As a customer
  I want to deal with Orders functionality

@AddOrder
Scenario: Add simple order
	Given I have added following products into the order
  | Name         | Price | Amount |
	| White Vine   | 10    | 1      |
  Then All price calculated with price policy is
  | Name       | Price |
  | All  price | 10    |
	And I have added following products into the order
  | Name | Price | Amount |
  | Bear | 5     | 2      |
  Then All price calculated with price policy is
  | Name       | Price |
  | All  price | 20    |
	When I press add order and set customer info
  | Name |
  | Ivan |
	Then the Order should be added on the Orders system
  | CustomerName | Name       | Price | Amount |
  | Ivan         | White Vine | 10    | 1      |
  | Ivan         | Bear       | 5     | 2      |

@EditAndAddOrder
Scenario: Edit simple order
	Given I have added following products into the order
  | Name         | Price | Amount |
	| White Vine   | 10    | 1      |
  Then All price calculated with price policy is
  | Name       | Price |
  | All  price | 10    |
	And I have added following products into the order
  | Name | Price | Amount |
  | Bear | 5     | 2      |
  Then All price calculated with price policy is
  | Name       | Price |
  | All  price | 20    |
  And I have deleted following products from the order
  | Name | Price | Amount |
  | Bear | 5     | 1      |
  Then All price calculated with price policy is
  | Name       | Price |
  | All  price | 15    |
	When I press add order and set customer info
  | Name |
  | Ivan |
	Then the Order should be added on the Orders system
  | CustomerName | Name       | Price | Amount |
  | Ivan         | White Vine | 10    | 1      |
  | Ivan         | Bear       | 5     | 1      |