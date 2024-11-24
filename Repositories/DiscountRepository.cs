using Dapper;
using Monk_Task.Helpers;
using Monk_Task.Models;
using System.Data;
using static Azure.Core.HttpHeader;
using static Dapper.SqlMapper;

namespace Monk_Task.Repositories
{
    public interface IDiscountRepository
    {
        Task<bool> Create(Coupons discountcode);
        Task<IEnumerable<Coupons>> GetAllCoupons();
        Task<Coupons> GetCouponById(int id);
        Task<IEnumerable<ApplicableCoupons>> ApplicableCoupons(List<Items> cartItems);
        Task<bool> DeleteCoupon(int id);
    }
    public class DiscountRepository : IDiscountRepository
    {
        private DataContext _context;

        public DiscountRepository(DataContext context)
        {
            _context = context;
        }
       
        public async Task<IEnumerable<ApplicableCoupons>> ApplicableCoupons(List<Items> cartItems)
        {
            using var connection = _context.CreateConnection();
            var sql = @"EXEC dbo.GetApplicableCoupons";
            return await connection.QueryAsync<ApplicableCoupons>(sql);
        }

        public async Task<Cart> ApplyCoupon(int couponId, List<Items> cartItems)
        {
            using var connection = _context.CreateConnection();
            var sql = """
            EXEC dbo.ApplyCoupon @couponId = @couponId,@cartItems=@cartItems
        """;
            var parameters = new DynamicParameters();
            DataTable cartDetailsTable = Extensions.ConvertToTableSQL(cartItems);
            parameters.Add("@couponId", couponId);
            parameters.Add("@cartItems", cartDetailsTable.AsTableValuedParameter("dbo."));

            return await connection.QuerySingleOrDefaultAsync<Cart>(sql, parameters);

        }

