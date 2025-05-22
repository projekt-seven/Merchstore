using System;
using System.Collections.Generic;

public class AiReviewResponse
{
    public string ProductId { get; set; }
    public AiReviewStats Stats { get; set; }
    public List<AiSingleReview> Reviews { get; set; }
}

public class AiReviewStats
{
    public string ProductId { get; set; }
    public string ProductName { get; set; }
    public double CurrentAverage { get; set; }
    public int TotalReviews { get; set; }
    public DateTime LastReviewDate { get; set; }
}

public class AiSingleReview
{
    public DateTime Date { get; set; }
    public string Name { get; set; }
    public int Rating { get; set; }
    public string Text { get; set; }
}