using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosApi.Models;

public class Operacao
{
    [Key]
    public int Id { get; set; }

    [Column("usuario_id")]
    public int UsuarioId { get; set; }

    [Column("ativo_id")]
    public int AtivoId { get; set; }

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("tipo_operacao")]
    public string TipoOperacao { get; set; } = string.Empty;

    [Column("corretagem")]
    public decimal Corretagem { get; set; }

    [Column("data_hora")]
    public DateTime DataHora { get; set; }

    [ForeignKey(nameof(UsuarioId))]
    public Usuario? Usuario { get; set; }
    [ForeignKey(nameof(AtivoId))]
    public Ativo? Ativo { get; set; }
}