        public async Task<bool> Create(Coupons discountcode)
        {
            using var connection = _context.CreateConnection();
            var sql = @"
                 EXEC dbo.sp_CreateCoupons
                 @type = @type,
                 @expiresOn = @expiresOn,
                 @productId = @productId,
                 @threshold = @threshold,
                 @discount = @discount,
                 @repitionLimit = @repitionLimit,
                 @buyProducts = @buyProducts,
                 @getProducts = @getProducts,
                 @result = @result OUTPUT;  -- Capture the output result
                 ";

            var parameters = new DynamicParameters();
            parameters.Add("@type", discountcode.Type);
            parameters.Add("@expiresOn", DateTime.UtcNow.AddDays(7)); 
            parameters.Add("@productId", discountcode.Details.ProductId);
            parameters.Add("@threshold", discountcode.Details.Threshold);
            parameters.Add("@discount", discountcode.Details.Discount);
            parameters.Add("@repitionLimit", discountcode.Details.RepitionLimit);

            if (discountcode.Details.BuyProducts != null && discountcode.Details.BuyProducts.Any())
            {
                DataTable buyProductsTable = new DataTable();
                buyProductsTable.Columns.Add("ProductId", typeof(int));    
                buyProductsTable.Columns.Add("Quantity", typeof(int));   
                buyProductsTable.Columns.Add("Price", typeof(decimal));   
                buyProductsTable.Columns.Add("TotalDiscount", typeof(decimal));  

                foreach (var item in discountcode.Details.BuyProducts)
                {
                    buyProductsTable.Rows.Add(item.ProductId ?? 0, item.Quantity ?? 0, item.Price ?? 0, item.TotalDiscount ?? 0);
                }

                parameters.Add("@buyProducts", buyProductsTable.AsTableValuedParameter("dbo.type_BuyProducts"));
            }
            else
            {
                DataTable emptyBuyProductsTable = new DataTable();
                emptyBuyProductsTable.Columns.Add("ProductId", typeof(int));   
                emptyBuyProductsTable.Columns.Add("Quantity", typeof(int));     
                emptyBuyProductsTable.Columns.Add("Price", typeof(decimal));   
                emptyBuyProductsTable.Columns.Add("TotalDiscount", typeof(decimal));

                parameters.Add("@buyProducts", emptyBuyProductsTable.AsTableValuedParameter("dbo.type_BuyProducts"));
            }

            if (discountcode.Details.GetProducts != null && discountcode.Details.GetProducts.Any())
            {
                DataTable getProductsTable = new DataTable();
                getProductsTable.Columns.Add("ProductId", typeof(int));   
                getProductsTable.Columns.Add("Quantity", typeof(int));   
                getProductsTable.Columns.Add("Price", typeof(decimal));
                getProductsTable.Columns.Add("TotalDiscount", typeof(decimal));

                foreach (var item in discountcode.Details.GetProducts)
                {
                    getProductsTable.Rows.Add(item.ProductId ?? 0, item.Quantity ?? 0, item.Price ?? 0, item.TotalDiscount ?? 0);
                }

                parameters.Add("@getProducts", getProductsTable.AsTableValuedParameter("dbo.type_GetProducts"));
            }
            else
            {
                DataTable emptyGetProductsTable = new DataTable();
                emptyGetProductsTable.Columns.Add("ProductId", typeof(int));    
                emptyGetProductsTable.Columns.Add("Quantity", typeof(int));    
                emptyGetProductsTable.Columns.Add("Price", typeof(decimal));  
                emptyGetProductsTable.Columns.Add("TotalDiscount", typeof(decimal));  

                parameters.Add("@getProducts", emptyGetProductsTable.AsTableValuedParameter("dbo.type_GetProducts"));
            }

            try
            {
                parameters.Add("@result", dbType: DbType.Int32, direction: ParameterDirection.Output);
                await connection.ExecuteAsync(sql, parameters);
                var result = parameters.Get<int>("@result");
                return result == 1;  
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task<IEnumerable<Coupons>> GetAllCoupons()
        {

            using var connection = _context.CreateConnection();
            using GridReader multi = await connection.QueryMultipleAsync("dbo.sp_getAllCoupons", commandType: CommandType.StoredProcedure);
            List<Coupons> coupons = (await multi.ReadAsync<Coupons>()).ToList();
            var buyProductsByCoupon = (await multi.ReadAsync<Items>()).ToList().GroupBy(bp => bp.CouponId).ToDictionary(g => g.Key, g => g.ToList());
            var getProductsByCoupon = (await multi.ReadAsync<Items>()).ToList().GroupBy(gp => gp.CouponId).ToDictionary(g => g.Key, g => g.ToList());
            foreach (var coupon in coupons)
            {
                coupon.Details = new CouponDetails
                {
                    CouponId = coupon.Id,
                    Threshold = coupon.Details.Threshold,
                    Discount = coupon.Details.Discount,
                    ProductId = coupon.Details.ProductId,
                    RepitionLimit = coupon.Details.RepitionLimit,
                    BuyProducts = buyProductsByCoupon.TryGetValue(coupon.Id, out var bp) ? bp : new List<Items>(),
                    GetProducts = getProductsByCoupon.TryGetValue(coupon.Id, out var gp) ? gp : new List<Items>()
                };
            }

            return coupons;
        }
        public async Task<Coupons> GetCouponById(int couponId)
        {
            using var connection = _context.CreateConnection();
            using var multi = await connection.QueryMultipleAsync(
                "dbo.sp_getCouponById",
                new { CouponId = couponId },
                commandType: CommandType.StoredProcedure);

            Coupons coupon = await multi.ReadSingleOrDefaultAsync<Coupons>();
            List<Items> buyProducts = (await multi.ReadAsync<Items>()).ToList();
            List<Items> getProducts = (await multi.ReadAsync<Items>()).ToList();
            coupon.Details = new CouponDetails
            {
                CouponId = coupon.Id,
                Threshold = coupon.Details?.Threshold,
                Discount = coupon.Details?.Discount,
                ProductId = coupon.Details?.ProductId,
                RepitionLimit = coupon.Details?.RepitionLimit,
                BuyProducts = buyProducts,
                GetProducts = getProducts
            };
            return coupon;
        }

        public async Task<bool> DeleteCoupon(int id)
        {
            using var connection = _context.CreateConnection();
            var sql = @"EXEC dbo.sp_DeleteCoupon @id=@id";
            return await connection.QuerySingleAsync<bool>(sql, new { id });
        }

    }
}
