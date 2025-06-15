using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosApi.Models;

public class Ativo
{
    [Key]
    public int Id { get; set; }

    [Column("codigo")]
    public string Codigo { get; set; }

    [Column("nome")]
    public string Nome { get; set; }
    public ICollection<Operacao>? Operacoes { get; set; }
    public ICollection<Cotacao>? Cotacoes { get; set; }
    public ICollection<Posicao>? Posicoes { get; set; }
}