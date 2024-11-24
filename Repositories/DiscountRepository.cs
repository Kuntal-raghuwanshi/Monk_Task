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
        Task<Cart> ApplyCoupon(int couponId, List<Items> cartItems);
        Task<bool> DeleteCoupon(int id);
        //Task<ResponseModel> UpdateCoupon(Coupons detail);
    }
    public class DiscountRepository : IDiscountRepository
    {
        private DataContext _context;

        public DiscountRepository(DataContext context)
        {
            _context = context;
        }
        //public IEnumerable<Cart> PopulatingData(GridReader result)
        //{
            // try
            //{
            //    var offeringsList = result.Read<Offering>().ToList();
            //    var locationsList = result.Read<OfferingsLocation>().ToList();
            //    var locationLookup = locationsList.GroupBy(x => x.OfferingsId).ToDictionary(g => g.Key, g => g.Select(x => new { x.OfferingsId, x.LocationId, x.Location }).ToList());

            //    foreach (var item in offeringsList)
            //    {
            //        // locations data Bind
            //        List<OfferingsLocation> offeringLocationsList = new List<OfferingsLocation>();
            //        if (locationLookup.TryGetValue(item.Id, out var loc))
            //        {
            //            offeringLocationsList = loc.Select(x => new OfferingsLocation
            //            {
            //                OfferingsId = item.Id,
            //                LocationId = x.LocationId,
            //                Location = x.Location
            //            }).ToList();
            //        }
            //        item.Location = offeringLocationsList;
            //    }
            //    return offeringsList;
            //}
            //catch (Exception ex)
            //{
            //    throw;
            //}

        //}
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
                EXEC dbo.sp_CreateCupons
                        
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
                DataTable buyProductsTable = Extensions.ConvertToTableSQL(discountcode.Details.BuyProducts);
                parameters.Add("@buyProducts", buyProductsTable.AsTableValuedParameter("dbo.type_BuyProducts"));
            }
            else
            {
                parameters.Add("@buyProducts", null);
            }

            if (discountcode.Details.GetProducts != null && discountcode.Details.GetProducts.Any())
            {
                DataTable getProductsTable = Extensions.ConvertToTableSQL(discountcode.Details.GetProducts);
                parameters.Add("@getProducts", getProductsTable.AsTableValuedParameter("dbo.type_GetProducts"));
            }
            else
            {
                parameters.Add("@getProducts", null);
            }
            return await connection.QuerySingleOrDefaultAsync<bool>(sql, parameters);
        }
        public async Task<IEnumerable<Coupons>> GetAllCoupons()
        {

            using var connection = _context.CreateConnection();
            using GridReader multi = await connection.QueryMultipleAsync("dbo.sp_getAllCoupons", commandType: CommandType.StoredProcedure);
            List<Coupons> coupons = (await multi.ReadAsync<Coupons>()).ToList();
            //var buyProducts = (await multi.ReadAsync<Items>()).ToList();
            //var getProducts = (await multi.ReadAsync<Items>()).ToList();
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
            if (coupon == null)
            {
                return null;
            }
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



        //public Task<ResponseModel> UpdateCoupon(Coupons detail)
        //{
        //    using var connection = _context.CreateConnection();
        //    var sql = @"
        //        EXEC dbo.UpdateDiscountCodes 
        //                @id = @id,
        //                @orgainizationsid = @orgainizationsid,
        //                @name = @name,
        //                @discountcode = @discountcode,
        //                @status = @status,
        //                @startdate = @startdate,
        //                @expirationdadte = @expirationdadte,
        //                @appliestospecificofferings = @appliestospecificofferings,
        //                @appliestospecifictype = @appliestospecifictype,
        //                @amount = @amount,
        //                @percentnumber = @percentnumber	
        //    ";
        //    return await connection.ExecuteAsync(sql, discountcode);
        //}
    }
}
