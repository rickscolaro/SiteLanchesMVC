using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanchesMac.Models
{
    [Table("CarrinhoCompraItens")]
    public class CarrinhoCompraItem
    {
        public int CarrinhoCompraItemId { get; set; }// Ja mapeia como uma chave primaria Identity
        public int Quantidade { get; set; }

        [StringLength(200)]
        public string CarrinhoCompraId { get; set; }
        public Lanche Lanche { get; set; } // O Entity Relaciona com a tabela Lanche criando uma chave estrangeira
    }
}
