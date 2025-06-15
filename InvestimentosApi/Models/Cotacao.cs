using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosApi.Models;

public class Cotacao
{
    [Key]
    public int Id { get; set; }
    
    [Column("ativo_id")]
    public int AtivoId { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("data_hora")]
    public DateTime DataHora { get; set; }

    [ForeignKey(nameof(AtivoId))]
    public Ativo? Ativo { get; set; }
}