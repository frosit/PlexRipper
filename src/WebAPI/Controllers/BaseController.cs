﻿using System;
using System.Net.Mime;
using AutoMapper;
using FluentResults;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlexRipper.Application.Common;
using PlexRipper.Domain;
using PlexRipper.WebAPI.Common.FluentResult;

namespace PlexRipper.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces(MediaTypeNames.Application.Json)]
    [EnableCors("CORS_Configuration")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMapper _mapper;

        protected readonly INotificationsService _notificationsService;

        protected BaseController(IMapper mapper, INotificationsService notificationsService)
        {
            _mapper = mapper;
            _notificationsService = notificationsService;
        }

        [NonAction]
        protected IActionResult InternalServerError(Exception e)
        {
            string msg = $"Internal server error: {e.Message}";
            Log.Error(e);
            var resultDTO = _mapper.Map<ResultDTO>(Result.Fail(msg));
            return StatusCode(StatusCodes.Status500InternalServerError, resultDTO);
        }

        [NonAction]
        protected IActionResult InternalServerError(Result result)
        {
            Log.Error("Internal server error:");
            result.LogError();
            _notificationsService.SendResult(result);
            var resultDTO = _mapper.Map<ResultDTO>(result);
            return StatusCode(StatusCodes.Status500InternalServerError, resultDTO);
        }

        [NonAction]
        protected IActionResult BadRequest(int id, string nameOf = "")
        {
            var error = new Error($"The Id: {id} was 0 or lower");

            if (nameOf != string.Empty)
            {
                error = new Error($"The Id parameter \"{nameOf}\" has an invalid id of {id}");
            }

            return BadRequest(Result.Fail(error));
        }

        [NonAction]
        protected IActionResult BadRequestInvalidId()
        {
            return BadRequest(Result.Fail("The Id was 0 or lower"));
        }

        [NonAction]
        protected IActionResult BadRequest(Result result)
        {
            _notificationsService.SendResult(result);
            var resultDTO = _mapper.Map<ResultDTO>(result);
            return new BadRequestObjectResult(resultDTO);
        }

        [NonAction]
        protected IActionResult Ok<T>(Result<T> result)
        {
            var resultDTO = _mapper.Map<ResultDTO<T>>(result);
            return new OkObjectResult(resultDTO);
        }

        private IActionResult ConvertToActionResult<T>(Result<T> result)
        {
            // Status Code 200
            if (result.IsSuccess)
            {
                var resultDTO = _mapper.Map<ResultDTO<T>>(result);
                return new OkObjectResult(resultDTO);
            }

            var failedResult = _mapper.Map<ResultDTO>(result);
            if (result.Has400BadRequestError())
            {
                return new BadRequestObjectResult(failedResult);
            }

            if (result.Has404NotFoundError())
            {
                return new NotFoundObjectResult(failedResult);
            }

            // Status Code 500
            return new ObjectResult(failedResult)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }


        private IActionResult ConvertToActionResult(Result result)
        {
            // Status Code 200
            if (result.IsSuccess)
            {
                var resultDTO = _mapper.Map<ResultDTO>(result);
                return new OkObjectResult(resultDTO);
            }

            var failedResult = _mapper.Map<ResultDTO>(result);
            if (result.Has400BadRequestError())
            {
                return new BadRequestObjectResult(failedResult);
            }

            if (result.Has404NotFoundError())
            {
                return new NotFoundObjectResult(failedResult);
            }

            // Status Code 500
            return new ObjectResult(failedResult)
            {
                StatusCode = StatusCodes.Status500InternalServerError,
            };
        }

        [NonAction]
        protected IActionResult ToActionResult(Result result)
        {
            return ConvertToActionResult(result);
        }

        [NonAction]
        protected IActionResult ToActionResult<TEntity, TDTO>(Result<TEntity> result)
        {
            Result<TDTO> resultDto;
            if (result.IsSuccess)
            {
                resultDto = Result.Ok(_mapper.Map<TDTO>(result.Value));
            }
            else
            {
                resultDto = result.ToResult();
            }

            return ConvertToActionResult(resultDto);
        }
    }
}