# Monk_Task
 
# Detailed API Cases & Scenarios

**2. GET /coupon (Retrieve all coupons)**
This endpoint allows us to retrieve a list of all available coupons from the system. The coupon are returned based on whether the coupon is still valid(not expired) and active in the system
.
**3. GET /coupons/{id} (Retrieve a specific coupon by its ID)**
This endpoint allows us to retrieve a single coupon's detail from the system.It is returned based on whether the coupon is still valid(not expired) and active in the system.

**4. PUT /coupons/{id} (Update a specific coupon by its ID)** (Not Implemented)
This endpoint allows us to update the details of a single coupon in the system. When updating, we need to consider the coupon type because each type has specific requirements and validations
Server-side Validations:
If the coupon is cart-wise, only discount and threshold values should be required.
If the coupon is product-wise, the productId and discount must be required.
For BXGY type coupons, the buy products, get products, and repetition value are required.
Database Updates:
First, delete all the existing records for the coupon from its related sub-tables, such as tblBuyProducts and tblGetProduct.Then insert updated details into the main tblCoupons table and the sub-tables, if needed based on the type of coupon.

**5. DELETE /coupons/{id} (Delete a specific coupon by its ID)**
This endpoint allows us to soft delete a coupon by updating its status in database. Instead of permanently removing the coupon, we set its status to 0, which means the coupon is now inactive/deleted.So that it's data remains in the system for future references as well.
And then the coupon is no longer valid or applicable for use after being marked as inactive

**6. POST /applicable-coupons (Fetch all applicable coupons for a given cart and calculate the total discount that will be applied by each coupon.)**
## Cart-wise Coupons:
	Condition: The total value of the cart (@CartTotal) meets or exceeds the coupon's threshold.
	Result: Applies a discount percentage to the total cart value.
## Product-wise Coupons:
	Condition: Specific products in the cart match the products eligible for the coupon.
	Result: Applies a discount percentage to the total amount of the matching products.
## Buy X Get Y (BXGY) Coupons:
	Condition:Cart contains sufficient quantities of the buy products required by the coupon.Meets the repetition limit specified in the coupon.
	Result: Calculates a discount based on the "get" products associated with the coupon.
	If insufficient "buy" quantities are available, no discount is applied.

	Apart from this, procedure ensures that coupons are only applied if their thresholds or conditions are met and prevents InActive or expired or deleted coupons from being considered.
	Discounts for BXGY coupons respect the repetition limit (repitionLimit).

**7. POST /apply-coupon/{id} (Apply a specific coupon to the cart and return the updated cart with discounted prices for each item.)** (Not Implemented)
This endpoint will start by
Check Coupon Validity: Verify if the coupon is active (status = 1) and not expired (expiryDate > GETUTCDATE()), If invalid, return an error message like "Coupon has expired or is inactive." without further calculations.
Determine Coupon Type:Retrieve the coupon type (cart-wise, product-wise, or BxGy) from the database and process accordingly.
## Cart-Wise Coupons
Logic:
	Retrieve the threshold and discount values from tblCoupons.
	Calculate the total price of all cart items (sum(cartItem.price * cartItem.quantity)).
	Check if the total price is >= the threshold.
	If yes, calculate the discount (total_price * (discount / 100)).
	Subtract the discount from the total price and return the total price, total discount, and final price.
	Distribute the discount equally for all cart items in totalDiscount fields for each item.
		Example:
	Input: Total Price = 1000, Threshold = 500, Discount = 10%
	Output:
	Total Price = 1000
	Total Discount = 100
	Final Price = 900
	Cart Items: Each item's discount is proportional.
## Product-Wise Coupons
Logic:
	Retrieve the productId and discount from tblCoupons.
	For cart items with matching productId, calculate the discount (item_price * (discount / 100)).
	Subtract the discount from the total price of matching items and return the total discount and final price.
	Only the discounted items will have values in their totalDiscount fields, others will remain same.
		Example:
	Input: Cart has products [A, B, C] with prices [100, 200, 300], Coupon applies 20% discount to productId = B.
	Output:
	Total Price = 600
	Total Discount = 40 (on product B)
	Final Price = 560
	Cart Items: Product B has total_discount = 40, others = 0.
## BxGy Coupons
Logic:
	Identify Buy and Get Arrays with Quantities: Retrieve rows for the Buy Array (tblBuyProduct) and Get Array (tblGetProduct) for the given couponId.
	Sum the quantity values from the Buy Array to know the total required quantity (ex. Buy 3).
	Sum the quantity values from the Get Array to know the total free quantity (ex. Get 1).
	--Cart Validation: Cross check if cart items contain products from the Buy Array.
	If a product matches,ensure the quantity of matched products meets the required total quantity from the Buy Array.
		Example:
	Buy Array: [X (Qty 2), Y (Qty 1)] ? Total Buy = 3
	Cart: [X (Qty 2), Y (Qty 1)] ? Valid match.
	Cart: [X (Qty 1), Y (Qty 1)] ? Invalid, as total Buy = 2 (less than required).
	--Check Get Array:If the Buy condition is successfully checked check the same for products in the Get Array.
	
