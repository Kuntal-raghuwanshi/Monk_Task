using Microsoft.AspNetCore.Mvc;
using Monk_Task.Models;
using Monk_Task.Service;
using System.Reflection;
using static Monk_Task.Helpers.Extensions;

namespace Monk_Task.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DiscountController : Controller
    {
        private IDiscountService _discountCodesService;
        private IHttpContextAccessor _httpContext;

        public DiscountController(IDiscountService discountCodesService, IHttpContextAccessor httpContext)
        {
            _discountCodesService = discountCodesService;
            _httpContext = httpContext;
        }

        [HttpPost("/coupons")]
        public async Task<IActionResult> CreateCoupons(Coupons model)
        {
            try
            {
                ResponseModel response = await _discountCodesService.Create(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    IsSuccess = false,
                    Message = "An error occurred while creating the coupon"
                });
            }
        }

        [HttpGet("/coupons")]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {
                ResponseModel response = await _discountCodesService.GetAllCoupons();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    IsSuccess = false,
                    Message = ErrorLogType.Error.GetDescription()
                }) ;
            }
        }
        [HttpGet("coupons/{id}")]
        public async Task<IActionResult> GetCouponById(int id)
        {
            try
            {
                ResponseModel response = await _discountCodesService.GetCouponById(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseModel
                {
                    IsSuccess = false,
                    Message = ErrorLogType.Error.GetDescription()
                });
            }
        }

        [HttpPost("/applicable-coupons")]
        public async Task<IActionResult> ApplicableCoupons(List<Items> cartItems)
        {
            ResponseModel response = await _discountCodesService.ApplicableCoupons(cartItems);
            return Ok(response);
        }

        [HttpPost("/apply-coupons/{couponId}")]
        public async Task<IActionResult> ApplyCoupon(int couponId, List<Items> cartItems)
        {
            ResponseModel response = await _discountCodesService.ApplyCoupon(couponId, cartItems);
            return Ok(response);
        }

        //[HttpPut("coupons/{id}")]
       // public async Task<IActionResult> UpdateCoupon(Coupons detail)
       // {
           // ResponseModel response = await _discountCodesService.UpdateCoupon(detail);
           // return Ok(response);
        //}

        [HttpDelete("coupons/{id}")]
        public async Task<IActionResult> DeleteCoupon(int id)
        {
            try
            {
                ResponseModel response = await _discountCodesService.DeleteCoupon(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = "Failed in Deleting" + ex.Message });
            }
        }
    }
}
