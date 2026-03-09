## Activity 1:
 - Two working classes: InventoryItem and Order 
	- Created three classes
		- OrderItem
		- OrderSummaryDto for performance as there is not tracking from EF
		- Order
		- InventoryItem
	- Reason: is that Quantity now can be represented as the quantity ordered in an order and for Quantity from the perspective of inventory

- Test data with basic output shown in controller - /ordersummaries?page=0&pageSize=200

- LogiTrackContext with EF Core integration - Done

- Database successfully created and tested - Done

- Copilot-assisted review applied - CoPilot recommended the number of classes stated above after having a conversation about Quantity

- Code saved for integration in Part 2 - Done


## Activity 2:

- Two API controllers: Inventory and Order - Done

- Working GET, POST, DELETE routes for both controllers - Done

- Routes tested in Swagger or Postman - Done

- Microsoft Copilot suggestions applied - Done

- Code saved for use in Part 3 - Done


## Activity 3:

- ASP.NET Identity installed and configured - Done

- ApplicationUser model created - Done

- Registration and login routes added - Done

- JWT token-based authentication implemented - Done

- Routes secured with [Authorize] and roles - Done

- Copilot suggestions reviewed and applied - Done

- Database updated with Identity schema - Done


## Activity 4:
In-memory caching implemented on at least one route - Done

Query optimizations applied in controller logic - Done

Slow or repeated queries reduced or eliminated - Done

Cache expiration policy in place - Done

Copilot prompts used to refine logic - Done

	“Suggest performance improvements in this controller.”

	“How can I avoid repeated DB calls for the same result?”

	“Improve the speed of this API endpoint.”

	"Add JWT authentication to an ASP.NET Core API."

	"Suggest improvements to secure user registration."
	"•	Implement email confirmation flow and confirmemail endpoint,"
	"•	Add quantity support in CreateOrderDto and respect it when creating OrderItem entries."
	"•	Add index / migration suggestions for frequently filtered columns (e.g., DatePlaced, NormalizedCustomerName)."

	"Check the linq queries in OrderController.cs and optimize them, some come from the LogicTrackContext assess those as well"

	"Check the linq queries in InventoryController.cs and optimize them, some come from the LogicTrackContext assess those as well"
	"shouldnt the page param be by default 0?"

	"please add in every controller method a stopwatch and send send time to execute each controller to the requests calling them so I can document their performance"
	"yes put it in the response body"

	"Now where appropriate and only in the InventoryController and OrderController utilize the IMemoryCache build into aspcorenet"

	"The GetAll doesa not have a stopwatch"


Improvements verified through testing or time measurement - Done

Code saved for use in Part 5  - Done

Caching Improvements
-------------------------------------------------------

- Before caching InventoryController 
		- GetAll = 1.06ms
		- GetById = 4.18ms
		
- Before caching OrderController
	- GetAll =  45.32ms
	- GetById = 52.32ms

- After caching InventoryController 
	- GetAll = 0.02ms
	- GetById = 3.87ms
	
- After caching OrderController
	- GetAll =  0.01ms
	- GetById = 0.01ms

-------------------------------------------------------

## Activity 5:

Persistent state implementation verified  - Done

Full workflow tested: authentication, caching, API behavior  - Done

Code optimized using Copilot recommendations  - Done 

Redundant or unnecessary logic removed  - Done

Final version ready for peer submission  - Done