Repetition Limit: Calculate the number of repetitions occured based on the quantities in the Buy Array.
	Limit the free products to the repetition count multiplied by the Get Array's total quantity.
		Example:
	Coupon: Buy 3, Get 1 Free, Repetition Limit = 2
	Cart: [X (Qty 6), A (Qty 2)]
	Result: Get 2 free products (A and B if available).
	Distribute Discounts:
	
	Assign discounts to free products from the Get Array:
	The price of free products is subtracted as a discount.
	Update each applicable cart item with its respective discount.
	Example Scenarios:
	Scenario 1: Valid Application
	Coupon Details:
	Buy Array: [X (Qty 2), Y (Qty 1)]
	Get Array: [A (Qty 1)]
	Repetition Limit: 1
	Cart: [X (Qty 2), Y (Qty 1), A (Qty 1)]
	Process:
	Total Buy = 3, matches cart quantities.
	Total Get = 1, matches cart quantities.
	Result:
	A is free.
	Total discount = A.price.
	Scenario 2: Multiple Repetitions
	Coupon Details:
	Buy Array: [X (Qty 1), Y (Qty 2)]
	Get Array: [A (Qty 1)]
	Repetition Limit: 2
	Cart: [X (Qty 2), Y (Qty 4), A (Qty 2)]
	Process:
	Total Buy = 3, repeated twice (Buy Array = 6 matches cart).
	Total Get = 2 matches repetition limit.
	Result:
	A (Qty 2) is free.
	Total discount = Price of 2 A products.
	Scenario 3: Insufficient Buy Quantity
	Coupon Details:
	Buy Array: [X (Qty 2), Y (Qty 1)]
	Get Array: [A (Qty 1)]
	Repetition Limit: 1
	Cart: [X (Qty 1), Y (Qty 1), A (Qty 1)]
	Process:
	Total Buy = 2, does not meet required Buy Array quantity of 3.
	Result:
	Coupon is not applied.
	Scenario 4: Uneven Free Products
	Coupon Details:
	Buy Array: [X (Qty 1), Y (Qty 2)]
	Get Array: [A (Qty 1), B (Qty 1)]
	Repetition Limit: 3
	Cart: [X (Qty 3), Y (Qty 6), A (Qty 2), B (Qty 1)]
	Process:
	Total Buy = 9, allows 3 repetitions.
	Total Get = 3 (but only 2 free products available).
	Result:
	A and B are free.
	Total discount = Price of A + B.
	
**1. POST /coupons (Create Coupon)**
>
## Cart-wise Coupon Cases
	Assumptions
	 
		All discounts are percentage-based.	
		Threshold calculation includes all eligible cart items.	
		Default expiration period of 7 days from creation.
		Default status will be active.	
		Single coupon will be application per cart.	
		Cart total is calculated before any discounts.
	 
	Calculation
	 
		Threshold calculation happens before tax
		Gift cards are excluded from discount calculations
	 
	Limitations
	 
		Discount Constraints for a single product only
		Gift cards redemption should also be included along with discount calculations
		No support for flat amount discounts
		Cannot combine with other coupon types
		No user/user type specific discounts
		Cannot exclude specific products
	 
	Improvements
	 
		Discount Flexibility: Add support for flat amount discounts
		Add time-based discount variations
		Dynamic pricing based on cart composition
		First-time user special rates
		Creation/Adding/Collecting points on applying/purchasing
 
>	
## Product-wise Coupon Cases
	Assumptions
	 
		Single discount per product
		Percentage-based discounts only
		Valid product ID required
		Standard 7-day expiration
		Applies to individual product quantity
		No minimum quantity requirement
	 
	Limitations
	 
		No flat amount discounts
		No quantity-based scaling
		No bundle discounts
		Cannot combine with other discounts
		Single product discount only
	 
	Improvements
	 
		Add quantity-based added discounts
		Time-sensitive product deals
 
 
>	
## BxGy (BuyX Get Y) Coupon Cases
	Assumptions
	 
		Distinct buy and get product lists
		No mixing between buy and get lists
		Valid product IDs in both lists
		Standard 7-day expiration
		Repetition limit applies
		Get products are completely free and considered as discount and calculate as %
		Sequential application of repetitions
	 
	Limitations
	 
		Cannot mix products between lists
		No variable quantities in repetitions
		Fixed discount (100% off) for "get" products
		No Max Threshold Discount for Free products
	 
	Improvements
	 
		Dynamic product combinations
		Variable discounts on "get" products
		Flexible repetition structure
		Category-based BXGY
		Mixed category combinations
		Progressive discount on repetitions
	 
	 
	Edge Cases to Consider
	 
	Cart Modifications
	 
		Items removed after coupon application
		Quantity changes after application
		Price changes during active coupon
		Out-of-stock scenarios
		Handling Cart Expiration
		Handling Wishlisting 
		Handling Coupon Saving
	 
	Timing Issues
	 
		Coupon expiration during checkout
		Price changes during session
		Stock changes during application
		Concurrent coupon applications
	 
	Scalability Aspects
	 
		Handling multiple concurrent requests
		Large cart processing
		Bulk coupon applications
		Real-time price updates
	 
	Future Extensions
	 
	New Coupon Types
	 
		Time-based flash discounts
		Loyalty program integration
		Referral-based discounts
		Bundle deals
		Counting Type discount
	 
	Advanced Features that can be added
	 
		Analytics and reporting
		AI-powered discount suggestions
		Dynamic pricing optimization
		Personalized coupon generation
