// Controllers/UsersController.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Factories;
using API.Models;
using API.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        [HttpGet(Name = "GetAllUsers")]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsers()
        {
            try
            {
                // using-blokki varmistaa, että kun repo-instanssia ei enää tarvita
                // kutustaan UsersSQLiteRepositoryn Dispose-metodia
                // eli tietokantayhteys suljetaan ja siivotaan pois
                using (var repo = UsersRepositoryFactory.Create())
                {
                    var users = await repo.GetAll();
                    return Ok(users);
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                {
                    return Conflict(
                        "The provided username already exists. Please choose a different one."
                    );
                }
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpGet("{id}", Name = "GetUserById")]
        public async Task<ActionResult<AppUser>> GetUserById(long id)
        {
            try
            {
                using (var repo = UsersRepositoryFactory.Create())
                {
                    var user = await repo.GetById(id);
                    if (user == null)
                    {
                        return NotFound("User not found");
                    }
                    return Ok(user);
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                {
                    return Conflict(
                        "The provided username already exists. Please choose a different one."
                    );
                }
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpPost(Name = "AddUser")]
        public async Task<ActionResult<AppUser>> AddNewUser(AddUserRequest request)
        {
            try
            {
                using (var repo = UsersRepositoryFactory.Create())
                {
                    var user = await repo.Save(
                        request.FirstName,
                        request.LastName,
                        request.Username
                    );
                    if (user == null)
                    {
                        return Problem(
                            $"error creating user",
                            statusCode: StatusCodes.Status500InternalServerError
                        );
                    }

                    return Ok(user);
                }
            }
            catch (Exception ex)
            {
                return Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPatch("{id}", Name = "UpdateUserById")]
        public async Task<ActionResult<AppUser>> UpdateUser(long id, AddUserRequest request)
        {
            try
            {
                using (var repo = UsersRepositoryFactory.Create())
                {
                    var user = await repo.Save(
                        request.FirstName,
                        request.LastName,
                        request.Username,
                        id
                    );

                    if (user == null)
                    {
                        return Problem(
                            $"Error updating user",
                            statusCode: StatusCodes.Status500InternalServerError
                        );
                    }

                    return user;
                }
            }
            catch (SqliteException ex)
            {
                if (ex.SqliteErrorCode == 19)
                {
                    return Conflict(
                        "The provided username already exists. Please choose a different one."
                    );
                }
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }

        [HttpDelete("{id}", Name = "RemoveUserById")]
        public async Task<ActionResult<AppUser>> RemoveUser(long id)
        {
            try
            {
                using (var repo = UsersRepositoryFactory.Create())
                {
                    bool removed = await repo.Remove(id);

                    if (removed)
                    {
                        return NoContent();
                    }

                    return NotFound("user not found");
                }
            }
            catch (Exception ex)
            {
                return Problem(
                    $"Database error: {ex.Message}",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        }
    }
}
