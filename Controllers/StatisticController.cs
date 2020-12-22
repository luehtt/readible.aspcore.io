using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Readible.Services;
using Readible.Shared;
using static Readible.Shared.HttpStatus;
using static Readible.Shared.Const;
using static Readible.Shared.RouteConst;

namespace Readible.Controllers
{
    [Route(STATISTIC)]
    [Authorize(Roles = USER_ROLE_ADMIN)]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        [HttpGet("orders")]
        public async Task<IActionResult> SummarizeOrder([FromServices] OrderService service,
            [FromQuery(Name = QUERY_FROM_DATE)] string inputFromDate,
            [FromQuery(Name = QUERY_TO_DATE)] string inputToDate,
            [FromQuery(Name = QUERY_REFERENCE)] string inputReference
        )
        {
            try
            {
                var fromDate = DateTime.Parse(inputFromDate);
                var toDate = DateTime.Parse(inputToDate);
                if (string.IsNullOrEmpty(inputReference)) inputReference = QUERY_DATE;

                var data = await service.SummarizeOrder(fromDate, toDate, inputReference);
                return Ok(data);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }
        
        [HttpGet("orders-timestamp")]
        public async Task<IActionResult> SummarizeOrderTimestamp([FromServices] OrderService service)
        {
            try
            {
                var latest = service.GetLatestOrder();
                var oldest = service.GetOldestOrder();

                await Task.WhenAll(latest, oldest);
                var item = new { latestOrder = await latest, oldestOrder = await oldest };
                return Ok(item);
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }

        [HttpGet("customers")]
        public async Task<IActionResult> AnalyzeCustomers([FromServices] CustomerService service,
            [FromQuery(Name = QUERY_FROM_DATE)] string inputFromDate,
            [FromQuery(Name = QUERY_TO_DATE)] string inputToDate,
            [FromQuery(Name = QUERY_REFERENCE)] string inputReference
        )
        {
            try
            {
                var fromDate = DateTime.Parse(inputFromDate);
                var toDate = DateTime.Parse(inputToDate);

                switch (inputReference)
                {
                    case QUERY_AGE: return Ok(await service.SummarizeAge(fromDate, toDate));
                    case QUERY_GENDER: return Ok(await service.SummarizeGender(fromDate, toDate));
                    default: return StatusCode(BAD_REQUEST_CODE, BAD_REQUEST);
                }
            }
            catch (HttpResponseException err)
            {
                return err.ToResponse();
            }
            catch (Exception err)
            {
                return StatusCode(SERVER_ERROR_CODE, err.Message);
            }
        }
    }
}