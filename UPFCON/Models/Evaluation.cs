using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Evaluation
{
    public Guid Id { get; set; }
    
    [Required, MaxLength(255)] public required string Comment { get; set; }
    
    [Required, Range(1, 5)] public int ClarityScore { get; set; }
    
    [Required, Range(1, 5)] public int PertinenceScore { get; set; }
    
    [Required, Range(1, 5)] public int OriginalityScore { get; set; }
    
    [Required] public DateTime EvaluationDate { get; set; }
    
    // For composite key
    public Guid EventId { get; set; }
    public Guid EvaluatorId { get; set; }
    public CommitteeMember Evaluator { get; set; } = null!;
    public Guid PaperId { get; set; }
    public Paper Paper { get; set; } = null!;
}