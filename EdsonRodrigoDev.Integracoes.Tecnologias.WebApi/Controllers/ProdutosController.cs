using EdsonRodrigoDev.Integracoes.Tecnologias.Application.Services;
using EdsonRodrigoDev.Integracoes.Tecnologias.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EdsonRodrigoDev.Integracoes.Tecnologias.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly ProdutoAppServices _produtoAppServices;

        public ProdutosController(ProdutoAppServices produtoAppServices)
        {
            _produtoAppServices = produtoAppServices;
        }

        [HttpPost("novoproduto")]
        public async Task<IActionResult> CadastrarProduto(Produtos produto)
        {
            await _produtoAppServices.CriarProduto(produto);
            return Ok(produto);
        }
    }
}
