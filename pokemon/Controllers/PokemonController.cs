using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using pokemon.Dto;
using pokemon.Interfaces;
using pokemon.Models;
using pokemon.Repository;

namespace pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;

        public PokemonController(IPokemonRepository pokemonRepository, IMapper mapper, IOwnerRepository ownerRepository, IReviewRepository reviewRepository)
        {
            _pokemonRepository = pokemonRepository;
            _mapper = mapper;
            _ownerRepository = ownerRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemons()
        {
            var pokemons = _mapper.Map<List<PokemonDTO>>(_pokemonRepository.GetPokemons());

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
              return NotFound();
            var pokemon = _mapper.Map<PokemonDTO>(_pokemonRepository.GetPokemon(pokeId));

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof (decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if(!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            var rating = _pokemonRepository.GetPokemonRating(pokeId);
            
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(rating);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateCountry([FromQuery] int ownerId, [FromQuery] int categoryId, [FromBody] PokemonDTO pokemonCreate)
        {
            if (pokemonCreate == null)
            {
                return BadRequest();
            }

            var pokemons = _pokemonRepository.GetPokemons()
                .Where(o => o.Name.Trim().ToUpper() == pokemonCreate.Name.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon allready exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);

            if (!_pokemonRepository.CreatePokemon(ownerId, categoryId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Succesfully created");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateOwner(int pokeId, [FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDTO updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokemonMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while updating category");
                return StatusCode(500, ModelState);
            }

            return Ok("Succes");
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
            {
                return NotFound();
            }

            var reviewsToDelete = _reviewRepository.GetReviewsOfAPokemon(pokeId);
            var pokemonToDelete = _pokemonRepository.GetPokemon(pokeId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReviews(reviewsToDelete.ToList()))
            {
                ModelState.AddModelError("", "Something went wrong while deleting reviews");
            }

            if (!_pokemonRepository.DeletePokemon(pokemonToDelete))
            {
                ModelState.AddModelError("", "Something went wrong while deleting pokemon");
            }

            return NoContent();
        }
    }
}

