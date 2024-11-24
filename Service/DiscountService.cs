using Azure;
using Microsoft.AspNetCore.Http.HttpResults;
using Monk_Task.Models;
using Monk_Task.Repositories;
using System.Collections.Generic;
using static Monk_Task.Helpers.Extensions;

namespace Monk_Task.Service
{
    public interface IDiscountService
    {
        Task<ResponseModel> ApplicableCoupons(List<Items> cartItems);
        Task<ResponseModel> ApplyCoupon(int couponId, List<Items> cartItems);
        Task<ResponseModel> Create(Coupons discountcode);
        Task<ResponseModel> DeleteCoupon(int id);
        Task<ResponseModel> GetAllCoupons();
        Task<ResponseModel> GetCouponById(int id);
        //Task<ResponseModel> UpdateCoupon(Coupons detail);
    }
    public class DiscountService : IDiscountService
    {
        private IDiscountRepository _discountRepository;

        public DiscountService(IDiscountRepository discountRepository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<ResponseModel> Create(Coupons discountcode)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                bool isCreated = await _discountRepository.Create(discountcode);
                response.IsSuccess = isCreated;
                response.Message = isCreated ? SuccessLogType.Create.GetDescription() : ErrorLogType.NotCreated.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel> GetAllCoupons()
        {
            ResponseModel response = new ResponseModel();
            try
            {
                IEnumerable<Coupons> allCoupons = await _discountRepository.GetAllCoupons();
                if (allCoupons != null && allCoupons.Any())
                {

                    response.IsSuccess = true;
                    response.Data = allCoupons;
                    return response;

                }
                response.IsSuccess = false;
                response.Message = SuccessLogType.NoData.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel> GetCouponById(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Coupons coupons = await _discountRepository.GetCouponById(id);
                if (coupons != null)
                {
                    response.IsSuccess = true;
                    response.Data = coupons;
                    return response;
                }
                response.IsSuccess = false;
                response.Message = SuccessLogType.NoData.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }
        public async Task<ResponseModel> ApplicableCoupons(List<Items> ApplicableCoupons)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                IEnumerable<ApplicableCoupons> coupons = await _discountRepository.ApplicableCoupons(ApplicableCoupons);
                if (coupons != null && coupons.Any())
                {
                    response.IsSuccess = true;
                    response.Data = coupons;
                    return response;
                }
                response.IsSuccess = false;
                response.Message = SuccessLogType.NoData.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }

        public async Task<ResponseModel> ApplyCoupon(int couponId, List<Items> cartItems)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                Cart coupons = await _discountRepository.ApplyCoupon(couponId, cartItems);
                if (coupons != null)
                {
                    response.IsSuccess = true;
                    response.Data = coupons;
                    return response;
                }
                response.IsSuccess = false;
                response.Message = SuccessLogType.Invalid.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }

        //public async Task<ResponseModel> UpdateCoupon(Coupons detail)
        //{
        //    return await _discountRepository.UpdateCoupon(detail);
        //}
        public async Task<ResponseModel> DeleteCoupon(int id)
        {
            ResponseModel response = new ResponseModel();
            try
            {
                bool isDeleted = await _discountRepository.DeleteCoupon(id);
                if (isDeleted != null)
                {
                    response.IsSuccess = true;
                    response.Message = SuccessLogType.Delete.GetDescription();
                    return response;
                }
                response.IsSuccess = false;
                response.Message = SuccessLogType.NoData.GetDescription();
                return response;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ErrorLogType.Error.GetDescription(); //+ ex.Message;
                return response;
            }
        }

    }
}
