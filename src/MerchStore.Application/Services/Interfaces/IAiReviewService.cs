using System;
using System.Threading.Tasks;
using MerchStore.Application.DTOs;

public interface IAiReviewService
{
    Task<AiReviewResponse?> GetReviewAsync(Guid productId);
}