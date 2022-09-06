using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Core.Interfaces;
using Core.Specifications;
using API.Dtos;
using AutoMapper;
using API.Errors;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _productsRepos;
        private readonly IGenericRepository<ProductBrand> _productBrandRepos;
        private readonly IGenericRepository<ProductType> _productTypeRepos;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> productsRepos,
                                  IGenericRepository<ProductBrand> productBrandRepos,
                                  IGenericRepository<ProductType> productTypeRepos,
                                  IMapper mapper)
        {
            _productsRepos = productsRepos;
            _productBrandRepos = productBrandRepos;
            _productTypeRepos = productTypeRepos;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ProductDto>>> GetProducts()
        {
            var spec = new ProductsWithBrandsAndTypesSpecification();
            var products = await _productsRepos.ListAsync(spec);
            return Ok(_mapper
                .Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetProduct(int id)
        {
            var spec = new ProductsWithBrandsAndTypesSpecification(id);
            var product = await _productsRepos.GetEntityWithSpec(spec);
            if (product == null) return NotFound(new ApiResponse(404));
            return _mapper.Map<Product, ProductDto>(product);
        }

        [HttpGet("brands")]
        public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetProductBrands()
        {
            return Ok(await _productBrandRepos.ListAllAsync());
        }

        [HttpGet("types")]
        public async Task<ActionResult<IReadOnlyList<ProductType>>> GetProductTypes()
        {
            return Ok(await _productTypeRepos.ListAllAsync());
        }
    }
}