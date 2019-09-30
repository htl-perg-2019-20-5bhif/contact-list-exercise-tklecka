using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace contact_list_exercise.Controllers
{

    public class Person
    {
        [Required]
        public int id { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string firstName { get; set; }

        [MinLength(1)]
        [MaxLength(50)]
        public string lastName { get; set; }

        [Required]
        [MinLength(2)]
        [EmailAddress]
        public string email { get; set; }
    }

    [ApiController]
    [Route("contacts")]

    public class AddressBookController : ControllerBase
    {
        private static readonly Dictionary<int, Person> items = new Dictionary<int, Person> {
            {0, new Person { id = 0, firstName = "Tim", lastName = "Klecka", email = @"tim@klecka.at" } },
            {1, new Person { id = 1, firstName = "Markus", lastName = "Kurzmann", email = @"markus@opendrone.at" } }
        };

        [HttpGet]
        public IActionResult GetAllItems()
        {
            return Ok(items.Values);
        }

        [HttpGet]
        [Route("{index}", Name = "GetSpecificItem")]
        public IActionResult GetItem(int index)
        {
            if (items.GetValueOrDefault(index) != null)
            {
                return Ok(items.GetValueOrDefault(index));
            }
            else { return BadRequest("😬 "+"Invalid index"); }
        }

        [HttpGet]
        [Route("findByName", Name = "GetSpecificItemName")]
        public IActionResult GetItembyName([FromQuery] String nameFilter)
        {
            List<Person> filtereditems = new List<Person>();
            if (nameFilter == null)
            {
                return BadRequest("😬 "+"Invalid or missing name"+ nameFilter);
            }
            foreach (KeyValuePair<int, Person> entry in items)
            {
                if (entry.Value.firstName.ToLower().Contains(nameFilter.ToLower()) || entry.Value.lastName.ToLower().Contains(nameFilter.ToLower()))
                {
                    filtereditems.Add(entry.Value);
                }
            }
            if (filtereditems.Count > 0)
            {
                return Ok(filtereditems);
            }
            else
            {
                return NotFound("🤔");
            }
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] Person newItem)
        {
            if (ModelState.IsValid)
            {
                items.TryAdd(newItem.id, newItem);
                return Created("" + newItem.id, newItem);
            }
            return BadRequest("😬 "+ModelState);
        }

        [HttpDelete]
        [Route("{index}")]
        public IActionResult DeleteItem(int index)
        {
            if (index < 0)
            {
                return BadRequest("😬 "+"Invalid ID supplied");
            }
            else if (items.GetValueOrDefault(index) != null)
            {
                items.Remove(index);
                return NoContent();
            }
            else
            {
                return NotFound("🤔 "+"Person Not Found");
            }
        }
    }
}
