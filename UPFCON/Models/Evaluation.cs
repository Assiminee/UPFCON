using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UPFCON.Models;

public class Evaluation
{
    [Key] public Guid Id { get; set; }
    
    [Required, MaxLength(255)] public string Comment { get; set; }
    
    [Required, Range(1, 5)] public int ClarityScore { get; set; }
    
    [Required, Range(1, 5)] public int PertinenceScore { get; set; }
    
    [Required, Range(1, 5)] public int OriginalityScore { get; set; }
    
    [Required] public DateTime EvaluationDate { get; set; }
    
    public Guid EvaluatorId { get; set; }
    [ForeignKey(nameof(EvaluatorId))] public CommitteeMember Evaluator { get; set; } = null!;
    
    public Guid PaperId { get; set; }
    [ForeignKey(nameof(PaperId))] public Paper Paper { get; set; } = null!;
}