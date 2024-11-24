using Microsoft.AspNetCore.Mvc;
using Monk_Task.Models;
using Monk_Task.Service;
using System.Reflection;
using static Monk_Task.Helpers.Extensions;

namespace Monk_Task.Controllers
{
    [ApiController]
    [Route("coupons")]
    public class CouponsController : Controller
    {
        private IDiscountService _discountCodesService;
        private IHttpContextAccessor _httpContext;

        public CouponsController(IDiscountService discountCodesService, IHttpContextAccessor httpContext)
        {
            _discountCodesService = discountCodesService;
            _httpContext = httpContext;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateCoupons(Coupons model)
        {
            try
            {
                ResponseModel response = await _discountCodesService.Create(model);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("")]
        public async Task<IActionResult> GetAllCoupons()
        {
            try
            {
                ResponseModel response = await _discountCodesService.GetAllCoupons();
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ResponseModel
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
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


        [HttpDelete("{id}")]
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

        [HttpPost("/applicable-coupons")]
        public async Task<IActionResult> ApplicableCoupons(IEnumerable<Items> cartItems)
        {
            ResponseModel response = await _discountCodesService.ApplicableCoupons(cartItems.ToList());
            return Ok(response);
        }

    }
}
