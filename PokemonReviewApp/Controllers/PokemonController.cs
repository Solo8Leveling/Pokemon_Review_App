﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.DTO;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models; //route atributu yazilanda hemise bunu sec

namespace PokemonReviewApp.Controllers
{

    //asagidakilar atribut adlanir
    [Route("api/[controller]")]
    [ApiController]

    public class PokemonController : Controller
    {
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMapper _mapper;
        //repository almaq ucun asagidaki PokemonController method'unu yaratdim
        public PokemonController(IPokemonRepository pokemonRepository,IReviewRepository reviewRepository,IMapper mapper)
        {
            _pokemonRepository = pokemonRepository;
            _reviewRepository = reviewRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Pokemon>))]        
        public IActionResult GetPokemons()
        {   //asagidaki code ucun list daxilinde PokemonDto yazmammisdim deye error yedim, cunki burda bir nece verilen gosterir
            var pokemons = _mapper.Map<List<PokemonDto>>(_pokemonRepository.GetPokemons());//bu PokemonRepository'deki GetPokemons'du
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(pokemons);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200,Type = typeof(Pokemon))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();
            //automapper ile yazdigimiza gore null olan hec bir seyi ekranda yazdirmir, sadece verilenleri goruruk
            var pokemon = _mapper.Map<PokemonDto>(_pokemonRepository.GetPokemon(pokeId));//GetPokemon Interface'de verdiyimizdi

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type = typeof(decimal))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonRating(int pokeId)
        {
            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            var rating = _pokemonRepository.GetPokemonRating(pokeId);

            if (!ModelState.IsValid)
                return BadRequest();

            return Ok(rating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreatePokemon([FromQuery] int ownerId, [FromQuery] int catId, [FromBody] PokemonDto pokemonCreate)
        {
            if (pokemonCreate == null)
                return BadRequest(ModelState);
            var pokemons = _pokemonRepository.GetPokemons()
                           .Where(c => c.Name.Trim().ToUpper() == pokemonCreate.Name.Trim().ToUpper())
                           .FirstOrDefault();

            if (pokemons != null)
            {
                ModelState.AddModelError("", "Pokemon already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            var pokemonMap = _mapper.Map<Pokemon>(pokemonCreate);
            

            if (!_pokemonRepository.CreatePokemon(ownerId, catId, pokemonMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
            }
            return Ok("Succesfully created");//bu return yuxaridaki if islemediyi zaman isleyir yeni else'idi
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult UpdatePokemon(int pokeId,  //demek burda verilen ile yuxarida yazilan HttpPut eyni olamlidi ki, bizden iki basqa sey sorusmasin
                                            [FromQuery] int ownerId,
                                            [FromQuery] int catId,
                                            [FromBody] PokemonDto updatedPokemon)
        {
            if (updatedPokemon == null)
                return BadRequest(ModelState);

            if (pokeId != updatedPokemon.Id)
                return BadRequest(ModelState);

            if (!_pokemonRepository.PokemonExists(pokeId))
                return NotFound();

            if (!ModelState.IsValid)
                return BadRequest();

            var pokiMap = _mapper.Map<Pokemon>(updatedPokemon);

            if (!_pokemonRepository.UpdatePokemon(ownerId, catId, pokiMap))
            {
                ModelState.AddModelError("", "Something wnet wrong updating owner");
                return StatusCode(500, ModelState);
            }
            return Ok("Succesfully updated");
        }

        [HttpDelete("{pokemonId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeletePokemon(int pokemonId)
        {
            if (!_pokemonRepository.PokemonExists(pokemonId))
                return NotFound();
            var reviewsDelete = _reviewRepository.GetReviewsOfAPokemon(pokemonId);
            var pokemonDelete = _pokemonRepository.GetPokemon(pokemonId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_reviewRepository.DeleteReviews(reviewsDelete.ToList()))
            {
                ModelState.AddModelError("", "something went wrong when deleting reviews");
            }

            if (!_pokemonRepository.DeletePokemon(pokemonDelete))
            {
                ModelState.AddModelError("", "something went wrong deleting pokemon");
            }
            return Ok("Deleting Succesfully");
        }
    }
}
