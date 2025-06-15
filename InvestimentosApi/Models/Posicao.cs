using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosApi.Models;

public class  Posicao
{
    [Key]
    public int Id { get; set; }
    
    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("ativo_id")]
    public int AtivoId { get; set; }

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_medio")]
    public decimal PrecoMedio { get; set; }

    [Column("pnl")]
    public decimal PnL { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }

    [ForeignKey(nameof(AtivoId))]
    public Ativo? Ativo { get; set; }
}