using System;
using Microsoft.AspNetCore.Mvc;
using Readible.Shared;
using static Readible.Migrations.MigrationConst;
using static Readible.Shared.HttpStatus;

namespace Readible.Migrations
{
    [Route("migration")]
    [ApiController]
    public class MigrationController : ControllerBase
    {
        [HttpGet("run")]
        public IActionResult Migration([FromServices] MigrationService service)
        {
            try
            {
                var db = service.Migration();
                return Ok(db);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("seed")]
        public IActionResult SeedUsers([FromServices] MigrationService service)
        {
            try
            {
                var users = service.SeedUsers(SEEDER_ADMIN, SEEDER_MANAGER, SEEDER_CUSTOMER);
                service.SeedBooks(SEEDER_BOOK_CATEGORY, SEEDER_BOOK);
                service.SeedBookComments(SEEDER_BOOK_COMMENT);
                service.SeedOrders(SEEDER_ORDER, SEEDER_MULTIPLIER);
                return Ok(users);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("seed/users")]
        public IActionResult SeedUsers([FromServices] MigrationService service,
            [FromQuery(Name = QUERY_SEED_ADMIN)] string inputAdmin,
            [FromQuery(Name = QUERY_SEED_MANAGER)] string inputManager,
            [FromQuery(Name = QUERY_SEED_CUSTOMER)]
            string inputCustomer)
        {
            try
            {
                var totalAdmin = Common.IntParse(inputAdmin, SEEDER_ADMIN);
                var totalManager = Common.IntParse(inputManager, SEEDER_MANAGER);
                var totalCustomer = Common.IntParse(inputCustomer, SEEDER_CUSTOMER);
                if (totalAdmin < 1 || totalManager < 1 || totalCustomer < 1) return BadRequest(BAD_REQUEST);

                var list = service.SeedUsers(totalAdmin, totalManager, totalCustomer);
                return Ok(list);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("seed/books")]
        public IActionResult SeedBooks([FromServices] MigrationService service,
            [FromQuery(Name = QUERY_SEED_CATEGORY)] string inputCategory,
            [FromQuery(Name = QUERY_SEED_BOOK)] string inputBook)
        {
            try
            {
                var totalCategory = Common.IntParse(inputCategory, SEEDER_BOOK_CATEGORY);
                var totalBook = Common.IntParse(inputBook, SEEDER_BOOK);
                if (totalCategory < 1 || totalBook < 1) return BadRequest(BAD_REQUEST);

                var list = service.SeedBooks(totalCategory, totalBook);
                return Ok(list);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("seed/book-comments")]
        public IActionResult SeedBooks([FromServices] MigrationService service,
            [FromQuery(Name = QUERY_BOOK_COMMENT)] string inputComment)
        {
            try
            {
                var totalComment = Common.IntParse(inputComment, SEEDER_BOOK_COMMENT);
                var list = service.SeedBookComments(totalComment);
                return Ok(list);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("seed/orders")]
        public IActionResult SeedOrders([FromServices] MigrationService service,
            [FromQuery(Name = QUERY_SEED_ORDER)] string inputOrder,
            [FromQuery(Name = QUERY_SEED_MULTIPLIER)] string inputMulti)
        {
            try
            {
                var totalOrder = Common.IntParse(inputOrder, SEEDER_ORDER);
                var multiplier = Common.IntParse(inputMulti, SEEDER_MULTIPLIER);
                var list = service.SeedOrders(totalOrder, multiplier);
                return Ok(list);
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }
    }
}