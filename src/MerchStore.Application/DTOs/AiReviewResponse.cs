using System;
using System.Collections.Generic;

namespace MerchStore.Application.DTOs
{
    public class AiReviewResponse
    {
        public string ProductId { get; set; } = string.Empty;
        public AiReviewStats Stats { get; set; } = new();
        public List<AiSingleReview> Reviews { get; set; } = new();
    }

    public class AiReviewStats
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double CurrentAverage { get; set; }
        public int TotalReviews { get; set; }
        public DateTime LastReviewDate { get; set; }
    }

    public class AiSingleReview
    {
        public DateTime Date { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}
