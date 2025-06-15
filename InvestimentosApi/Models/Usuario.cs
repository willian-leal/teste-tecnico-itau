using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosApi.Models;

public class Usuario
{
    [Key]
    public int Id { get; set; }

    [Column("nome")]
    public string Nome { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("percentual_corretagem")]
    public decimal PercentualCorretagem { get; set; }
    public ICollection<Operacao>? Operacoes { get; set; }
    public ICollection<Posicao>? Posicoes { get; set; }
}