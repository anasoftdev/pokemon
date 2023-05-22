using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using pokemon.Controllers;
using pokemon.Dto;
using pokemon.Interfaces;
using pokemon.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pokemon.Tests.Controller
{
    public class PokemonControllerTests
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        private readonly IOwnerRepository _ownerRepository;
        private readonly PokemonController _pokemonController;

        public PokemonControllerTests()
        {
            _pokemonRepository = A.Fake<IPokemonRepository>();
            _reviewRepository = A.Fake<IReviewRepository>();
            _mapper = A.Fake<IMapper>();
            _ownerRepository = A.Fake<IOwnerRepository>();

            _pokemonController = new PokemonController(_pokemonRepository, _mapper, _ownerRepository, _reviewRepository);
        }

        [Fact]
        public void PokemonController_GetPokemons_ReturnOk()
        {
            //Arrange
            var pokemons = A.Fake<ICollection<PokemonDTO>>();
            var pokemonList = A.Fake<List<PokemonDTO>>();
            A.CallTo(() => _mapper.Map<List<PokemonDTO>>(pokemons)).Returns(pokemonList);
            var controller = _pokemonController;

            //Act
            var result = controller.GetPokemons();

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void PokemonController_CreatePokemon_ReturnOk()
        {

        }
    }
}
